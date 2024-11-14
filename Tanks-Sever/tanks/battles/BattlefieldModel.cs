using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.bonuses;
using Tanks_Sever.tanks.battles.bonuses.timer;
using Tanks_Sever.tanks.battles.ctf;
using Tanks_Sever.tanks.battles.inventory.visualization;
using Tanks_Sever.tanks.battles.managers;
using Tanks_Sever.tanks.battles.mine;
using Tanks_Sever.tanks.battles.spectator;
using Tanks_Sever.tanks.battles.tanks;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.tanks.statistic;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.system;
using Tanks_Sever.tanks.utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tanks_Sever.tanks.battles
{
    public class BattlefieldModel
    {
        public Dictionary<string, BattlefieldPlayerController> Players;
        public Dictionary<string, Bonus> ActiveBonuses = new Dictionary<string, Bonus>();

        public BattleInfo battleInfo;
        private bool battleFinish = false;
        private long endBattleTime = 0;
        public PlayersStatisticModel statistics;
        public TankKillModel tanksKillModel;
        public CTFModel ctfModel;
        public BonusSpawn bonusSpawn;
        public int incration = 0;
        private AutoEntryServices autoEntryServices = AutoEntryServices.Instance;
        public EffectsVisualizationModel effectsModel;
        public SpectatorModel spectatorModel;
        public BattleMinesModel battleMinesModel;

        public BattlefieldModel(BattleInfo battleInfo) 
        {
            this.battleInfo = battleInfo;
            statistics = new PlayersStatisticModel(this);
            tanksKillModel = new TankKillModel(this);

            spectatorModel = new SpectatorModel(this);
            effectsModel = new EffectsVisualizationModel(this);

            battleMinesModel = new BattleMinesModel(this);

            if (battleInfo.Time > 0)
            {
                StartTimeBattle();
            }

            if (battleInfo.BattleType.Equals("CTF"))
            {
                ctfModel = new CTFModel(this);
            }

            Players = new Dictionary<string, BattlefieldPlayerController>();
            ActiveBonuses = new Dictionary<string, Bonus>();
            new Thread(() => bonusSpawn = new BonusSpawn(this)).Start();
            BattlesGC.AddBattleForRemove(this);
        }

        private void StartTimeBattle()
        {
            endBattleTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (battleInfo.Time * 1000);
            Task.Run(async () =>
            {
                await Task.Delay(battleInfo.Time * 1000);

                Console.WriteLine("Battle end...");
                tanksKillModel.RestartBattle(true);
            });
        }

        public void BattleRestart()
        {
            if (battleInfo.Team)
            {
                SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "RED", battleInfo.ScoreRed.ToString());
                SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "BLUE", battleInfo.ScoreBlue.ToString());
            }

            battleFinish = false;

            foreach (var player in Players.Values)
            {
                if (player != null)
                {
                    player.statistic.Clear();
                    player.ClearEffects();
                    RespawnPlayer(player, false);
                }
            }

            long currentTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int prepareTimeLeft = (int)((currentTimeMillis + (battleInfo.Time * 1000) - currentTimeMillis) / 1000L);
            SendToAllPlayers(CommandType.BATTLE, "battle_restart", prepareTimeLeft.ToString());
            if (battleInfo.Time > 0)
            {
                StartTimeBattle();
            }

        }

        public void BattleFinish()
        {
            if (Players != null)
            {
                battleFinish = true;
                if (ActiveBonuses != null)
                {
                    ActiveBonuses.Clear();
                }

                bonusSpawn.BattleFinished();
                tanksKillModel.SetBattleFund(0);
                battleInfo.ScoreBlue = 0;
                battleInfo.ScoreRed = 0;

                foreach (var player in Players.Values)
                {
                    if (player != null)
                    {
                        TankRespawn.CancelRespawn(player);
                    }
                }

                autoEntryServices.BattleRestarted(this);
            }
        }

        public int GetTimeLeft()
        {
            return (int)((endBattleTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000);
        }

        public void Fire(BattlefieldPlayerController tank, string json)
        {
            sendToAllPlayersOther(tank, CommandType.BATTLE, "fire", tank.tank.Id, json);
        }

        public void StartFire(BattlefieldPlayerController tank)
        {
            sendToAllPlayersOther(tank, CommandType.BATTLE, "start_fire", tank.tank.Id);
        }

        public void StopFire(BattlefieldPlayerController tank)
        {
            sendToAllPlayersOther(tank, CommandType.BATTLE, "stop_fire", tank.tank.Id);
        }

        public void SpawnBonus(Bonus bonus, int inc, int disappearingTime)
        {
                string id = StringUtils.ConcatStrings(bonus.Type.ToString(), "_", inc.ToString());
                ActiveBonuses[id] = bonus;
                BonusesScheduler.RunRemoveTask(this, id, disappearingTime);
                SendToAllPlayers(CommandType.BATTLE, "spawn_bonus", JSONUtils.ParseBonusInfo(bonus, inc, disappearingTime));
        }

        public void RespawnPlayer(BattlefieldPlayerController controller, bool kill)
        {
            if (!battleFinish)
            {
                controller.Send(CommandType.BATTLE, "local_user_killed");
                battleMinesModel.PlayerDied(controller);
                if (kill)
                {
                    controller.ClearEffects();
                    SendToAllPlayers(CommandType.BATTLE, "kill_tank", controller.tank.Id, "suicide");
                    controller.statistic.AddDeaths();
                    statistics.ChangeStatistic(controller);
                    if (ctfModel != null && controller.flag != null)
                    {
                        ctfModel.DropFlag(controller, controller.tank.Position);
                    }
                }
                controller.tank.State = "suicide";
                TankRespawn.StartRespawn(controller, false);
            }
        }

        public void MoveTank(BattlefieldPlayerController controller) 
        {
            SendToAllPlayers(CommandType.BATTLE, "move", JSONUtils.ParseMoveCommand(controller));
        }

        private void SpawnPlayer(BattlefieldPlayerController controller)
        {
            if(!battleFinish)
            {
                TankRespawn.StartRespawn(controller, true);
            }
        }

        public void SetupTank(BattlefieldPlayerController controller) 
        {
            controller.tank.Id = controller.parentLobby.GetLocalUser().GetNickname();
        }

        public void AddPlayer(BattlefieldPlayerController controller)
        {
            SetupTank(controller);
            Players[controller.tank.Id] = controller;
            ++incration;
            BattlesGC.CancelRemoving(this);
        }

        public void RemoveUser(BattlefieldPlayerController controller, bool cache)
        {
            controller.ClearEffects();
            battleMinesModel.PlayerDied(controller);
            Players.Remove(controller.parentLobby.GetLocalUser().GetNickname());
            if (!cache)
            {
                if (!battleInfo.Team)
                {
                    --BattlesList.GetBattleInfoById(battleInfo.BattleId).CountPeople;
                }
                else if (controller.playerTeamType.Equals("RED"))
                {
                    --BattlesList.GetBattleInfoById(battleInfo.BattleId).RedPeople;
                }
                else
                {
                    --BattlesList.GetBattleInfoById(battleInfo.BattleId).BluePeople;
                }
            }

            if (ctfModel != null && controller.flag != null)
            {
                ctfModel.DropFlag(controller, controller.tank.Position);
            }

            SendToAllPlayers(CommandType.BATTLE, "remove_user", controller.tank.Id);
            if (Players.Count() == 0)
            {
                BattlesGC.AddBattleForRemove(this);
            }
        }

        public void InitLocalTank(BattlefieldPlayerController controller)
        {
            controller.userInited = true;
            Vector3 position = SpawnManager.GetSpawnState(battleInfo.Map, controller.playerTeamType);

            if (battleInfo.BattleType.Equals("CTF"))
            {
                controller.Send(CommandType.BATTLE, "init_ctf_model", JSONUtils.ParseCTFModelData(this));
            }

            controller.Send(CommandType.BATTLE, "init_gui_model", JSONUtils.ParseBattleData(this));
            controller.inventory.Init();
            battleMinesModel.InitModel(controller);
            battleMinesModel.SendMines(controller);
            SendAllTanks(controller);
            SendToAllPlayers(CommandType.BATTLE, "init_tank", JSONUtils.ParseTankData(this, controller, controller.parentLobby.GetLocalUser().GetGarage(), position, true, incration, controller.tank.Id, controller.parentLobby.GetLocalUser().GetNickname(), controller.parentLobby.GetLocalUser().GetRang()));
            statistics.ChangeStatistic(controller);
            effectsModel.SendInitData(controller);
            SpawnPlayer(controller);
            bonusSpawn.StartSpawning();
        }

        public void SendAllTanks(BattlefieldPlayerController controller)
        {
            foreach (var player in Players.Values)
            {
                if (player != controller && player.userInited)
                {
                    controller.Send(CommandType.BATTLE, "init_tank", JSONUtils.ParseTankData(this, player, player.parentLobby.GetLocalUser().GetGarage(), player.tank.Position, false, incration, player.tank.Id, player.parentLobby.GetLocalUser().GetNickname(), player.parentLobby.GetLocalUser().GetRang()));
                    statistics.ChangeStatistic(player);
                }
            }
        }

        public void ActivateTank(BattlefieldPlayerController player)
        {
            player.tank.State = "active";
            SendToAllPlayers(CommandType.BATTLE, "activate_tank", player.tank.Id);
        }

        public void SendToAllPlayers(CommandType type, params string[] args)
        {
            if (Players != null)
            {
                if (Players.Count != 0)
                {
                    foreach (var player in Players.Values)
                    {
                        if (player.userInited)
                        {
                            player.Send(type, args);
                        }
                    }
                }

                spectatorModel.SendCommandToSpectators(type, args);
            }
        }

        public void sendToAllPlayersOther(BattlefieldPlayerController other, CommandType type, params string[] args)
        {
            if (Players.Count() != 0)
            {
                foreach (var player in Players.Values)
                {
                    if (player.userInited && player != other)
                    {
                        player.Send(type, args);
                    }
                }
            }

            spectatorModel.SendCommandToSpectators(type, args);
        }

        public void Destroy()
        {
            Players.Clear();
            ActiveBonuses.Clear();
            tanksKillModel = null;
            Players = null;
            ActiveBonuses = null;
            battleInfo = null;
        }
    }
}
