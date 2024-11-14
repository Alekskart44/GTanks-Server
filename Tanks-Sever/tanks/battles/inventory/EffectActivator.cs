using System;
using System.Threading;

namespace Tanks_Sever.tanks.battles.effects
{
    public class EffectActivator
    {
        private static readonly EffectActivator instance = new EffectActivator();
        private Timer timer;

        private EffectActivator()
        {
        }

        public static EffectActivator Instance => instance;

        public void ActivateEffect(Action effectTask, int delay)
        {
            timer = new Timer(_ => effectTask.Invoke(), null, delay, Timeout.Infinite);
        }
    }
}
