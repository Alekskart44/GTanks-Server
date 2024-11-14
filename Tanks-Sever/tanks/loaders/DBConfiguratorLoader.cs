using System;
using System.Collections.Generic;
using System.IO;

namespace Tanks_Sever.tanks.loaders
{
    public class DBConfiguratorLoader
    {
        private readonly string configFile;
        private Dictionary<string, string> parameters;

        public DBConfiguratorLoader(string configFile)
        {
            this.configFile = configFile;
            LoadParameters();
        }

        private void LoadParameters()
        {
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("Dababase Configuration not found:" + configFile);
            }

            var lines = File.ReadAllLines(configFile);
            parameters = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    parameters[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        public string GetConnectionString()
        {
            if (parameters.TryGetValue("server", out var server) && parameters.TryGetValue("port", out var port) && parameters.TryGetValue("username", out var username) && parameters.TryGetValue("password", out var password) && parameters.TryGetValue("database", out var database))
            {
                return "Server= " + server + ";Port=" + port + ";Database=" + database + ";User=" + username + ";Password=" + password + ";AllowZeroDateTime=True";
            }

            throw new ArgumentException("Not all connection parameters have been found.");
        }
    }
}
