using Tanks_Sever.tanks.main.procotol;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.users.garage.items;
using Tanks_Sever.tanks.users.garage;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.lobby.chat;
using Tanks_Sever.tanks.system;
using Tanks_Sever.tanks.lobby.top;
using Tanks_Sever.tanks.battles.maps;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.battles.maps.parser;
using System.Text.Json;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.battles.spectator;
using Repository;
using System.Numerics;

namespace Tanks_Sever.tanks.lobby
{
    public class LobbyManager
    {
        private User localUser;
        public ProtocolTransfer transfer;
        public SpectatorController spectatorController;
        private ChatLobby chatLobby = ChatLobby.Instance;
        private LobbysServices lobbysServices = LobbysServices.Instance;
        private HallOfFame hallOfFame = HallOfFame.GetInstance();
        public BattlefieldPlayerController battle;
        private AutoEntryServices autoEntryServices = AutoEntryServices.Instance;

        public LobbyManager(ProtocolTransfer transfer, User localUser)
        {
            this.transfer = transfer;
            this.localUser = localUser;

            localUser.setUserLocation(UserLocation.GARAGE);
            lobbysServices.AddLobby(this);
        }

        public void ExecuteCommand(Command command)
        {
            string _name;
            switch (command.Type)
            {
                case CommandType.GARAGE:

                    if (command.Args[0].Equals("try_mount_item"))
                    {
                        OnMountItem(command.Args[1]);
                    }

                    if (command.Args[0].Equals("try_update_item"))
                    {
                        OnTryUpdateItem(command.Args[1]);
                    }

                    if (command.Args[0].Equals("get_garage_data") && localUser.GetGarage().MountHull != null && localUser.GetGarage().MountTurret != null && localUser.GetGarage().MountColormap != null)
                    {
                        transfer.Send(CommandType.GARAGE, "init_mounted_item", StringUtils.ConcatStrings(localUser.GetGarage().MountHull.Id, "_m", localUser.GetGarage().MountHull.ModificationIndex.ToString()));
                        transfer.Send(CommandType.GARAGE, "init_mounted_item", StringUtils.ConcatStrings(localUser.GetGarage().MountTurret.Id, "_m", localUser.GetGarage().MountTurret.ModificationIndex.ToString()));
                        transfer.Send(CommandType.GARAGE, "init_mounted_item", StringUtils.ConcatStrings(localUser.GetGarage().MountColormap.Id, "_m", localUser.GetGarage().MountColormap.ModificationIndex.ToString()));
                    }

                    if (command.Args[0].Equals("try_buy_item"))
                    {
                        OnTryBuyItem(command.Args[1], int.Parse(command.Args[2]));
                    }
                    break;
                case CommandType.LOBBY:
                    if (command.Args[0].Equals("get_hall_of_fame_data"))
                    {
                        localUser.setUserLocation(UserLocation.HALL_OF_FAME);
                        hallOfFame.InitHallFromCollection(GenericManager.repository.GetAll<User>());
                        transfer.Send(CommandType.LOBBY, "init_hall_of_fame", JSONUtils.ParseHallOfFame(hallOfFame));
                    }

                    if (command.Args[0].Equals("get_garage_data"))
                    {
                        SendGarage();
                    }

                    if (command.Args[0].Equals("get_data_init_battle_select"))
                    {
                        SendMapsInit();
                    }

                    if (command.Args[0].Equals("check_battleName_for_forbidden_words"))
                    {
                        _name = command.Args.Length > 0 ? command.Args[1] : "";
                        CheckBattleName(_name);
                    }

                    if (command.Args[0].Equals("try_create_battle_dm"))
                    {
                        TryCreateBattleDM(command.Args[1], command.Args[2], int.Parse(command.Args[3]), int.Parse(command.Args[4]), int.Parse(command.Args[5]), int.Parse(command.Args[6]), int.Parse(command.Args[7]), StringToBoolean(command.Args[8]), StringToBoolean(command.Args[9]), StringToBoolean(command.Args[10]));
                    }

                    if (command.Args[0].Equals("try_create_battle_tdm"))
                    {
                        TryCreateTDMBattle(command.Args[1]);
                    }

                    if (command.Args[0].Equals("try_create_battle_ctf"))
                    {
                        TryCreateCTFBattle(command.Args[1]);
                    }

                    if (command.Args[0].Equals("get_show_battle_info"))
                    {
                        SendBattleInfo(command.Args[1]);
                    }

                    if (command.Args[0].Equals("enter_battle"))
                    {
                        OnEnterInBattle(command.Args[1]);
                    }

                    if (command.Args[0].Equals("enter_battle_team"))
                    {
                        OnEnterInTeamBattle(command.Args[1], bool.Parse(command.Args[2]));
                    }

                    if (command.Args[0].Equals("enter_battle_spectator"))
                    {
                        if (GetLocalUser().GetType() == TypeUser.DEFAULT)
                        {
                            return;
                        }

                        enterInBattleBySpectator(command.Args[1]);
                    }

                    break;
                case CommandType.LOBBY_CHAT:
                    chatLobby.AddMessage(new ChatMessage(localUser, command.Args[0], StringToBoolean(command.Args[1]), command.Args[2].Equals("NULL") ? null : GenericManager.repository.SingleByNickname<User>(command.Args[2]), false, this));
                    break;
                case CommandType.BATTLE:
                    if (battle != null)
                    {
                        battle.ExecuteCommand(command);
                    }

                    if (spectatorController != null)
                    {
                        spectatorController.ExecuteCommand(command);
                    }

                    break;
                case CommandType.SYSTEM:
                    _name = command.Args[0];
                    if (_name.Equals("c01"))
                    {
                        Kick();
                    }
                    break;
            }
        }

        private void SendTableMessage(string message)
        {
            transfer.Send(CommandType.LOBBY, "server_message", message);
        }

        private void SendBattleInfo(string id)
        {
            transfer.Send(CommandType.LOBBY, "show_battle_info", JSONUtils.ParseBattleInfoShow(BattlesList.GetBattleInfoById(id), GetLocalUser().GetType() != TypeUser.DEFAULT && GetLocalUser().GetType() != TypeUser.TESTER));
        }

        private void TryCreateBattleDM(string gameName, string mapId, int time, int kills, int maxPlayers, int minRang, int maxRang, bool isPrivate, bool pay, bool mm)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - localUser.getAntiCheatData().lastTimeCreationBattle <= 300000)
            {
                if (localUser.getAntiCheatData().countCreatedBattles >= 3)
                {
                    if (localUser.getAntiCheatData().countWarningForFludCreateBattle >= 5)
                    {
                        Kick();
                    }

                    SendTableMessage("Вы можете создавать не более трех битв в течении 5 минут.");
                    ++localUser.getAntiCheatData().countWarningForFludCreateBattle;
                    return;
                }
            }
            else
            {
                localUser.getAntiCheatData().countCreatedBattles = 0;
                localUser.getAntiCheatData().countWarningForFludCreateBattle = 0;
            }

            BattleInfo battle = new BattleInfo();
            Map map = MapsLoader.Maps[mapId];
            if (maxRang < minRang)
            {
                maxRang = minRang;
            }

            if (maxPlayers < 2)
            {
                maxPlayers = 2;
            }

            if (time <= 0 && kills <= 0)
            {
                time = 900;
                kills = 0;
            }

            if (maxPlayers > map.MaxPlayers)
            {
                maxPlayers = map.MaxPlayers;
            }

            if (kills > 999)
            {
                kills = 999;
            }

            battle.Name = gameName;
            battle.Map = MapsLoader.Maps[mapId];
            battle.Time = time;
            battle.NumKills = kills;
            battle.MaxPeople = maxPlayers;
            battle.MinRank = minRang;
            battle.CountPeople = 0;
            battle.MaxRank = maxRang;
            battle.Team = false;
            battle.IsPrivate = isPrivate;
            battle.IsPaid = pay;
            BattlesList.TryCreateBattle(battle);
            localUser.getAntiCheatData().lastTimeCreationBattle = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ++localUser.getAntiCheatData().countCreatedBattles;
        }

        private void TryCreateTDMBattle(string json)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - localUser.getAntiCheatData().lastTimeCreationBattle <= 300000)
            {
                if (localUser.getAntiCheatData().countCreatedBattles >= 3)
                {
                    if (localUser.getAntiCheatData().countWarningForFludCreateBattle >= 5)
                    {
                        Kick();
                    }

                    SendTableMessage("Вы можете создавать не более трех битв в течении 5 минут.");
                    ++localUser.getAntiCheatData().countWarningForFludCreateBattle;
                    return;
                }
            }
            else
            {
                localUser.getAntiCheatData().countCreatedBattles = 0;
                localUser.getAntiCheatData().countWarningForFludCreateBattle = 0;
            }

            BattleInfo battle = new BattleInfo();
            try
            {
                var parser = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                battle.BattleType = "TDM";
                battle.IsPaid = parser["pay"].GetBoolean();
                battle.IsPrivate = parser["privateBattle"].GetBoolean();
                battle.FriendlyFire = parser["frielndyFire"].GetBoolean();
                battle.Name = parser["gameName"].GetString();
                battle.Map = MapsLoader.Maps[parser["mapId"].GetString()];
                battle.MaxPeople = parser["numPlayers"].GetInt32();
                battle.NumKills = parser["numKills"].GetInt32();
                battle.MinRank = parser["minRang"].GetInt32();
                battle.MaxRank = parser["maxRang"].GetInt32();
                battle.Team = true;
                battle.Time = parser["time"].GetInt32();
                battle.Autobalance = parser["autoBalance"].GetBoolean();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return; 
            }

            Map map = battle.Map;
            if (battle.MaxRank < battle.MinRank)
            {
                battle.MaxRank = battle.MinRank;
            }

            if (battle.MaxPeople < 2)
            {
                battle.MaxPeople = 2;
            }

            if (battle.Time <= 0 && battle.NumKills <= 0)
            {
                battle.Time = 900;
                battle.NumKills = 0;
            }

            if (battle.MaxPeople > map.MaxPlayers)
            {
                battle.MaxPeople = map.MaxPlayers;
            }

            if (battle.NumKills > 999)
            {
                battle.NumKills = 999;
            }

            BattlesList.TryCreateBattle(battle);
            localUser.getAntiCheatData().lastTimeCreationBattle = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ++localUser.getAntiCheatData().countCreatedBattles;
        }

        private void TryCreateCTFBattle(string json)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - localUser.getAntiCheatData().lastTimeCreationBattle <= 300000)
            {
                if (localUser.getAntiCheatData().countCreatedBattles >= 3)
                {
                    if (localUser.getAntiCheatData().countWarningForFludCreateBattle >= 5)
                    {
                        Kick();
                    }

                    SendTableMessage("Вы можете создавать не более трех битв в течении 5 минут.");
                    ++localUser.getAntiCheatData().countWarningForFludCreateBattle;
                    return;
                }
            }
            else
            {
                localUser.getAntiCheatData().countCreatedBattles = 0;
                localUser.getAntiCheatData().countWarningForFludCreateBattle = 0;
            }

            BattleInfo battle = new BattleInfo();
            try
            {
                var parser = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                battle.BattleType = "CTF";
                battle.IsPaid = parser["pay"].GetBoolean();
                battle.IsPrivate = parser["privateBattle"].GetBoolean();
                battle.FriendlyFire = parser["frielndyFire"].GetBoolean();
                battle.Name = parser["gameName"].GetString();
                battle.Map = MapsLoader.Maps[parser["mapId"].GetString()];
                battle.MaxPeople = parser["numPlayers"].GetInt32();
                battle.NumFlags = parser["numFlags"].GetInt32();
                battle.MinRank = parser["minRang"].GetInt32();
                battle.MaxRank = parser["maxRang"].GetInt32();
                battle.Team = true;
                battle.Time = parser["time"].GetInt32();
                battle.Autobalance = parser["autoBalance"].GetBoolean();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            Map map = battle.Map;
            if (battle.MaxRank < battle.MinRank)
            {
                battle.MaxRank = battle.MinRank;
            }

            if (battle.MaxPeople < 2)
            {
                battle.MaxPeople = 2;
            }

            if (battle.Time <= 0 && battle.NumFlags <= 0)
            {
                battle.Time = 900;
                battle.NumFlags = 0;
            }

            if (battle.MaxPeople > map.MaxPlayers)
            {
                battle.MaxPeople = map.MaxPlayers;
            }

            if (battle.NumFlags > 999)
            {
                battle.NumFlags = 999;
            }

            BattlesList.TryCreateBattle(battle);
            localUser.getAntiCheatData().lastTimeCreationBattle = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ++localUser.getAntiCheatData().countCreatedBattles;
        }

        private void OnEnterInBattle(string battleId)
        {
            localUser.setUserLocation(UserLocation.BATTLE);
            autoEntryServices.RemovePlayer(GetLocalUser().GetNickname());
            if (battle == null) 
            {
                BattleInfo battleInfo = BattlesList.GetBattleInfoById(battleId);
                if (battleInfo != null)
                {
                   if (battleInfo.Model.Players.Count() < battleInfo.MaxPeople)
                   {
                        battle = new BattlefieldPlayerController(this, battleInfo.Model, "NONE");
                        ++battleInfo.CountPeople;
                        Console.WriteLine("incration");
                        if (!battleInfo.Team)
                        {
                            lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, StringUtils.ConcatStrings("update_count_users_in_dm_battle", ";", battleId, ";", battle.battleModel.battleInfo.CountPeople.ToString()));
                        }
                        else 
                        {
                            lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "update_count_users_in_team_battle", JSONUtils.ParseUpdateCountPeopleCommand(battleInfo));
                        }

                        transfer.Send(CommandType.BATTLE, "init_battle_model", JSONUtils.ParseBattleModelInfo(battleInfo, false));
                        lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "add_player_to_battle", JSONUtils.ParseAddPlayerCommand(battle, battleInfo));
                    }
                }
            }
        }

        private void OnEnterInTeamBattle(string battleId, bool red)
        {
            localUser.setUserLocation(UserLocation.BATTLE);
            if (battle == null)
            {
                BattleInfo battleInfo = BattlesList.GetBattleInfoById(battleId);
                if (battleInfo != null)
                {
                    if (battleInfo.Model.Players.Count() < battleInfo.MaxPeople * 2)
                    {
                        if (red)
                        {
                            ++battleInfo.RedPeople;
                        }
                        else
                        {
                            ++battleInfo.BluePeople;
                        }

                        battle = new BattlefieldPlayerController(this, battleInfo.Model, red ? "RED" : "BLUE");
                        lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "update_count_users_in_team_battle", JSONUtils.ParseUpdateCountPeopleCommand(battleInfo));
                        transfer.Send(CommandType.BATTLE, "init_battle_model", JSONUtils.ParseBattleModelInfo(battleInfo, false));
                        lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "add_player_to_battle", JSONUtils.ParseAddPlayerCommand(battle, battleInfo));
                    }
                }
            }
        }

        private void enterInBattleBySpectator(String battleId)
        {
            BattleInfo battle = BattlesList.GetBattleInfoById(battleId);
            if (battle != null)
            {
                battle.Model.spectatorModel.AddSpectator(spectatorController = new SpectatorController(this, battle.Model, battle.Model.spectatorModel));
                localUser.setUserLocation(UserLocation.BATTLE);
                transfer.Send(CommandType.BATTLE, "init_battle_model", JSONUtils.ParseBattleModelInfo(battle, true));
                Console.WriteLine("User " + localUser.GetNickname() + " enter in battle by spectator.");
            }
        }

        public void OnExitFromBattle()
        {
            if (battle != null)
            {
                if (autoEntryServices.RemovePlayer(battle.battleModel, GetLocalUser().GetNickname(), battle.playerTeamType, battle.battleModel.battleInfo.Team))
                {
                    battle.Destroy(true);
                }
                else
                {
                    battle.Destroy(false);
                }

                battle = null;
            }

            if (spectatorController != null)
            {
                spectatorController.OnDisconnect();
                spectatorController = null;
            }


            transfer.Send(CommandType.LOBBY_CHAT, "init_messages", JSONUtils.ParseChatLobbyMessages(chatLobby.GetMessages()));
        }

        public void OnExitFromStatistic()
        {
            OnExitFromBattle();
            SendMapsInit();
        }

        private void CheckBattleName(string name)
        {
            transfer.Send(CommandType.LOBBY, "check_battle_name", name);
        }

        private void SendMapsInit()
        {
            localUser.setUserLocation(UserLocation.BATTLESELECT);
            transfer.Send(CommandType.LOBBY, "init_battle_select", JSONUtils.ParseBattleMapList());
        }

        private void SendGarage()
        {
            localUser.setUserLocation(UserLocation.GARAGE);
            transfer.Send(CommandType.GARAGE, "init_garage_items", JSONUtils.ParseGarageUser(localUser).Trim());
            transfer.Send(CommandType.GARAGE, "init_market", JSONUtils.ParseMarketItems(localUser));
        }

        private void OnMountItem(string itemId) 
        {
            if (localUser.GetGarage().MountItem(itemId))
            {
                transfer.Send(CommandType.GARAGE, "mount_item", itemId);
                localUser.GetGarage().ParseJSONData();
                GenericManager.repository.Update(localUser.GetGarage());
            }
            else
            {
                transfer.Send(CommandType.GARAGE, "try_mount_item_NO");
            }
        }

        private void OnTryUpdateItem(string itemId) 
        {
            Item item = GarageItemsLoader.Items[itemId.Substring(0, itemId.Length - 3)];
            int modificationId = int.Parse(itemId.Substring(itemId.Length - 1));
            if (CheckMoney(item.Modifications[modificationId + 1].Price))
            {
                if (GetLocalUser().GetRang() + 1 < item.Modifications[modificationId + 1].Rank)
                {
                    return;
                }

                if (localUser.GetGarage().UpdateItem(itemId))
                {
                    transfer.Send(CommandType.GARAGE, "update_item", itemId);
                    addCrystall(-item.Modifications[modificationId + 1].Price);
                    localUser.GetGarage().ParseJSONData();
                    GenericManager.repository.Update(localUser.GetGarage());
                }

            }
            else 
            {
                transfer.Send(CommandType.GARAGE, "try_update_NO");
            }
        }

        private void OnTryBuyItem(string itemId, int count)
        {
            if (count > 0 && count <= 9999)
            {
                Item item = GarageItemsLoader.Items[itemId.Substring(0, itemId.Length - 3)];
                Item fromUser = null;
                int price = item.Price * count;
                int itemRank = item.Modifications[0].Rank;
                if (CheckMoney(price))
                {
                    if (GetLocalUser().GetRang() + 1 < itemRank)
                    {
                        return;
                    }

                    if ((fromUser = localUser.GetGarage().BuyItem(itemId, count)) != null)
                    {
                        transfer.Send(CommandType.GARAGE, "buy_item", StringUtils.ConcatStrings(item.Id, "_m", item.ModificationIndex.ToString()), JSONUtils.ParseItemInfo(fromUser));
                        addCrystall(-price);
                        localUser.GetGarage().ParseJSONData();
                        GenericManager.repository.Update(localUser.GetGarage());
                    }
                    else
                    {
                        transfer.Send(CommandType.GARAGE, "try_buy_item_NO");
                    }
                }
            }
            else 
            {
                CrystallToZero();
            }
        }

        private bool CheckMoney(int buyValue)
        {
            return localUser.GetCrystall() - buyValue >= 0;
        }

        private void addCrystall(int value)
        {
            localUser.AddCrystall(value);
            transfer.Send(CommandType.LOBBY, "add_crystall", localUser.GetCrystall().ToString());
            GenericManager.repository.Update(localUser);
        }

        public void CrystallToZero()
        {
            localUser.SetCrystall(0);
            transfer.Send(CommandType.LOBBY, "add_crystall", localUser.GetCrystall().ToString());
            GenericManager.repository.Update(localUser);
        }

        private bool StringToBoolean(string src)
        {
            return string.Equals(src, "true", StringComparison.OrdinalIgnoreCase);
        }

        public void OnDisconnect()
        {
            lobbysServices.RemoveLobby(this);

            if (spectatorController != null)
            {
                spectatorController.OnDisconnect();
                spectatorController = null;
            }

            if (battle != null)
            {
                battle.OnDisconnect();
                battle = null;
            }

            localUser.Session = null;
        }

        public void Kick()
        {
            transfer.CloseConnection();
        }

        public User GetLocalUser()
        {
            return localUser;
        }
    }
}
