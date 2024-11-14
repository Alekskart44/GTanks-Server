namespace Tanks_Sever.tanks.users.garage.enums
{
    public enum PropertyType
    {
        DAMAGE,
        DAMAGE_PER_SECOND,
        AIMING_ERROR,
        CONE_ANGLE,
        SHOT_AREA,
        SHOT_FREQUENCY,
        SHOT_RANGE,
        TURN_SPEED,
        MECH_RESISTANCE,
        PLASMA_RESISTANCE,
        RAIL_RESISTANCE,
        VAMPIRE_RESISTANCE,
        ARMOR,
        TURRET_TURN_SPEED,
        FIRE_RESISTANCE,
        THUNDER_RESISTANCE,
        FREEZE_RESISTANCE,
        RICOCHET_RESISTANCE,
        HEALING_RADUIS,
        HEAL_RATE,
        VAMPIRE_RATE,
        SPEED,
        UNKNOWN
    }

    public static class PropertyTypeExtensions
    {
        public static string ToStringValue(this PropertyType typeProperty)
        {
            switch (typeProperty)
            {
                case PropertyType.DAMAGE:
                    return "damage";
                case PropertyType.DAMAGE_PER_SECOND:
                    return "damage_per_second";
                case PropertyType.AIMING_ERROR:
                    return "aiming_error";
                case PropertyType.CONE_ANGLE:
                    return "cone_angle";
                case PropertyType.SHOT_AREA:
                    return "shot_area";
                case PropertyType.SHOT_FREQUENCY:
                    return "shot_frequency";
                case PropertyType.SHOT_RANGE:
                    return "shot_range";
                case PropertyType.TURN_SPEED:
                    return "turn_speed";
                case PropertyType.MECH_RESISTANCE:
                    return "mech_resistance";
                case PropertyType.PLASMA_RESISTANCE:
                    return "plasma_resistance";
                case PropertyType.RAIL_RESISTANCE:
                    return "rail_resistance";
                case PropertyType.VAMPIRE_RESISTANCE:
                    return "vampire_resistance";
                case PropertyType.ARMOR:
                    return "armor";
                case PropertyType.TURRET_TURN_SPEED:
                    return "turret_turn_speed";
                case PropertyType.FIRE_RESISTANCE:
                    return "fire_resistance";
                case PropertyType.THUNDER_RESISTANCE:
                    return "thunder_resistance";
                case PropertyType.FREEZE_RESISTANCE:
                    return "freeze_resistance";
                case PropertyType.RICOCHET_RESISTANCE:
                    return "ricochet_resistance";
                case PropertyType.HEALING_RADUIS:
                    return "healing_radius";
                case PropertyType.HEAL_RATE:
                    return "heal_rate";
                case PropertyType.VAMPIRE_RATE:
                    return "vampire_rate";
                case PropertyType.SPEED:
                    return "speed";
                case PropertyType.UNKNOWN:
                    return "unknown";
                default:
                    return null;
            }
        }
    }
}
