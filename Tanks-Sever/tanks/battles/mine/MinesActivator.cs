using System;
using System.Timers;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.battles.mine
{
    public class MineActivator
    {
        private const string PUT_MINE_COMMAND = "put_mine";
        private const string ACTIVATE_MINE_COMMAND = "activate_mine";
        private BattlefieldModel bfModel;
        private ServerMine mine;
        private static readonly int ActivationTime = 2000; // Время активации мины в миллисекундах
        private System.Timers.Timer timer;

        public MineActivator(BattlefieldModel bfModel, ServerMine mine)
        {
            this.bfModel = bfModel;
            this.mine = mine;
        }

        // Метод для отправки информации о минах всем игрокам
        public void PutMine()
        {
            bfModel.SendToAllPlayers(CommandType.BATTLE, PUT_MINE_COMMAND, JSONUtils.ParsePutMineCommand(this.mine));
        }

        // Метод для активации мины, который будет вызываться по таймеру
        public void ActivateMine(object sender, ElapsedEventArgs e)
        {
            bfModel.SendToAllPlayers(CommandType.BATTLE, ACTIVATE_MINE_COMMAND, mine.GetId().ToString());
            timer.Stop(); // Остановить таймер после активации, чтобы он больше не срабатывал
            timer.Dispose(); // Очистить ресурсы таймера
        }

        // Метод для запуска таймера на активацию мины
        public void ScheduleActivation()
        {
            timer = new System.Timers.Timer(ActivationTime); // Установка таймера с временем активации
            timer.Elapsed += ActivateMine;
            timer.AutoReset = false; // Таймер должен сработать только один раз
            timer.Start();
            PutMine();
        }
    }

    public class MinesActivatorService
    {
        private static readonly MinesActivatorService instance = new MinesActivatorService();

        private MinesActivatorService() { }

        public static MinesActivatorService Instance => instance;

        public void Activate(BattlefieldModel model, ServerMine mine)
        {
            var activator = new MineActivator(model, mine);
            activator.ScheduleActivation(); // Запуск процесса активации мины
        }
    }
}
