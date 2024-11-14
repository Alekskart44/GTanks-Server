using System.Collections.Generic;
using Tanks_Sever.tanks.users.garage.enums;

namespace Tanks_Sever.tanks.battles.tanks.colormaps
{
    public class ColormapsFactory
    {
        private static Dictionary<string, Colormap> colormaps = new Dictionary<string, Colormap>();

        public static void AddColormap(string id, Colormap colormap)
        {
            colormaps[id] = colormap;
        }

        public static Colormap GetColormap(string id)
        {
            colormaps.TryGetValue(id, out Colormap colormap);
            return colormap;
        }

        public static ColormapResistanceType GetResistanceType(PropertyType pType)
        {
            ColormapResistanceType type = ColormapResistanceType.NULL;

            switch (pType)
            {
                case PropertyType.MECH_RESISTANCE:
                    type = ColormapResistanceType.SMOKY;
                    break;
                case PropertyType.PLASMA_RESISTANCE:
                    type = ColormapResistanceType.TWINS;
                    break;
                case PropertyType.RAIL_RESISTANCE:
                    type = ColormapResistanceType.RAILGUN;
                    break;
                case PropertyType.VAMPIRE_RESISTANCE:
                    type = ColormapResistanceType.ISIDA;
                    break;
                case PropertyType.FIRE_RESISTANCE:
                    type = ColormapResistanceType.FLAMETHROWER;
                    break;
                case PropertyType.THUNDER_RESISTANCE:
                    type = ColormapResistanceType.THUNDER;
                    break;
                case PropertyType.FREEZE_RESISTANCE:
                    type = ColormapResistanceType.FREZEE;
                    break;
                case PropertyType.RICOCHET_RESISTANCE:
                    type = ColormapResistanceType.RICOCHET;
                    break;
            }

            return type;
        }
    }
}
