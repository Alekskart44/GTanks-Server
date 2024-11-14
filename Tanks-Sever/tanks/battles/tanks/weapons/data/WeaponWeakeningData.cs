using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.tanks.weapons.data
{
    public class WeaponWeakeningData
    {
        public double MaximumDamageRadius { get; set; }
        public double MinimumDamagePercent { get; set; }
        public double MinimumDamageRadius { get; set; }

        // инициализации данных об ослаблении урона оружия
        public WeaponWeakeningData(double maximumDamageRadius, double minimumDamagePercent, double minimumDamageRadius)
        {
            MaximumDamageRadius = maximumDamageRadius;
            MinimumDamageRadius = minimumDamageRadius;
            MinimumDamagePercent = minimumDamagePercent;
        }
    }
}
