using System;
using System.Threading;
using gtanks.battles;
using Tanks_Sever.tanks.battles.ctf.flags;
using Tanks_Sever.tanks.battles.ctf;

namespace gtanks.battles.ctf
{
    public class FlagReturnTimer
    {
        public bool stop = false;
        private readonly CTFModel _ctfModel;
        private readonly FlagServer _flag;
        private Thread _thread;

        public FlagReturnTimer(CTFModel ctfModel, FlagServer flag)
        {
            _ctfModel = ctfModel;
            _flag = flag;
            _thread = new Thread(Run) { Name = "FlagReturnTimer THREAD" };
        }

        public void Start()
        {
            _thread.Start();
        }

        private void Run()
        {
            try
            {
                Thread.Sleep(20000);
                if (!stop)
                {
                    _ctfModel.ReturnFlag(null, _flag);
                }
            }
            catch (ThreadInterruptedException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Stop()
        {
            stop = true;
            _thread.Interrupt(); // Прерывает поток
        }
    }
}
