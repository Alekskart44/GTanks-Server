namespace Tanks_Sever.tanks.users.garage.enums
{
    public enum ItemType
    {
        WEAPON,
        ARMOR,
        COLOR,
        INVENTORY,
        PLUGIN
    }

    public static class ItemTypeExtensions
    {
        public static string ToStringValue(this ItemType typeItem)
        {
            switch (typeItem)
            {
                case ItemType.WEAPON:
                    return "1";
                case ItemType.ARMOR:
                    return "2";
                case ItemType.COLOR:
                    return "3";
                case ItemType.INVENTORY:
                    return "4";
                case ItemType.PLUGIN:
                    return "5";
                default:
                    return null;
            }
        }
    }
}
