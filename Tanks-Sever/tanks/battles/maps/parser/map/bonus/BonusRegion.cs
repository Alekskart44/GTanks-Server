using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tanks_Sever.tanks.battles.bonuses;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.maps.parser.map.bonus
{
    public class BonusRegion
    {
        [XmlElement("max")]
        public Vector3d max { get; set; } = new Vector3d();
        [XmlElement("min")]
        public Vector3d min { get; set; } = new Vector3d();
        [XmlElement("bonus-type")]
        public List<BonusType> type { get; set; } = new List<BonusType>();

        public Vector3d GetMax()
        {
            return max;
        }

        public void SetMax(Vector3d max)
        {
            this.max = max;
        }

        public Vector3d GetMin()
        {
            return min;
        }

        public void SetMin(Vector3d min)
        {
            this.min = min;
        }

        public void SetBonusType(string value)
        {
            this.type.Add(BonusType.GetType(value));
        }

        public List<BonusType> GetType()
        {
            return type;
        }

        public override string ToString()
        {
            return $"BONUS-REGION[TYPE = {string.Join(", ", type)}] max: {max} min: {min}";
        }

        public bonuses.BonusRegion ToServerBonusRegion()
        {
            string[] convert = new string[type.Count];
            for (int i = 0; i < type.Count; i++)
            {
                convert[i] = type[i].GetValue();
            }

            return new bonuses.BonusRegion(ToVector3(max), ToVector3(min), convert);
        }

        public Vector3 ToVector3(Vector3d v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}