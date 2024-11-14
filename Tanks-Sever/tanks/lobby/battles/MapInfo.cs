using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.lobby.battles
{
    public class MapInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GameName { get; set; }
        public int MaxPeople { get; set; }
        public int MaxRank { get; set; }
        public int MinRank { get; set; }
        public string ThemeName { get; set; }
        public bool Ctf { get; set; }
        public bool Tdm { get; set; }
    }
}
