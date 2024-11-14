using System;
using System.IO;

namespace Tanks_Sever.tanks.loaders
{
    internal static class ConfiguratorLoader
    {
        public static int Port { get; private set; }

        public static void Init(String config) {
            var conf = config;
            if (File.Exists(conf))
            {
                string[] lines = File.ReadAllLines(conf);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2 && parts[0].Trim().ToLower() == "port")
                    {
                        if (int.TryParse(parts[1].Trim(), out int port))
                        {
                            Port = port;
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException(conf + " file was not found!");
            }
        }

    }
}
