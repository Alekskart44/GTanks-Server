using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.effects;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.inventory.effects
{
    public class NitroEffect : Effect
    {
        private const string CHANGE_TANK_SPEC_COMMAND = "change_spec_tank";
        private const long INVENTORY_TIME_ACTION = 60000;
        private const long DROP_TIME_ACTION = 40000;
        private readonly EffectActivator effectActivatorService = EffectActivator.Instance;
        private BattlefieldPlayerController player;
        private bool fromInventory;
        private bool deactivated;

        public void Activate(BattlefieldPlayerController player, bool fromInventory, Vector3 tankPos)
        {
            this.fromInventory = fromInventory;
            this.player = player;
            lock (player.tank.ActiveEffects)
            {
                player.tank.ActiveEffects.Add(this);
            }

            player.tank.Speed = AddPercent(player.tank.Speed, 30);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, CHANGE_TANK_SPEC_COMMAND, player.tank.Id, JSONUtils.ParseTankSpec(player.tank, true));
            _ = Task.Run(() => Run());
        }

        public void Deactivate()
        {
            deactivated = true;
            player.tank.ActiveEffects.Remove(this);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, "disnable_effect", player.GetUser().GetNickname(), GetID().ToString());
            player.tank.Speed = player.tank.GetHull().Speed;
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, CHANGE_TANK_SPEC_COMMAND, player.tank.Id, JSONUtils.ParseTankSpec(player.tank, true));
        }

        private async Task Run()
        {
            if (!deactivated)
            {
                await Task.Delay((int)(fromInventory ? INVENTORY_TIME_ACTION : DROP_TIME_ACTION));
                Deactivate();
            }
        }

        public EffectType GetEffectType()
        {
            return EffectType.NITRO;
        }

        public int GetID()
        {
            return 4;
        }

        private float AddPercent(float value, int percent)
        {
            return value / 100.0f * percent + value;
        }

        public int GetDurationTime()
        {
            return 60000;
        }
    }
}