using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.spectator;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.inventory.visualization
{
    public class EffectsVisualizationModel
    {
        private BattlefieldModel battlefieldModel;

        public EffectsVisualizationModel(BattlefieldModel battlefieldModel)
        {
            this.battlefieldModel = battlefieldModel;
        }

        public void SendInitData(BattlefieldPlayerController player)
        {
            var effects = new List<Dictionary<string, object>>();

            foreach (var _player in battlefieldModel.Players.Values)
            {
                if (_player == player) continue;

                lock (_player.tank.ActiveEffects)
                {
                    foreach (var effect in _player.tank.ActiveEffects)
                    {
                        var effectData = new Dictionary<string, object>
                        {
                            { "userID", _player.GetUser().GetNickname() },
                            { "itemIndex", effect.GetID() },
                            { "durationTime", 60000 } // 60 секунд
                        };

                        effects.Add(effectData);
                    }
                }
            }

            var data = new Dictionary<string, object>
            {
                { "effects", effects }
            };

            string json = JsonSerializer.Serialize(data);
            player.Send(CommandType.BATTLE, "init_effects", json);
        }

        public void SendInitDataSpectator(SpectatorController player)
        {
            var effects = new List<Dictionary<string, object>>();

            foreach (var _player in battlefieldModel.Players.Values)
            {

                lock (_player.tank.ActiveEffects)
                {
                    foreach (var effect in _player.tank.ActiveEffects)
                    {
                        var effectData = new Dictionary<string, object>
                        {
                            { "userID", _player.GetUser().GetNickname() },
                            { "itemIndex", effect.GetID() },
                            { "durationTime", 60000 } // 60 секунд
                        };

                        effects.Add(effectData);
                    }
                }
            }

            var data = new Dictionary<string, object>
            {
                { "effects", effects }
            };

            string json = JsonSerializer.Serialize(data);
            player.SendCommand(CommandType.BATTLE, "init_effects", json);
        }

    }
}