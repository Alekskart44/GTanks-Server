using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.battles.maps;

namespace Tanks_Sever.tanks.lobby.battles
{
    public class BattleInfo
    {
        public string BattleId { get; set; }
        public Map Map { get; set; }
        public string BattleType { get; set; } = "DM";
        public string Name { get; set; }
        public bool Team { get; set; }
        public int RedPeople { get; set; }
        public int BluePeople { get; set; }
        public int CountPeople { get; set; }
        public int MaxPeople { get; set; }
        public int MinRank { get; set; }
        public int MaxRank { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPrivate { get; set; }
        public int Time { get; set; }
        public int NumKills { get; set; }
        public int NumFlags { get; set; }
        public bool FriendlyFire { get; set; }
        public MapInfo Info { get; set; }
        public int ScoreBlue { get; set; } = 0;
        public int ScoreRed { get; set; } = 0;
        public bool Autobalance { get; set; }
        public bool Inventory { get; set; }
        public bool WithBonus { get; set; }
        public BattlefieldModel Model { get; set; }

        public override string ToString()
        {
            return $"{{ {Name} | {BattleId} }}";
        }
    }
}
