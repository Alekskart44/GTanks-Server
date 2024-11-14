using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.system;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.spectator
{
    public class SpectatorModel
    {
        private Dictionary<string, SpectatorController> spectators;
        private BattlefieldModel bfModel;
        private SpectatorChatModel chatModel;

        public SpectatorModel(BattlefieldModel bfModel)
        {
            this.bfModel = bfModel;
            spectators = new Dictionary<string, SpectatorController>();
            chatModel = new SpectatorChatModel(this);
        }

        public void AddSpectator(SpectatorController spec)
        {
            spectators[spec.GetId()] = spec;
            BattlesGC.CancelRemoving(bfModel);
        }

        public void RemoveSpectator(SpectatorController spec)
        {
            spectators.Remove(spec.GetId());
            if (bfModel != null && bfModel.Players != null)
            {
                if (bfModel.Players.Count == 0 && spectators.Count == 0)
                {
                    BattlesGC.AddBattleForRemove(bfModel);
                }
            }
        }

        public SpectatorChatModel GetChatModel()
        {
            return chatModel;
        }

        public BattlefieldModel getBattleModel()
        {
            return bfModel;
        }

        public void SendCommandToSpectators(CommandType type, params string[] args)
        {
            foreach (var sc in spectators.Values)
            {
                sc.SendCommand(type, args);
            }
        }
    }
}