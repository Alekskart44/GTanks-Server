using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.maps.parser.map.bonus;

namespace Tanks_Sever.tanks.battles.maps.parser.map
{
    [XmlRoot("bonus-regions")]
    public class BonusRegions
    {
        [XmlElement("bonus-region")]
        public List<BonusRegion> _bonusRegions;

        public List<BonusRegion> GetBonusRegions()
        {
            return _bonusRegions;
        }

        public void SetBonusRegions(List<BonusRegion> bonusRegions)
        {
            _bonusRegions = bonusRegions;
        }
    }
}