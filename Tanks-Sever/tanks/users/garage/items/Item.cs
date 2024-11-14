using Tanks_Sever.tanks.users.garage.enums;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.system.localization;

namespace Tanks_Sever.tanks.users.garage.items
{
    public class Item
    {
        public string Id { get; private set; }
        public LocalizationHandler Description { get; private set; }
        public bool IsInventory { get; private set; }
        public int Index { get; private set; }
        public PropertyItem[] Properties { get; private set; }
        public ItemType ItemType { get; private set; }
        public int ModificationIndex { get; set; }
        public LocalizationHandler Name { get; private set; }
        public PropertyItem[] NextProperty { get; set; }
        public int NextPrice { get; set; }
        public int NextRankId { get; set; }
        public int Price { get; private set; }
        public int RankId { get; private set; }
        public ModificationInfo[] Modifications { get; private set; }
        public bool SpecialItem { get; private set; }
        public int Count { get; set; }

        public Item(string id, LocalizationHandler description, bool isInventory, int index, PropertyItem[] properties, ItemType itemType, int modificationIndex, LocalizationHandler name, PropertyItem[] nextProperty, int nextPrice, int nextRankId, int price, int rankId, ModificationInfo[] modifications, bool specialItem, int count)
        {
            Id = id;
            Description = description;
            IsInventory = isInventory;
            Index = index;
            Properties = properties;
            ItemType = itemType;
            ModificationIndex = modificationIndex;
            Name = name;
            NextProperty = nextProperty;
            NextPrice = nextPrice;
            NextRankId = nextRankId;
            Price = price;
            RankId = rankId;
            Modifications = modifications;
            SpecialItem = specialItem;
            Count = count;
        }

        public string GetId()
        {
            return StringUtils.ConcatStrings(Id, "_m", ModificationIndex.ToString());
        }

        public Item Clone()
        {
            return new Item(Id, Description, IsInventory, Index, Properties, ItemType, ModificationIndex, Name, NextProperty, NextPrice, NextRankId, Price, RankId, Modifications, SpecialItem, Count);
        }
    }
}
