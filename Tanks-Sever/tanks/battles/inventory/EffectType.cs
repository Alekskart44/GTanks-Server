using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.effect
{
    public enum EffectType
    {
        HEALTH,
        ARMOR,
        DAMAGE,
        NITRO,
        MINE
    }

    public static class EffectTypeExtensions
    {
        public static string ToFriendlyString(this EffectType effectType)
        {
            return effectType switch
            {
                EffectType.HEALTH => "health",
                EffectType.ARMOR => "armor",
                EffectType.DAMAGE => "damage",
                EffectType.NITRO => "nitro",
                EffectType.MINE => "mine",
                _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
            };
        }
    }
}