using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.spectator;
using System.Text.Json;

namespace Tanks_Sever.tanks.battles.mine
{
    public class BattleMinesModel
    {
        private const string REMOVE_MINES_COMMAND = "remove_mines";
        private const string INIT_MINES_COMMAND = "init_mines";
        private const string HIT_MINE_COMMAND = "hit_mine";
        private const string INIT_MINE_MODEL_COMMAND = "init_mine_model";

        private BattlefieldModel bfModel;
        private Dictionary<BattlefieldPlayerController, List<ServerMine>> mines;
        private static string _initObjectData;

        private MinesActivatorService minesActivatorService = MinesActivatorService.Instance;

        private int _incrationId;
        private static int minDamage;
        private static int maxDamage;

        static BattleMinesModel()
        {
            minDamage = 120;
            maxDamage = 240;
        }

        public BattleMinesModel(BattlefieldModel bfModel)
        {
            this.bfModel = bfModel;
            mines = new Dictionary<BattlefieldPlayerController, List<ServerMine>>();
        }

        public void SendMines(BattlefieldPlayerController player)
        {
            player.Send(CommandType.BATTLE, INIT_MINES_COMMAND, JSONUtils.ParseInitMinesCommand(mines));
        }

        public void SendMines(SpectatorController spectator)
        {
            spectator.SendCommand(CommandType.BATTLE, INIT_MINES_COMMAND, JSONUtils.ParseInitMinesCommand(mines));
        }

        private string ParseMine()
        {
            var obj = new Dictionary<string, object>();
            obj["activationTimeMsec"] = 2000;
            obj["farVisibilityRadius"] = 15;
            obj["nearVisibilityRadius"] = 5;
            obj["impactForce"] = 6.0;
            obj["minDistanceFromBase"] = 6;
            obj["radius"] = 1;

            return JsonSerializer.Serialize(obj);
        }

        public void InitModel(BattlefieldPlayerController player)
        {
            if (_initObjectData == null)
            {
                _initObjectData = ParseMine();
            }

            player.Send(CommandType.BATTLE, INIT_MINE_MODEL_COMMAND, _initObjectData);
        }

        public void InitModel(SpectatorController spectator)
        {
            if (_initObjectData == null)
            {
                _initObjectData = ParseMine();
            }

            spectator.SendCommand(CommandType.BATTLE, INIT_MINE_MODEL_COMMAND, _initObjectData);
        }

        public void TryPutMine(BattlefieldPlayerController player, Vector3 pos)
        {
            var mine = new ServerMine();
            mine.SetId(player.tank.Id + "_" + _incrationId);
            mine.SetOwner(player);
            mine.SetPosition(pos);

            if (!mines.TryGetValue(player, out var userMines))
            {
                userMines = new List<ServerMine> { mine };
                mines[player] = userMines;
            }
            else
            {
                userMines.Add(mine);
            }

            minesActivatorService.Activate(bfModel, mine);
            _incrationId++;
        }

        public void PlayerDied(BattlefieldPlayerController player)
        {
            if (mines.TryGetValue(player, out var userMines))
            {
                userMines.Clear();
                bfModel.SendToAllPlayers(CommandType.BATTLE, REMOVE_MINES_COMMAND, player.tank.Id);
            }
        }

        public void HitMine(BattlefieldPlayerController whoHit, string mineId)
        {
            BattlefieldPlayerController mineOwner = null;

            foreach (var serverMines in mines.Values)
            {
                var mine = serverMines.FirstOrDefault(m => m.GetId().Equals(mineId));
                if (mine != null)
                {
                    mineOwner = mine.GetOwner();
                    serverMines.Remove(mine);
                    break;
                }
            }

            bfModel.SendToAllPlayers(CommandType.BATTLE, HIT_MINE_COMMAND, mineId, whoHit.tank.Id);
            if (mineOwner != null)
            {
                bfModel.tanksKillModel.DamageTank(whoHit, mineOwner, RandomUtils.GetRandom(minDamage, maxDamage), false);
            }
        }
    }
}
