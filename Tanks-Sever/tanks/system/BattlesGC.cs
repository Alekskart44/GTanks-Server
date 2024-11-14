using System;
using System;
using System.Collections.Generic;
using System.Timers;


using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.services;

namespace Tanks_Sever.tanks.system
{
    public static class BattlesGC
    {
        private const double TIME_FOR_REMOVING_EMPTY_BATTLE = 20000;
        private static Dictionary<BattlefieldModel, System.Timers.Timer> battlesForRemove = new Dictionary<BattlefieldModel, System.Timers.Timer>();

        private static readonly LobbysServices lobbysServices = LobbysServices.Instance;
        private static readonly AutoEntryServices autoEntryServices = AutoEntryServices.Instance;

        public static void AddBattleForRemove(BattlefieldModel battle)
        {
            if (battle != null)
            {
                var timer = new System.Timers.Timer(TIME_FOR_REMOVING_EMPTY_BATTLE);
                timer.Elapsed += (sender, e) => RemoveEmptyBattle(battle);
                timer.AutoReset = false; // Чтобы срабатывал только один раз
                timer.Start();

                battlesForRemove[battle] = timer;
            }
        }

        public static void CancelRemoving(BattlefieldModel model)
        {
            if (battlesForRemove.TryGetValue(model, out System.Timers.Timer timer))
            {
                timer.Stop();
                timer.Dispose();
                battlesForRemove.Remove(model);
            }
        }

        private static void RemoveEmptyBattle(BattlefieldModel battle)
        {
            Console.WriteLine("[BattlesGarbageCollector]: battle[" + battle.battleInfo + "] has been deleted by inactivity.");
            BattlesList.RemoveBattle(battle.battleInfo);
            autoEntryServices.BattleDisposed(battle);
        }
    }
}