
using System.Timers;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.effects;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.inventory.effects
{
    public class ArmorEffect : Effect
    {
        private const long INVENTORY_TIME_ACTION = 60000L;
        private const long DROP_TIME_ACTION = 40000L;
        private static readonly EffectActivator effectActivatorService = EffectActivator.Instance;
        private BattlefieldPlayerController player;
        private bool fromInventory;
        private bool deactivated;
        private System.Timers.Timer timer;

        public void Activate(BattlefieldPlayerController player, bool fromInventory, Vector3 tankPos)
        {
            this.fromInventory = fromInventory;
            this.player = player;
            player.tank.ActiveEffects.Add(this);
            timer = new System.Timers.Timer(fromInventory ? INVENTORY_TIME_ACTION : DROP_TIME_ACTION);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false; // Деактивировать только один раз
            timer.Start();
        }

        public void Deactivate()
        {
            deactivated = true;
            player.tank.ActiveEffects.Remove(this);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, "disnable_effect", player.GetUser().GetNickname(), GetID().ToString());
            timer?.Stop(); // Остановить таймер, если он запущен
            timer?.Dispose(); // Освободить ресурсы таймера
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!deactivated)
            {
                Deactivate();
            }
        }

        public EffectType GetEffectType()
        {
            return EffectType.ARMOR;
        }

        public int GetID()
        {
            return 2;
        }

        public int GetDurationTime()
        {
            return 60000;
        }
    }
}