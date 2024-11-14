using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.hulls;

namespace Tanks_Sever.tanks.loaders
{
    public class HullsFactory
    {
        private static Dictionary<string, Hull> hulls = new Dictionary<string, Hull>();

        public static void Init(string pathToConfigs)
        {
            hulls.Clear();

            try
            {
                var files = Directory.GetFiles(pathToConfigs);
                foreach (var config in files)
                {
                    Parse(config);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Parse(string configPath)
        {
            try
            {
                string jsonContent = File.ReadAllText(configPath);
                JsonObject jsonObject = JsonNode.Parse(jsonContent).AsObject();
                string type = jsonObject["type"].GetValue<string>();
                JsonArray modifications = jsonObject["modifications"].AsArray();

                foreach (var obj in modifications)
                {
                    var jt = obj.AsObject();
                    Hull hull = new Hull(
                        (float)jt["mass"].GetValue<double>(),
                        (float)jt["power"].GetValue<double>(),
                        (float)jt["speed"].GetValue<double>(),
                        (float)jt["turn_speed"].GetValue<double>(),
                        (float)jt["hp"].GetValue<long>()
                    );
                    string key = $"{type}_{jt["modification"].GetValue<string>()}";
                    hulls[key] = hull;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing file {configPath}: {ex.Message}");
            }
        }

        public static Hull GetHull(string id)
        {
            if (hulls.TryGetValue(id, out Hull hull))
            {
                return hull;
            }
            else
            {
                Console.WriteLine($"Hull with id {id} is null!");
                return null;
            }
        }
    }
}