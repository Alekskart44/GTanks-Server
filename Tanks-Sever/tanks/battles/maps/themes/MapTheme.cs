namespace Tanks_Sever.tanks.battles.maps.themes
{
    public class MapTheme
    {
        private string gameModeId;
        private string ambientSoundId;

        public string GetAmbientSoundId()
        {
            return ambientSoundId;
        }

        public void SetAmbientSoundId(string ambientSoundId)
        {
            this.ambientSoundId = ambientSoundId;
        }

        public string GetGameModeId()
        {
            return gameModeId;
        }

        public void SetGameModeId(string gameModeId)
        {
            this.gameModeId = gameModeId;
        }
    }
}
