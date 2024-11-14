using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.managers;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.tanks
{
    public class TankRespawn
    {
        private static readonly int TIME_TO_PREPARE_SPAWN = 3000; // Время подготовки спавна
        private static readonly int TIME_TO_SPAWN = 5000; // Время до спавна
        private static readonly Dictionary<BattlefieldPlayerController, TankRespawn> tasks = new Dictionary<BattlefieldPlayerController, TankRespawn>();
        private static bool disposed = false;

        private BattlefieldPlayerController player;
        private Vector3 preparedPosition;
        private bool onlySpawn;
        private CancellationTokenSource cancellationTokenSource;

        public static void StartRespawn(BattlefieldPlayerController player, bool onlySpawn)
        {
            if (disposed || player == null || player.battleModel == null)
            {
                Console.WriteLine("Ошибка: Игрок или модель боя отсутствуют.");
                return;
            }

            if (tasks.ContainsKey(player))
            {
                Console.WriteLine("Ошибка: Задача спавна уже запущена для этого игрока.");
                return;
            }

            var respawnScheduler = new TankRespawn
            {
                player = player,
                onlySpawn = onlySpawn,
                cancellationTokenSource = new CancellationTokenSource()
            };

            tasks[player] = respawnScheduler;

            // Запуск задачи для подготовки и спавна танка
            respawnScheduler.PrepareAndSpawnAsync();
        }

        public static void Dispose()
        {
            disposed = true;
            foreach (var task in tasks.Values)
            {
                task.Cancel();
            }
            tasks.Clear();
        }

        public static void CancelRespawn(BattlefieldPlayerController player)
        {
            if (tasks.TryGetValue(player, out var task))
            {
                task.Cancel();
                tasks.Remove(player);
            }
        }

        private async void PrepareAndSpawnAsync()
        {
            try
            {
                await PrepareSpawnAsync(); // Подготовка к спавну
                if (!onlySpawn)
                {
                    await SpawnAsync(); // Спавн танка
                }
                else
                {
                    SpawnImmediately();
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Задача была отменена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при спавне танка: {ex.Message}");
            }
            finally
            {
                tasks.Remove(player); // Удаление задачи спавна
            }
        }

        private async Task PrepareSpawnAsync()
        {
            if (player?.tank == null || player.battleModel == null)
            {
                Console.WriteLine("Ошибка: Не удалось запустить спавн, так как танк или модель боя отсутствуют.");
                return;
            }

            preparedPosition = SpawnManager.GetSpawnState(player.battleModel.battleInfo.Map, player.playerTeamType);

            Console.WriteLine($"Подготовка к спавну на позиции: {preparedPosition}");

            player.Send(CommandType.BATTLE, "prepare_to_spawn", StringUtils.ConcatStrings(player.tank.Id, ";", preparedPosition.X.ToString(CultureInfo.InvariantCulture), "@", preparedPosition.Y.ToString(CultureInfo.InvariantCulture), "@", preparedPosition.Z.ToString(CultureInfo.InvariantCulture), "@", preparedPosition.Rot.ToString(CultureInfo.InvariantCulture)));

            await Task.Delay(TIME_TO_PREPARE_SPAWN, cancellationTokenSource.Token); // Ожидание подготовки спавна
        }

        private async Task SpawnAsync()
        {
            if (player?.tank == null || player.battleModel == null)
            {
                Console.WriteLine("Ошибка: Не удалось спавнить, так как танк или модель боя отсутствуют.");
                return;
            }

            player.tank.Position = preparedPosition;
            await Task.Delay(TIME_TO_SPAWN, cancellationTokenSource.Token); // Задержка перед спавном

            SpawnImmediately();
        }

        private void SpawnImmediately()
        {
            if (player?.tank == null || player.battleModel == null)
            {
                Console.WriteLine("Ошибка: Не удалось спавнить, так как танк или модель боя отсутствуют.");
                return;
            }

            player.battleModel.SendToAllPlayers(CommandType.BATTLE, "spawn",
                JSONUtils.ParseSpawnCommand(player, preparedPosition));

            player.tank.State = "newcome";
            Console.WriteLine($"Танк {player.tank.Id} был успешно заспавнен на позиции: {preparedPosition}");
        }

        private void Cancel()
        {
            cancellationTokenSource?.Cancel();
            Console.WriteLine($"Спавн для игрока {player?.tank?.Id} был отменен.");
        }
    }
}
