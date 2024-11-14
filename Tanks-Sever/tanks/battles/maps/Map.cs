using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.bonuses;
using Tanks_Sever.tanks.battles.maps.themes;

namespace Tanks_Sever.tanks.battles.maps
{
    public class Map
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string SkyboxId { get; set; }
        public string ThemeId { get; set; }
        public MapTheme MapTheme { get; set; }
        public int MinRank { get; set; }
        public int MaxRank { get; set; }
        public int MaxPlayers { get; set; }
        public bool Tdm { get; set; } = false;
        public bool Ctf { get; set; } = false;
        public List<Vector3> SpawnPositionsDM { get; set; } = new List<Vector3>();
        public List<Vector3> SpawnPositionsBlue { get; set; } = new List<Vector3>();
        public List<Vector3> SpawnPositionsRed { get; set; } = new List<Vector3>();
        public List<bonuses.BonusRegion> CrystallsRegions { get; set; } = new List<bonuses.BonusRegion>();
        public List<bonuses.BonusRegion> GoldsRegions { get; set; } = new List<bonuses.BonusRegion>();
        public List<bonuses.BonusRegion> ArmorsRegions { get; set; } = new List<bonuses.BonusRegion>();
        public List<bonuses.BonusRegion> DamagesRegions { get; set; } = new List<bonuses.BonusRegion>();
        public List<bonuses.BonusRegion> HealthsRegions { get; set; } = new List<bonuses.BonusRegion>();
        public List<bonuses.BonusRegion> NitrosRegions { get; set; } = new List<bonuses.BonusRegion>();
        public int TotalCountDrops { get; set; }
        public Vector3 FlagRedPosition { get; set; }
        public Vector3 FlagBluePosition { get; set; }
        public string Md5Hash { get; set; }

        public Map()
        {
        }

        public Map(string name, string id, string skyboxId, List<Vector3> spawnPositionsDM, List<Vector3> spawnPositionsBlue, List<Vector3> spawnPositionsRed, List<BonusRegion> goldsRegions, List<BonusRegion> crystallsRegions, List<BonusRegion> dropRegions, int min, int max, int maxPlayers, bool tdm, bool ctf)
        {
            Name = name;
            Id = id;  
            SkyboxId = skyboxId;
            SpawnPositionsDM = spawnPositionsDM;
            SpawnPositionsBlue = spawnPositionsBlue;
            SpawnPositionsRed = spawnPositionsRed;
            GoldsRegions = goldsRegions;
            CrystallsRegions = crystallsRegions;
            MinRank = min;
            MaxRank = max;
            Tdm = tdm;
            Ctf = ctf;
            MaxPlayers = maxPlayers;
        }
    }

}
