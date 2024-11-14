using Tanks_Sever.tanks.users.garage.enums;

namespace Tanks_Sever.tanks.users.garage.items
{
    public class PropertyItem
    {
        public PropertyType Property { get; }
        public string Value { get; }

        public PropertyItem(PropertyType property, string value)
        {
            Property = property;
            Value = value;
        }
    }
}
