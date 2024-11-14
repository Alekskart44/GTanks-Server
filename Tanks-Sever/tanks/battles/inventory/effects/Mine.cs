using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.effects;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.inventory.effects
{
    public class Mine : Effect
    {
        private static readonly EffectActivator effectActivatorService = EffectActivator.Instance;
        private BattlefieldPlayerController player;
        private Vector3 tankPos;
        private bool deactivated;

        public void Activate(BattlefieldPlayerController player, bool fromInventory, Vector3 tankPos)
        {
            if (!fromInventory)
            {
                throw new ArgumentException("Mine was not caused from inventory!");
            }

            this.player = player;
            this.tankPos = tankPos;
            lock (player.tank.ActiveEffects)
            {
                player.tank.ActiveEffects.Add(this);
            }

            player.battleModel.battleMinesModel.TryPutMine(player, tankPos);
            ActivateEffectAsync();
        }

        private async void ActivateEffectAsync()
        {
            await Task.Delay(GetDurationTime());
            Deactivate();
        }

        public void Deactivate()
        {
            deactivated = true;
            player.tank.ActiveEffects.Remove(this);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, "disnable_effect", player.GetUser().GetNickname(), GetID().ToString());
        }

        public EffectType GetEffectType()
        {
            return EffectType.MINE;
        }

        public int GetID()
        {
            return 5;
        }

        public int GetDurationTime()
        {
            return 30000;
        }
    }
}