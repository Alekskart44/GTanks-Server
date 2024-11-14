using System.Text.Json;
using Tanks_Sever.tanks.users.garage.enums;
using Tanks_Sever.tanks.system.localization;
using System.Text;
using Tanks_Sever.tanks.users.garage.items;
using Tanks_Sever.tanks.battles.tanks.colormaps;

namespace Tanks_Sever.tanks.users.garage
{
    public class GarageItemsLoader
    {
        public static Dictionary<string, Item> Items { get; private set; }
        private static int index = 1;

        public static void LoadFromConfig(string turrets, string hulls, string colormaps, string inventory, string subscription)
        {
            if (Items == null)
            {
                Items = new Dictionary<string, Item>();
            }

            for (int i = 0; i < 5; i++)
            {
                StringBuilder builder = new StringBuilder();
                string filePath = i switch
                {
                    0 => inventory,
                    1 => turrets,
                    2 => hulls,
                    _ => colormaps
                };

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            builder.Append(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                ParseAndInitItems(builder.ToString(), i == 0 ? ItemType.INVENTORY : (i == 1 ? ItemType.WEAPON : (i == 2 ? ItemType.ARMOR : ItemType.COLOR)));
            }
        }

        private static void ParseAndInitItems(string json, ItemType typeItem)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            var itemsArray = jsonDocument.RootElement.GetProperty("items").EnumerateArray();

            foreach (var item in itemsArray)
            {
                string id = item.GetProperty("id").GetString();
                var name = LocalizationHandler.RegisterString(item.GetProperty("name_ru").GetString() , item.GetProperty("name_en").GetString());
                var description = LocalizationHandler.RegisterString(item.GetProperty("description_ru").GetString(), item.GetProperty("description_en").GetString());

                int priceM0 = int.Parse(item.GetProperty("price_m0").GetString());
                int priceM1 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("price_m1").GetString()) : priceM0;
                int priceM2 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("price_m2").GetString()) : priceM0;
                int priceM3 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("price_m3").GetString()) : priceM0;

                int rangM0 = int.Parse(item.GetProperty("rang_m0").GetString());
                int rangM1 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("rang_m1").GetString()) : rangM0;
                int rangM2 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("rang_m2").GetString()) : rangM0;
                int rangM3 = typeItem != ItemType.COLOR && typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? int.Parse(item.GetProperty("rang_m3").GetString()) : rangM0;

                PropertyItem[] propertysItemM0 = null;
                PropertyItem[] propertysItemM1 = null;
                PropertyItem[] propertysItemM2 = null;
                PropertyItem[] propertysItemM3 = null;
                int countModification = typeItem == ItemType.COLOR ? 1 : (typeItem != ItemType.INVENTORY && typeItem != ItemType.PLUGIN ? 4 : (int)item.GetProperty("count_modifications").GetInt32());

                for (int m = 0; m < countModification; m++)
                {
                    var propertysArray = item.GetProperty($"propertys_m{m}").EnumerateArray();
                    var property = new PropertyItem[propertysArray.Count()];

                    int index = 0;
                    foreach (var prop in propertysArray)
                    {
                        property[index] = new PropertyItem(GetType(prop.GetProperty("type").GetString()), prop.GetProperty("value").GetString());
                        index++;
                    }

                    switch (m)
                    {
                        case 0: propertysItemM0 = property; break;
                        case 1: propertysItemM1 = property; break;
                        case 2: propertysItemM2 = property; break;
                        case 3: propertysItemM3 = property; break;
                    }
                }

                if (typeItem == ItemType.COLOR || typeItem == ItemType.INVENTORY || typeItem == ItemType.PLUGIN)
                {
                    propertysItemM1 = propertysItemM0;
                    propertysItemM2 = propertysItemM0;
                    propertysItemM3 = propertysItemM0;
                }

                ModificationInfo[] mods = new ModificationInfo[4];
                mods[0] = new ModificationInfo($"{id}_m0", priceM0, rangM0);
                mods[0].Propertys = propertysItemM0;
                mods[1] = new ModificationInfo($"{id}_m1", priceM1, rangM1);
                mods[1].Propertys = propertysItemM1;
                mods[2] = new ModificationInfo($"{id}_m2", priceM2, rangM2);
                mods[2].Propertys = propertysItemM2;
                mods[3] = new ModificationInfo($"{id}_m3", priceM3, rangM3);
                mods[3].Propertys = propertysItemM3;

                bool specialItem = item.TryGetProperty("special_item", out var specialItemElement) && specialItemElement.GetBoolean();
                Items[id] = new Item(id, description, typeItem == ItemType.INVENTORY || typeItem == ItemType.PLUGIN, index, propertysItemM0, typeItem, 0, name, propertysItemM1, priceM1, rangM1, priceM0, rangM0, mods, specialItem, 0);
                index++;

                if (typeItem == ItemType.COLOR)
                {
                    var colormap = new Colormap();
                    PropertyItem[] properties = mods[0].Propertys;

                    foreach (var property in properties)
                    {
                        PropertyType propertyType = GetType(property.Property.ToStringValue());

                        ColormapResistanceType resistanceType = ColormapsFactory.GetResistanceType(propertyType);

                        int resistanceValue = GetInt(property.Value.Replace("%", ""));
                        colormap.AddResistance(resistanceType, resistanceValue);
 
                    }

                    ColormapsFactory.AddColormap($"{id}_m0", colormap);
                }

            }
        }

        private static int GetInt(string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }

        private static PropertyType GetType(string s)
        {
            foreach (var type in Enum.GetValues(typeof(PropertyType)))
            {
                if (type.ToString().Equals(s, StringComparison.OrdinalIgnoreCase))
                {
                    return (PropertyType)type;
                }
            }

            return PropertyType.UNKNOWN;
        }
    }
}
