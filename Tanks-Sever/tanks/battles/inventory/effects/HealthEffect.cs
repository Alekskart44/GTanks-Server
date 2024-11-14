using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.effects;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.inventory.effects
{
    public class HealthEffect : Effect
    {
        private const int HP_IN_SEC = 30;
        private const int HP_FOR_ITERATION = 15;
        private int resource;
        private int accumulatedResource;
        private readonly EffectActivator effectActivatorService = EffectActivator.Instance;
        private BattlefieldPlayerController player;
        private bool fromInventory;
        private bool deactivated;

        public void Activate(BattlefieldPlayerController player, bool fromInventory, Vector3 tankPos)
        {
            this.fromInventory = fromInventory;
            this.player = player;
            this.resource = fromInventory ? (int)player.tank.GetHull().Hp : (int)player.tank.GetHull().Hp / 2;
            player.tank.ActiveEffects.Add(this);
            _ = Task.Run(() => Run());
        }

        public void Deactivate()
        {
            deactivated = true;
            player.tank.ActiveEffects.Remove(this);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, "disnable_effect", player.GetUser().GetNickname(), GetID().ToString());
        }

        private async Task Run()
        {
            while (true)
            {
                if (!deactivated)
                {
                    if (accumulatedResource + HP_FOR_ITERATION > resource)
                    {
                        HealTank(resource - accumulatedResource);
                    }
                    else
                    {
                        HealTank(HP_FOR_ITERATION);
                        accumulatedResource += HP_FOR_ITERATION;
                        await Task.Delay(500);

                        if (accumulatedResource <= resource)
                            continue;
                    }
                }

                if (!deactivated)
                {
                    Deactivate();
                }

                return;
            }
        }

        private void HealTank(int hp)
        {
            player.battleModel.tanksKillModel.HealPlayer(null, player, hp);
        }

        public EffectType GetEffectType()
        {
            return EffectType.HEALTH;
        }

        public int GetID()
        {
            return 1;
        }

        public int GetDurationTime()
        {
            return 5000;
        }
    }
}