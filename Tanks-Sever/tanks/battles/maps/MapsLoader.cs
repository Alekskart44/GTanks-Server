using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.maps.parser;
using Tanks_Sever.tanks.battles.maps.parser.map;
using Tanks_Sever.tanks.battles.maps.parser.map.spawn;
using Tanks_Sever.tanks.battles.maps.parser.map.bonus;
using Tanks_Sever.tanks.battles.maps.themes;

namespace Tanks_Sever.tanks.battles.maps
{
    public static class MapsLoader
    {
        public static Dictionary<string, Map> Maps = new Dictionary<string, Map>();
        private static List<IMapConfigItem> configItems = new List<IMapConfigItem>();
        private static Parser parser;

        public static void InitFactoryMaps()
        {
            Console.WriteLine("Maps Loader Factory initialized. Loading maps...");
            try
            {
                parser = new Parser();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            LoadConfig();
        }

        private static void LoadConfig()
        {
            try
            {
                var configJson = File.ReadAllText("maps/config.json");
                var jsonObject = JsonDocument.Parse(configJson).RootElement;
                var mapsArray = jsonObject.GetProperty("maps").EnumerateArray();

                foreach (var item in mapsArray)
                {
                    var id = item.GetProperty("id").GetString();
                    var name = item.GetProperty("name").GetString();
                    var skyboxId = item.GetProperty("skybox_id").GetString();
                    var minRank = int.Parse(item.GetProperty("min_rank").GetString());
                    var maxRank = int.Parse(item.GetProperty("max_rank").GetString());
                    var maxPlayers = int.Parse(item.GetProperty("max_players").GetString());
                    var tdm = item.GetProperty("tdm").GetBoolean();
                    var ctf = item.GetProperty("ctf").GetBoolean();
                    var ambientSoundId = item.TryGetProperty("ambient_sound_id", out var ambientSound) ? ambientSound.GetString() : null;
                    var gameModeId = item.TryGetProperty("gamemode_id", out var gameMode) ? gameMode.GetString() : null;
                    var themeId = item.TryGetProperty("theme_id", out var theme) ? theme.GetString() : null;

                    var configItem = !string.IsNullOrEmpty(ambientSoundId) && !string.IsNullOrEmpty(gameModeId)
                        ? new IMapConfigItem(id, name, skyboxId, minRank, maxRank, maxPlayers, tdm, ctf, ambientSoundId, gameModeId)
                        : new IMapConfigItem(id, name, skyboxId, minRank, maxRank, maxPlayers, tdm, ctf);

                    if (!string.IsNullOrEmpty(themeId))
                    {
                        configItem.ThemeName = themeId;
                    }

                    configItems.Add(configItem);
                }

                ParseMaps();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ParseMaps()
        {
            var mapFiles = Directory.GetFiles("maps", "*.xml");

            foreach (var file in mapFiles)
            {
                Parse(file);
            }

            Console.WriteLine("Loaded all maps!\n");
        }

        private static void Parse(string filePath)
        {
            Console.WriteLine("Loading " + Path.GetFileName(filePath) + "...");
            var mapId = Path.GetFileNameWithoutExtension(filePath);
            var configItem = GetMapItem(mapId);

            if (configItem == null) return;

            var map = new Map()
            {
                Name = configItem.Name,
                Id = configItem.Id,
                SkyboxId = configItem.SkyboxId,
                MinRank = configItem.MinRank,
                MaxRank = configItem.MaxRank,
                MaxPlayers = configItem.MaxPlayers,
                Tdm = configItem.Tdm,
                Ctf = configItem.Ctf,
                Md5Hash = CalculateMD5(filePath),
                MapTheme = !string.IsNullOrEmpty(configItem.AmbientSoundId) && !string.IsNullOrEmpty(configItem.GameMode)
                    ? MapThemeFactory.GetMapTheme(configItem.AmbientSoundId, configItem.GameMode)
                    : MapThemeFactory.GetDefaultMapTheme(),
                ThemeId = configItem.ThemeName
            };

            try
            {
                var parsedMap = parser.ParseMap(new FileInfo(filePath));
                PopulateMapSpawnPositions(map, parsedMap);
                PopulateMapBonuses(map, parsedMap);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

            Maps[map.Id] = map;
        }

        private static string CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static void PopulateMapSpawnPositions(Map map, parser.map.Map parsedMap)
        {
            foreach (var spawnPosition in parsedMap.GetSpawnPositions())
            {
                var spawnType = spawnPosition.GetSpawnPositionType();

                if (spawnType == SpawnPositionType.NONE)
                {
                    map.SpawnPositionsDM.Add(spawnPosition.GetVector3());
                }
                else if (spawnType == SpawnPositionType.RED)
                {
                    map.SpawnPositionsRed.Add(spawnPosition.GetVector3());
                }
                else if (spawnType == SpawnPositionType.BLUE)
                {
                    map.SpawnPositionsBlue.Add(spawnPosition.GetVector3());
                }
            }

            if (parsedMap.GetPositionBlueFlag() != null)
            {
                map.FlagBluePosition = parsedMap.GetPositionBlueFlag().ToVector3();
                map.FlagBluePosition.Z += 50.0f;
            }

            if (parsedMap.GetPositionRedFlag() != null)
            {
                map.FlagRedPosition = parsedMap.GetPositionRedFlag().ToVector3();
                map.FlagRedPosition.Z += 50.0f;
            }
        }

        private static void PopulateMapBonuses(Map map, parser.map.Map parsedMap)
        {
            foreach (var bonusRegion in parsedMap.GetBonusesRegion())
            {
                foreach (var type in bonusRegion.ToServerBonusRegion().Types)
                {
                    var serverBonusRegion = bonusRegion.ToServerBonusRegion();

                    if (type == BonusType.CRYSTALL.GetValue())
                    {
                        map.CrystallsRegions.Add(serverBonusRegion);
                    }
                    else if (type == BonusType.CRYSTALL_100.GetValue())
                    {
                        map.GoldsRegions.Add(serverBonusRegion);
                    }
                    else if (type == BonusType.ARMOR.GetValue())
                    {
                        map.ArmorsRegions.Add(serverBonusRegion);
                    }
                    else if (type == BonusType.DAMAGE.GetValue())
                    {
                        map.DamagesRegions.Add(serverBonusRegion);
                    }
                    else if (type== BonusType.HEAL.GetValue())
                    {
                        map.HealthsRegions.Add(serverBonusRegion);
                    }
                    else if (type == BonusType.NITRO.GetValue())
                    {
                        map.NitrosRegions.Add(serverBonusRegion);
                    }
                }
            }
        }

        private static IMapConfigItem GetMapItem(string id)
        {
            return configItems.FirstOrDefault(item => item.Id == id);
        }
    }
}