using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.spectator
{
    public class SpectatorChatModel
    {
        private const string CHAT_SPECTATOR_COMMAND = "spectator_message";
        private SpectatorModel spModel;
        private BattlefieldModel bfModel;

        public SpectatorChatModel(SpectatorModel spModel)
        {
            this.spModel = spModel;
        }

        public void OnMessage(string message, SpectatorController spectator)
        {
            spModel.getBattleModel().SendToAllPlayers(CommandType.BATTLE, CHAT_SPECTATOR_COMMAND, message);
        }
    }
}