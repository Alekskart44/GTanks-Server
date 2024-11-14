using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.tanks.math
{
    public class Vector3
    {
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;
        public float Z { get; set; } = 0.0f;
        public double Rot { get; set; } = 0.0;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double DistanceTo(Vector3 to)
        {
            return Math.Sqrt(Pow2(X - to.X) + Pow2(Y - to.Y) + Pow2(Z - to.Z));
        }

        public double DistanceToWithoutZ(Vector3 to)
        {
            return Math.Sqrt(Pow2(X - to.X) + Pow2(Y - to.Y));
        }

        private double Pow2(double value)
        {
            return Math.Pow(value, 2.0);
        }
    }
}
