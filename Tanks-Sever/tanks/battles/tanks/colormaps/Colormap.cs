using Tanks_Sever.tanks.battles.tanks.weapons;

namespace Tanks_Sever.tanks.battles.tanks.colormaps
{
    public class Colormap
    {

        private Dictionary<ColormapResistanceType, int> resistances = new Dictionary<ColormapResistanceType, int>();

        public void AddResistance(ColormapResistanceType type, int percent)
        {
            resistances[type] = percent;
        }

        public int GetResistance(EntityType weaponType)
        {
            ColormapResistanceType? resistanceType = GetResistanceTypeByWeapon(weaponType);
            if (resistanceType.HasValue)
            {
                resistances.TryGetValue(resistanceType.Value, out int resistance);
                return resistance;
            }
            return 0;
        }


        private ColormapResistanceType? GetResistanceTypeByWeapon(EntityType weaponType) 
        {
            ColormapResistanceType? type = null;
            switch (weaponType) 
            {
                case EntityType.SMOKY:
                    type = ColormapResistanceType.SMOKY;
                    break;
                case EntityType.FLAMETHROWER:
                    type = ColormapResistanceType.FLAMETHROWER;
                    break;
                case EntityType.TWINS:
                    type = ColormapResistanceType.TWINS;
                    break;
                case EntityType.RAILGUN:
                    type = ColormapResistanceType.RAILGUN;
                    break;
                case EntityType.ISIDA:
                    type = ColormapResistanceType.ISIDA;
                    break;
                case EntityType.THUNDER:
                    type = ColormapResistanceType.THUNDER;
                    break;
                case EntityType.FREZZE:
                    type = ColormapResistanceType.FREZEE;
                    break;
                case EntityType.RICOCHET:
                    type = ColormapResistanceType.RICOCHET;
                    break;
            }
            return type;
        }

    }
}
