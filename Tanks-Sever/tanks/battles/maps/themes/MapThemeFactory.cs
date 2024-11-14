using Tanks_Sever.tanks.battles.maps.themes;

namespace Tanks_Sever.tanks.battles.maps.themes
{
    public static class MapThemeFactory
    {
        public static MapTheme GetDefaultMapTheme()
        {
            var theme = new MapTheme();
            theme.SetAmbientSoundId("default_ambient_sound");
            theme.SetGameModeId("default");
            return theme;
        }

        public static MapTheme GetMapTheme(string soundId, string gameMode)
        {
            var theme = new MapTheme();
            theme.SetAmbientSoundId(soundId);
            theme.SetGameModeId(gameMode);
            return theme;
        }
    }
}
