using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.bonuses
{
    public enum BonusType
    {
        GOLD,
        CRYSTALL,
        ARMOR,
        HEALTH,
        DAMAGE,
        NITRO
    }

    public static class BonusTypeExtensions
    {
        public static string ToStringType(this BonusType bonusType)
        {
            return bonusType switch
            {
                BonusType.GOLD => "gold",
                BonusType.CRYSTALL => "crystall",
                BonusType.ARMOR => "armor",
                BonusType.HEALTH => "health",
                BonusType.DAMAGE => "damage",
                BonusType.NITRO => "nitro"
            };
        }
    }
}