namespace Tanks_Sever.tanks.battles.maps
{
    public class IMapConfigItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SkyboxId { get; set; }
        public string AmbientSoundId { get; set; }
        public string GameMode { get; set; }
        public string ThemeName { get; set; }
        public int MinRank { get; set; }
        public int MaxRank { get; set; }
        public int MaxPlayers { get; set; }
        public bool Tdm { get; set; } = false;
        public bool Ctf { get; set; } = false;

        public IMapConfigItem(string id, string name, string skyboxId, int minRank, int maxRank, int maxPlayers, bool tdm, bool ctf)
        {
            Id = id;
            Name = name;
            SkyboxId = skyboxId;
            MinRank = minRank;
            MaxRank = maxRank;
            MaxPlayers = maxPlayers;
            Tdm = tdm;
            Ctf = ctf;
        }

        public IMapConfigItem(string id, string name, string skyboxId, int minRank, int maxRank, int maxPlayers, bool tdm, bool ctf, string soundId, string gameModeId)
        {
            Id = id;
            Name = name;
            SkyboxId = skyboxId;
            MinRank = minRank;
            MaxRank = maxRank;
            MaxPlayers = maxPlayers;
            Tdm = tdm;
            Ctf = ctf;
            AmbientSoundId = soundId;
            GameMode = gameModeId;
        }
    }
}
