namespace Tanks_Sever.tanks.main.procotol.commands
{
    public enum CommandType
    {
        AUTH,
        REGISTRATION,
        GARAGE,
        CHAT,
        LOBBY,
        LOBBY_CHAT,
        BATTLE,
        PING,
        UNKNOWN,
        HTTP,
        SYSTEM
    }

    public static class CommandTypeExtensions
    {
        public static string ToStringValue(this CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.AUTH:
                    return "auth";
                case CommandType.REGISTRATION:
                    return "registration";
                case CommandType.GARAGE:
                    return "garage";
                case CommandType.CHAT:
                    return "chat";
                case CommandType.LOBBY:
                    return "lobby";
                case CommandType.LOBBY_CHAT:
                    return "lobby_chat";
                case CommandType.BATTLE:
                    return "battle";
                case CommandType.PING:
                    return "ping";
                case CommandType.UNKNOWN:
                    return "UNKNOWN";
                case CommandType.HTTP:
                    return "http";
                case CommandType.SYSTEM:
                    return "system";
                default:
                    return string.Empty;
            }
        }
    }
}
