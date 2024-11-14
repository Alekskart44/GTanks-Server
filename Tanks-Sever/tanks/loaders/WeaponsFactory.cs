using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.battles.tanks.weapons;
using Tanks_Sever.tanks.battles.tanks.weapons.data;
using Tanks_Sever.tanks.battles.tanks.weapons.turrets.smoky;

namespace Tanks_Sever.tanks.loaders
{
    public class WeaponsFactory
    {
        private static Dictionary<string, IEntity> weapons = new Dictionary<string, IEntity>();
        private static Dictionary<string, WeaponWeakeningData> wwd = new Dictionary<string, WeaponWeakeningData>();
        private static string jsonListWeapons;

        public static IWeapon GetWeapon(string turretId, BattlefieldPlayerController tank, BattlefieldModel battle)
        {
            string turret = turretId.Split("_m")[0];
            IWeapon weapon = null;

            switch (turret)
            {
                case "smoky":
                    weapon = new SmokyModel((SmokyEntity)GetEntity(turretId), GetWwd(turretId), battle, tank);
                    return weapon;
            }

            return weapon;
        }

        public static void Init(string path2config)
        {
            weapons.Clear();
            Console.WriteLine("Weapons Factory inited. Loading weapons...");

            try
            {
                var folder = new DirectoryInfo(path2config);
                foreach (var config in folder.GetFiles("*.cfg"))
                {
                    Console.WriteLine($"Loading {config.Name}...");
                    Parse(config.FullName);
                }

                jsonListWeapons = JSONUtils.ParseWeapons(GetEntities(), wwd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Loading entity weapons failed. {ex.Message}");
            }
        }

        private static void Parse(string jsonFilePath)
        {
            try
            {
                string json = File.ReadAllText(jsonFilePath);
                var jsonObject = JsonSerializer.Deserialize<JsonDocument>(json);

                string type = jsonObject.RootElement.GetProperty("type").GetString();
                var paramsArray = jsonObject.RootElement.GetProperty("params").EnumerateArray();

                foreach (var item in paramsArray)
                {
                    string modification = item.GetProperty("modification").GetString();
                    string id = $"{type}_{modification}";

                    var shotData = new ShotData(
                        id,
                        GetDouble(item.GetProperty("autoAimingAngleDown")),
                        GetDouble(item.GetProperty("autoAimingAngleUp")),
                        item.GetProperty("numRaysDown").GetInt32(),
                        item.GetProperty("numRaysUp").GetInt32(),
                        item.GetProperty("reloadMsec").GetInt32(),
                        (float)item.GetProperty("impactCoeff").GetDouble(),
                        (float)item.GetProperty("kickback").GetDouble(),
                        (float)item.GetProperty("turretRotationAccel").GetDouble(),
                        (float)item.GetProperty("turretRotationSpeed").GetDouble()
                    );

                    IEntity entity = null;

                    switch (type)
                    {
                        case "smoky":
                            var wwdData = new WeaponWeakeningData(
                                item.GetProperty("max_damage_radius").GetDouble(),
                                item.GetProperty("min_damage_percent").GetDouble(),
                                item.GetProperty("min_damage_radius").GetDouble()
                            );

                            entity = new SmokyEntity(
                                shotData,
                                (float)item.GetProperty("min_damage").GetDouble(),
                                (float)item.GetProperty("max_damage").GetDouble()
                            );

                            wwd[id] = wwdData;
                            break;
                    }

                    if (entity != null)
                    {
                        weapons[id] = entity;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse JSON: {jsonFilePath}. Error: {ex.Message}", ex);
            }
        }

        public static WeaponWeakeningData GetWwd(string id)
        {
            return wwd.TryGetValue(id, out var weakeningData) ? weakeningData : null;
        }

        public static IEntity GetEntity(string id)
        {
            return weapons.TryGetValue(id, out var entity) ? entity : null;
        }

        public static string GetId(IEntity entity)
        {
            return weapons.FirstOrDefault(x => x.Value.Equals(entity)).Key;
        }

        private static double GetDouble(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetDouble(out var doubleValue))
                {
                    return doubleValue;
                }
                if (element.TryGetInt64(out var longValue))
                {
                    return longValue;
                }
            }
            throw new FormatException("Invalid number format.");
        }

        public static ICollection<IEntity> GetEntities()
        {
            return weapons.Values;
        }

        public static string GetJSONList()
        {
            return jsonListWeapons;
        }
    }
}