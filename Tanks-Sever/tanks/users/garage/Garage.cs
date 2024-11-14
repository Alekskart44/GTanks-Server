using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tanks_Sever.tanks.users.garage.items;
using Tanks_Sever.tanks.users.garage.enums;
using System.Text.Json.Nodes;

namespace Tanks_Sever.tanks.users.garage
{
    [Table("garages")]
    public class Garage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("uid", TypeName = "bigint")]
        public long Id { get; set; }

        [Column("turrets", TypeName = "text")]
        public string JsonTurrets { get; set; }

        [Column("hulls", TypeName = "text")]
        public string JsonHulls { get; set; }

        [Column("colormaps", TypeName = "text")]
        public string JsonColormaps { get; set; }

        [Column("inventory", TypeName = "text")]
        public string JsonInventory { get; set; }

        [Column("userid", TypeName = "varchar(255)")]
        public string UserId { get; set; }

        [NotMapped]
        public List<Item> Items { get; set; } = new List<Item>();

        [NotMapped]
        public Item MountTurret { get; set; }

        [NotMapped]
        public Item MountHull { get; set; }

        [NotMapped]
        public Item MountColormap { get; set; }

        public Garage()
        {
            Items.Add((Item)GarageItemsLoader.Items["smoky"].Clone());
            Items.Add((Item)GarageItemsLoader.Items["wasp"].Clone());
            Items.Add((Item)GarageItemsLoader.Items["green"].Clone());
            Items.Add((Item)GarageItemsLoader.Items["holiday"].Clone());
            MountItem("wasp_m0");
            MountItem("smoky_m0");
            MountItem("green_m0");
        }

        public bool ContainsItem(string id)
        {
            return Items.Any(item => item.Id.Equals(id));
        }

        public Item GetItemById(string id)
        {
            return Items.FirstOrDefault(item => item.Id.Equals(id));
        }

        public bool MountItem(string id)
        {
            Item item = GetItemById(id.Substring(0, id.Length - 3));
            if (item != null && int.Parse(id.Substring(id.Length - 1, 1)) == item.ModificationIndex)
            {
                if (item.ItemType == ItemType.WEAPON)
                {
                    MountTurret = item;
                    return true;
                }
                if (item.ItemType == ItemType.ARMOR)
                {
                    MountHull = item;
                    return true;
                }
                if (item.ItemType == ItemType.COLOR)
                {
                    MountColormap = item;
                    return true;
                }
            }
            return false;
        }

        public bool UpdateItem(string id)
        {
            Item item = GetItemById(id.Substring(0, id.Length - 3));
            int modificationID = int.Parse(id.Substring(id.Length - 1));
            if (modificationID < 3 && item.ModificationIndex == modificationID)
            {
                item.ModificationIndex++;
                item.NextPrice = item.Modifications[item.ModificationIndex + 1 != 4 ? item.ModificationIndex + 1 : item.ModificationIndex].Price;
                item.NextProperty = item.Modifications[item.ModificationIndex + 1 != 4 ? item.ModificationIndex + 1 : item.ModificationIndex].Propertys;
                item.NextRankId = item.Modifications[item.ModificationIndex + 1 != 4 ? item.ModificationIndex + 1 : item.ModificationIndex].Rank;
                ReplaceItems(GetItemById(id.Substring(0, id.Length - 3)), item);
                return true;
            }
            return false;
        }

        public Item BuyItem(string id, int count)
        {
            id = id.Substring(0, id.Length - 3);
            Item temp = (Item)GarageItemsLoader.Items[id];
            if (temp.SpecialItem)
            {
                return null;
            }

            Item item = (Item)temp.Clone();
            if (!Items.Contains(GetItemById(id)))
            {
                if (item.ItemType == ItemType.INVENTORY)
                {
                    item.Count += count;
                }
                Items.Add(item);
                return item;
            }
            else if (item.ItemType == ItemType.INVENTORY)
            {
                Item fromUser = GetItemById(id);
                fromUser.Count += count;
                return fromUser;
            }
            return null;
        }

        private void ReplaceItems(Item oldItem, Item newItem)
        {
            if (Items.Contains(oldItem))
            {
                Items[Items.IndexOf(oldItem)] = newItem;
            }
        }

        public List<Item> GetInventoryItems()
        {
            return Items.Where(item => item.ItemType == ItemType.INVENTORY).ToList();
        }

        public void ParseJSONData()
        {
            var hulls = new JsonObject();
            var hullArray = new JsonArray();
            var colormaps = new JsonObject();
            var colorArray = new JsonArray();
            var turrets = new JsonObject();
            var turretArray = new JsonArray();
            var inventoryItems = new JsonObject();
            var inventoryArray = new JsonArray();

            foreach (var item in Items)
            {
                JsonObject inventory = new JsonObject();

                if (item.ItemType == ItemType.ARMOR)
                {
                    inventory["id"] = item.Id;
                    inventory["modification"] = item.ModificationIndex;
                    inventory["mounted"] = item == MountHull;
                    hullArray.Add(inventory);
                }
                else if (item.ItemType == ItemType.COLOR)
                {
                    inventory["id"] = item.Id;
                    inventory["modification"] = item.ModificationIndex;
                    inventory["mounted"] = item == MountColormap;
                    colorArray.Add(inventory);
                }
                else if (item.ItemType == ItemType.WEAPON)
                {
                    inventory["id"] = item.Id;
                    inventory["modification"] = item.ModificationIndex;
                    inventory["mounted"] = item == MountTurret;
                    turretArray.Add(inventory);
                }
                else if (item.ItemType == ItemType.INVENTORY)
                {
                    inventory["id"] = item.Id;
                    inventory["count"] = item.Count;
                    inventoryArray.Add(inventory);
                }
            }

            hulls["hulls"] = hullArray;
            colormaps["colormaps"] = colorArray;
            turrets["turrets"] = turretArray;
            inventoryItems["inventory"] = inventoryArray;

            JsonColormaps = JsonSerializer.Serialize(colormaps);
            JsonHulls = JsonSerializer.Serialize(hulls);
            JsonTurrets = JsonSerializer.Serialize(turrets);
            JsonInventory = JsonSerializer.Serialize(inventoryItems);
        }

        public void UnparseJSONData()
        {
            Items.Clear();

            using (var doc = JsonDocument.Parse(JsonTurrets))
            {
                var turrets = doc.RootElement.GetProperty("turrets");
                foreach (var _item in turrets.EnumerateArray())
                {
                    var item = (Item)GarageItemsLoader.Items[_item.GetProperty("id").GetString()].Clone();
                    item.ModificationIndex = _item.GetProperty("modification").GetInt32();
                    item.NextRankId = item.Modifications[item.ModificationIndex == 3 ? 3 : item.ModificationIndex + 1].Rank;
                    item.NextPrice = item.Modifications[item.ModificationIndex == 3 ? 3 : item.ModificationIndex + 1].Price;
                    Items.Add(item);
                    if (_item.GetProperty("mounted").GetBoolean())
                    {
                        MountTurret = item;
                    }
                }
            }

            using (var doc = JsonDocument.Parse(JsonColormaps))
            {
                var colormaps = doc.RootElement.GetProperty("colormaps");
                foreach (var _item in colormaps.EnumerateArray())
                {
                    var item = (Item)GarageItemsLoader.Items[_item.GetProperty("id").GetString()].Clone();
                    item.ModificationIndex = _item.GetProperty("modification").GetInt32();
                    Items.Add(item);
                    if (_item.GetProperty("mounted").GetBoolean())
                    {
                        MountColormap = item;
                    }
                }
            }

            using (var doc = JsonDocument.Parse(JsonHulls))
            {
                var hulls = doc.RootElement.GetProperty("hulls");
                foreach (var _item in hulls.EnumerateArray())
                {
                    var item = (Item)GarageItemsLoader.Items[_item.GetProperty("id").GetString()].Clone();
                    item.ModificationIndex = _item.GetProperty("modification").GetInt32();
                    item.NextRankId = item.Modifications[item.ModificationIndex == 3 ? 3 : item.ModificationIndex + 1].Rank;
                    item.NextPrice = item.Modifications[item.ModificationIndex == 3 ? 3 : item.ModificationIndex + 1].Price;
                    Items.Add(item);
                    if (_item.GetProperty("mounted").GetBoolean())
                    {
                        MountHull = item;
                    }
                }
            }

            if (!string.IsNullOrEmpty(JsonInventory))
            {
                using (var doc = JsonDocument.Parse(JsonInventory))
                {
                    var inventory = doc.RootElement.GetProperty("inventory");
                    foreach (var _item in inventory.EnumerateArray())
                    {
                        var item = (Item)GarageItemsLoader.Items[_item.GetProperty("id").GetString()].Clone();
                        item.Count = _item.GetProperty("count").GetInt32();
                        Items.Add(item);
                    }
                }
            }
        }

        public string GetUserId()
        {
            return UserId;
        }

        public void SetUserId(string userId)
        {
            UserId = userId;
        }
    }
}
