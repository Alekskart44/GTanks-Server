using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.utils;

namespace Tanks_Sever.tanks.battles.bonuses
{
    public class BonusSpawn : IDisposable
    {
        private const int DisappearingTimeDrop = 30;
        private const int DisappearingTimeMoney = 300;
        private BattlefieldModel battlefieldModel;
        private Random random = new Random();
        private int inc = 0;
        private int prevFund = 0;
        private int crystallFund;
        private int goldFund;
        private int nextGoldFund;
        private Thread spawnThread;
        private bool isRunning;

        public BonusSpawn(BattlefieldModel model)
        {
            battlefieldModel = model ?? throw new ArgumentNullException(nameof(model));
            nextGoldFund = (int)RandomUtils.GetRandom(700.0f, 730.0f);
        }

        public void StartSpawning()
        {
            if (spawnThread != null && spawnThread.IsAlive)
            {
                Console.WriteLine("Спавн уже запущен.");
                return;
            }

            isRunning = true;
            spawnThread = new Thread(Run) { IsBackground = true };
            spawnThread.Start();
            Console.WriteLine("Спавн бонусов запущен.");
        }

        public void StopSpawning()
        {
            isRunning = false;
            if (spawnThread != null)
            {
                spawnThread.Join(); // Ждет завершения потока
                Console.WriteLine("Спавн бонусов остановлен.");
            }
        }

        private void Run()
        {
            while (isRunning && battlefieldModel != null)
            {
                try
                {
                    Thread.Sleep(5000);
                    if (battlefieldModel.Players == null || battlefieldModel.Players.Count() == 0)
                    {
                        Console.WriteLine("Нет игроков для спавна. Ожидание...");
                        continue;
                    }
                    SpawnRandomBonus();
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine("Поток прерван: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка в спавне бонусов: " + e.Message);
                }
            }
        }

        public void SpawnRandomBonus()
        {
            bool wasSpawned = random.Next(2) == 0; // Увеличить шанс
            if (wasSpawned && battlefieldModel.Players.Count > 0)
            {
                int id = random.Next(4);
                BonusType bonusType;

                switch (id)
                {
                    case 0:
                        bonusType = BonusType.NITRO;
                        break;
                    case 1:
                        bonusType = BonusType.ARMOR;
                        break;
                    case 2:
                        bonusType = BonusType.HEALTH;
                        break;
                    case 3:
                        bonusType = BonusType.DAMAGE;
                        break;
                    default:
                        bonusType = BonusType.NITRO;
                        break;
                }

                int count = random.Next(1, 4);
                for (int i = 0; i < count; i++)
                {
                    SpawnBonus(bonusType);
                    Console.WriteLine($"Спавн бонуса: {bonusType}");
                }
            }
        }


        public void SpawnBonus(BonusType type)
        {
            BonusRegion region = null;
            Bonus bonus = null;
            int index;

            switch (type)
            {
                case BonusType.GOLD:
                    if (battlefieldModel.battleInfo.Map.GoldsRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.GoldsRegions.Count);
                        region = battlefieldModel.battleInfo.Map.GoldsRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.GOLD);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeMoney);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна GOLD.");
                    }
                    break;

                case BonusType.CRYSTALL:
                    if (battlefieldModel.battleInfo.Map.CrystallsRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.CrystallsRegions.Count);
                        region = battlefieldModel.battleInfo.Map.CrystallsRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.CRYSTALL);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeMoney);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна CRYSTALL.");
                    }
                    break;

                case BonusType.ARMOR:
                    if (battlefieldModel.battleInfo.Map.ArmorsRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.ArmorsRegions.Count);
                        region = battlefieldModel.battleInfo.Map.ArmorsRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.ARMOR);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeDrop);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна ARMOR.");
                    }
                    break;

                case BonusType.HEALTH:
                    if (battlefieldModel.battleInfo.Map.HealthsRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.HealthsRegions.Count);
                        region = battlefieldModel.battleInfo.Map.HealthsRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.HEALTH);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeDrop);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна HEALTH.");
                    }
                    break;

                case BonusType.DAMAGE:
                    if (battlefieldModel.battleInfo.Map.DamagesRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.DamagesRegions.Count);
                        region = battlefieldModel.battleInfo.Map.DamagesRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.DAMAGE);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeDrop);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна DAMAGE.");
                    }
                    break;

                case BonusType.NITRO:
                    if (battlefieldModel.battleInfo.Map.NitrosRegions.Count > 0)
                    {
                        index = random.Next(battlefieldModel.battleInfo.Map.NitrosRegions.Count);
                        region = battlefieldModel.battleInfo.Map.NitrosRegions[index];
                        bonus = new Bonus(GetRandomSpawnPosition(region), BonusType.NITRO);
                        battlefieldModel.SpawnBonus(bonus, inc, DisappearingTimeDrop);
                        Console.WriteLine($"Бонус {type} спавнится в {bonus.Position}");
                    }
                    else
                    {
                        Console.WriteLine("Нет доступных регионов для спавна NITRO.");
                    }
                    break;

                default:
                    Console.WriteLine($"Неизвестный тип бонуса: {type}");
                    break;
            }

            if (bonus != null)
            {
                inc++;
                Console.WriteLine($"Бонус {type} с ID {inc - 1} успешно добавлен в поле.");
            }
            else
            {
                Console.WriteLine($"Не удалось создать бонус {type}.");
            }
        }


        public void BattleFinished()
        {
            prevFund = 0;
            crystallFund = 0;
            goldFund = 0;
            nextGoldFund = (int)RandomUtils.GetRandom(700.0f, 730.0f);
        }

        private Vector3 GetRandomSpawnPosition(BonusRegion region)
        {
            float x = region.Min.X + (region.Max.X - region.Min.X) * (float)random.NextDouble();
            float y = region.Min.Y + (region.Max.Y - region.Min.Y) * (float)random.NextDouble();
            float z = region.Max.Z;
            return new Vector3(x, y, z);
        }

        public void UpdatedFund()
        {
            int deff = (int)battlefieldModel.tanksKillModel.GetBattleFund() - prevFund;
            goldFund += deff;
            crystallFund += deff;

            if (goldFund >= nextGoldFund)
            {
                SpawnBonus(BonusType.GOLD);
                nextGoldFund = (int)RandomUtils.GetRandom(700.0f, 730.0f);
                goldFund = 0;
            }

            if (crystallFund >= 6)
            {
                int count = (int)RandomUtils.GetRandom(1.0f, 6.0f);
                for (int i = 0; i < count; i++)
                {
                    SpawnBonus(BonusType.CRYSTALL);
                }
                crystallFund = 0;
            }

            prevFund = (int)battlefieldModel.tanksKillModel.GetBattleFund();
        }

        public void Dispose()
        {
            StopSpawning();
            battlefieldModel = null;
        }
    }
}
