using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.services;

namespace Tanks_Sever.tanks.lobby.battles
{
    public class BattlesList
    {
        private static List<BattleInfo> battles = new List<BattleInfo>();
        private static int countBattles = 0;

        private static LobbysServices lobbysServices = LobbysServices.Instance;

        public static bool TryCreateBattle(BattleInfo btl)
        {
            btl.BattleId = GenerateId(btl.Name, btl.Map.Id);
            if (GetBattleInfoById(btl.BattleId) != null)
            {
                return false;
            }
            else
            {
                battles.Add(btl);
                countBattles++;
                lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "create_battle", JSONUtils.ParseBattleInfoCreate(btl));
                var model = new BattlefieldModel(btl);
                btl.Model = model;
                return true;
            }
        }

        public static void RemoveBattle(BattleInfo battle)
        {
            if (battle != null)
            {
                lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, $"{string.Join(";", "remove_battle", battle.BattleId)}");
                if (battle.Model != null && battle.Model.Players != null)
                {
                     foreach (var player in battle.Model.Players.Values.ToList())
                     {
                        player.parentLobby.Kick();
                     }
                }
                battle.Model.Destroy();
                battles.Remove(battle);
            }
        }

        public static List<BattleInfo> GetList()
        {
            return battles;
        }

        private static string GenerateId(string gameName, string mapId)
        {
            var random = new Random();
            return $"{random.Next(50000)}@{gameName}@#{countBattles}";
        }

        public static BattleInfo GetBattleInfoById(string id)
        {
            return battles.FirstOrDefault(battle => battle.BattleId == id);
        }
    }
}
