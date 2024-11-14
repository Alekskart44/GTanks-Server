using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.bonuses
{
    public class BonusRegion
    {
        public Vector3 Max;
        public Vector3 Min;
        public string[] Types;

        public BonusRegion(Vector3 max, Vector3 min, string[] types)
        {
            Max = max;
            Min = min;
            Types = types;
        }
    }
}
