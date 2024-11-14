using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.maps.parser.map.bonus;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.bonuses
{
    public class Bonus
    {
        public Vector3 Position { get; private set; }
        public BonusType Type { get; private set; }
        public long SpawnTime { get; private set; }

        public Bonus(Vector3 position, BonusType type)
        {
            Position = position;
            Type = type;
            SpawnTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}