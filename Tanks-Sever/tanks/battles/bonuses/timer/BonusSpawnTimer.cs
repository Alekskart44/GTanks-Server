using System;
using System.Collections.Generic;
using System.Threading;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.bonuses.timer
{
    public static class BonusesScheduler
    {
        private static readonly Dictionary<string, Timer> tasks = new Dictionary<string, Timer>();

        public static void RunRemoveTask(BattlefieldModel bfModel, string bonusId, long disappearingTime)
        {
            var rbt = new RemoveBonusTask
            {
                BfModel = bfModel,
                BonusId = bonusId
            };

            Timer timer = new Timer(state =>
            {
                rbt.Run();
            }, null, disappearingTime * 1000L - 1250L, Timeout.Infinite);

            tasks[bonusId] = timer;
        }

        public class RemoveBonusTask
        {
            public string BonusId { get; set; }
            public BattlefieldModel BfModel { get; set; }

            public void Run()
            {
                if (BfModel?.ActiveBonuses != null)
                {
                    BfModel.ActiveBonuses.Remove(BonusId);
                    BfModel.SendToAllPlayers(CommandType.BATTLE, "remove_bonus", BonusId);
                    tasks.Remove(BonusId);
                }
            }
        }
    }
}
