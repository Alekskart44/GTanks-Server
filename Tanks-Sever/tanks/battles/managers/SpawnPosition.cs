using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.managers
{
    public class SpawnPosition
    {
        public Vector3 Position { get; set; }
        public Vector3 Orientation { get; set; }

        public SpawnPosition(Vector3 position, Vector3 orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}