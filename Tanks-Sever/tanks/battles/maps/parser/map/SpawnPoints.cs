using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.maps.parser.map.spawn;

namespace Tanks_Sever.tanks.battles.maps.parser.map
{
    [XmlRoot("spawn-points")]
    public class SpawnPoints
    {
        [XmlElement("spawn-point")]
        public List<SpawnPosition> _spawnPositions;

        public List<SpawnPosition> GetSpawnPositions()
        {
            return _spawnPositions;
        }

        public void SetSpawnPositions(List<SpawnPosition> spawnPositions)
        {
            _spawnPositions = spawnPositions;
        }
    }
}