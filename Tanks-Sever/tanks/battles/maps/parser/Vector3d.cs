using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.maps.parser
{
    public class Vector3d
    {
        public float x { get; set; }

        public float y { get; set; }

        public float z { get; set; }

        public Vector3d(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d() { }

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public float Z
        {
            get => z;
            set => z = value;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"x = {x} y = {y} z = {z}";
        }
    }
}