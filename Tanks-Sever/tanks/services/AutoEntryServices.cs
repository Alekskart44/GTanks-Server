using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.lobby.chat;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.battles.tanks.statistic;

namespace Tanks_Sever.tanks.services;
public class AutoEntryServices
{
    private static readonly AutoEntryServices _instance = new AutoEntryServices();
    private const string QUARTZ_NAME = "AutoEntryServices GC";
    private const string QUARTZ_GROUP = "runner";

    private readonly ChatLobby chatLobby = ChatLobby.Instance;
    private readonly LobbysServices lobbysServices = LobbysServices.Instance;

    public Dictionary<string, Data> PlayersForAutoEntry = new Dictionary<string, Data>();
    private readonly Timer _cleanupTimer;

    private AutoEntryServices()
    {

        // Запуск задачи для очистки устаревших записей
        _cleanupTimer = new Timer(CleanupOldEntries, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    private void CleanupOldEntries(object state)
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expiredEntries = PlayersForAutoEntry.Values
            .Where(data => currentTime - data.CreatedTime >= 120000)
            .ToList();

        foreach (var data in expiredEntries)
        {
            RemovePlayer(data.Battle, data.UserId, data.TeamType, data.Battle.battleInfo.Team);
        }
    }

    public void RemovePlayer(string userId)
    {
        PlayersForAutoEntry.Remove(userId);
    }

    public bool RemovePlayer(BattlefieldModel data, string userId, string teamType, bool team)
    {
        if (!PlayersForAutoEntry.ContainsKey(userId))
        {
            return false;
        }

        lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "remove_player_from_battle",
            JSONUtils.ParseRemovePlayerCommand(userId, data.battleInfo.BattleId));

        if (!team)
        {
            data.battleInfo.CountPeople--;
            lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT,
                StringUtils.ConcatStrings("update_count_users_in_dm_battle", ";", data.battleInfo.BattleId, ";", data.battleInfo.CountPeople.ToString()));
        }
        else
        {
            if (teamType == "RED")
            {
                data.battleInfo.RedPeople--;
            }
            else
            {
                data.battleInfo.BluePeople--;
            }

            lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT,
                "update_count_users_in_team_battle", JSONUtils.ParseUpdateCountPeopleCommand(data.battleInfo));
        }

        PlayersForAutoEntry.Remove(userId);
        return true;
    }

    public void PrepareToEnter(LobbyManager lobby)
    {
        if (!PlayersForAutoEntry.TryGetValue(lobby.GetLocalUser().Nickname, out var data))
        {
            TransmitToLobby(lobby);
            return;
        }

        var bModel = data.Battle;
        if (bModel == null)
        {
            TransmitToLobby(lobby);
            return;
        }

        RemovePlayer(lobby.GetLocalUser().Nickname);
        var statistic = data.Statistic;
        var battleInfo = bModel.battleInfo;

        lobby.GetLocalUser().setUserLocation(UserLocation.BATTLE);
        lobby.battle = new BattlefieldPlayerController(lobby, bModel, data.TeamType);
        lobby.battle.statistic = statistic;
        //lobby.DisconnectListener.AddListener(lobby.Battle);
        lobby.transfer.Send(CommandType.BATTLE, "init_battle_model", JSONUtils.ParseBattleModelInfo(battleInfo, false));
    }

    private void TransmitToLobby(LobbyManager lobby)
    {
        lobby.transfer.Send(CommandType.GARAGE, "init_garage_items", JSONUtils.ParseGarageUser(lobby.GetLocalUser()).Trim());
        lobby.transfer.Send(CommandType.GARAGE, "init_market", JSONUtils.ParseMarketItems(lobby.GetLocalUser()));
        lobby.transfer.Send(CommandType.LOBBY_CHAT, "init_chat");
        lobby.transfer.Send(CommandType.LOBBY_CHAT, "init_messages", JSONUtils.ParseChatLobbyMessages(chatLobby.GetMessages()));
    }

    public bool NeedEnterToBattle(User user)
    {
        return PlayersForAutoEntry.ContainsKey(user.Nickname);
    }

    public void UserExit(BattlefieldPlayerController player)
    {
         var data = new Data
         {
             Battle = player.battleModel,
             Statistic = player.statistic,
             CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
             TeamType = player.playerTeamType,
             UserId = player.GetUser().Nickname
         };

         PlayersForAutoEntry[player.GetUser().Nickname] = data;
    }

    public List<Data> GetPlayersByBattle(BattlefieldModel battle)
    {
        return PlayersForAutoEntry.Values
            .Where(data => data.Battle != null && data.Battle == battle)
            .ToList();
    }

    public void BattleRestarted(BattlefieldModel battle)
    {
        foreach (var data in PlayersForAutoEntry.Values)
        {
            if (data.Battle == battle)
            {
                data.Statistic.Clear();
            }
        }
    }

    public void BattleDisposed(BattlefieldModel battle)
    {
        var usersToRemove = PlayersForAutoEntry.Values
            .Where(data => data.Battle == battle)
            .Select(data => data.UserId)
            .ToList();

        foreach (var userId in usersToRemove)
        {
            PlayersForAutoEntry.Remove(userId);
        }
    }

    public static AutoEntryServices Instance => _instance;

    public class Data
    {
        public BattlefieldModel Battle { get; set; }
        public PlayerStatistic Statistic { get; set; }
        public string TeamType { get; set; }
        public long CreatedTime { get; set; }
        public string UserId { get; set; }
    }
}
