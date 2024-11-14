using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.maps.parser.map.bonus;
using Tanks_Sever.tanks.battles.maps.parser.map.spawn;

namespace Tanks_Sever.tanks.battles.maps.parser.map
{
    [XmlRoot("map")]
    public class Map
    {
        [XmlElement("spawn-points")]
        public SpawnPoints SpawnPoints;

        [XmlElement("bonus-regions")]
        public BonusRegions BonusRegions;

        [XmlElement("ctf-flags")]
        public FlagsPositions FlagPositions;

        // Метод для получения позиции синего флага
        public Vector3d GetPositionBlueFlag()
        {
            return FlagPositions?.GetBlueFlag();
        }

        // Метод для получения позиции красного флага
        public Vector3d GetPositionRedFlag()
        {
            return FlagPositions?.GetRedFlag();
        }

        // Метод для получения спавн-позиций
        public List<SpawnPosition> GetSpawnPositions()
        {
            return SpawnPoints.GetSpawnPositions();
        }

        // Метод для получения бонусных регионов
        public List<BonusRegion> GetBonusesRegion()
        {
            return BonusRegions?.GetBonusRegions();
        }
    }
}
