using System;

namespace Tanks_Sever.tanks.battles.maps.parser.map.spawn
{
    public class SpawnPositionType
    {
        public static readonly SpawnPositionType BLUE = new SpawnPositionType();
        public static readonly SpawnPositionType RED = new SpawnPositionType();
        public static readonly SpawnPositionType NONE = new SpawnPositionType();

        private SpawnPositionType() { }

        public static SpawnPositionType GetType(string value)
        {
            return value.ToLower() switch
            {
                "blue" => BLUE,
                "red" => RED,
                "dm" => NONE,
                _ => NONE,
            };
        }
    }
}
