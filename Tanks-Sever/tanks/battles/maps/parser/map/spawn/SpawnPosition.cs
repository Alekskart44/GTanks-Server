using System;
using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.managers;

namespace Tanks_Sever.tanks.battles.maps.parser.map.spawn
{
    [XmlRoot("spawn-point")]
    public class SpawnPosition
    {
        [XmlElement("position")]
        public Vector3d Position;

        [XmlElement("rotation")] 
        public Vector3d Rotation;

        [XmlAttribute("type")]
        public string Type;

        public SpawnPosition() { }

        public SpawnPosition(Vector3d position, Vector3d rotation, string type)
        {
            Position = position;
            Rotation = rotation;
            Type = type;
        }

        public SpawnPositionType GetSpawnPositionType()
        {
            return SpawnPositionType.GetType(Type);
        }

        public override string ToString()
        {
            return $"{Position} direction: {Rotation}";
        }

        public Tanks_Sever.tanks.battles.managers.SpawnPosition ToServerSpawnPosition()
        {
            return new Tanks_Sever.tanks.battles.managers.SpawnPosition(ToVector3(Position), ToVector3(Rotation));
        }

        public Vector3 ToVector3(Vector3d v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public Vector3 GetVector3()
        {
            var vector3 = new Vector3(Position.X, Position.Y, Position.Z)
            {
                Rot = (double)Rotation.Z
            };
            return vector3;
        }
    }
}
