using gtanks.battles.ctf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.ctf.flags
{
    public class FlagServer
    {
        public string FlagTeamType { get; set; }
        public BattlefieldPlayerController Owner { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 BasePosition { get; set; }
        public FlagState State { get; set; }
        public FlagReturnTimer ReturnTimer { get; set; }
    }
}