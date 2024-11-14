using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.tanks.statistic
{
    public class PlayersStatisticModel
    {
        private readonly BattlefieldModel _bfModel;

        public PlayersStatisticModel(BattlefieldModel bfModel)
        {
            _bfModel = bfModel;
        }

        public void ChangeStatistic(BattlefieldPlayerController player)
        {
            _bfModel.SendToAllPlayers(CommandType.BATTLE, "update_player_statistic", JSONUtils.ParsePlayerStatistic(player));
        }
    }
}