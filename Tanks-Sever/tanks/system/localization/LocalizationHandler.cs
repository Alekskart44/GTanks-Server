namespace Tanks_Sever.tanks.system.localization
{
    public class LocalizationHandler
    {
        public enum Localization
        {
            RU,
            EN
        }

        private Dictionary<Localization, string> localizedMap = new Dictionary<Localization, string>();

        public LocalizationHandler(string ruVersion, string enVersion)
        {
            localizedMap[Localization.RU] = ruVersion;
            localizedMap[Localization.EN] = enVersion;
        }

        public string GetLocalizedString(Localization loc)
        {
            return localizedMap.TryGetValue(loc, out string value) ? value : "null";
        }

        public static LocalizationHandler RegisterString(string ruVersion, string enVersion)
        {
            return new LocalizationHandler(ruVersion, enVersion);
        }
    }
}
