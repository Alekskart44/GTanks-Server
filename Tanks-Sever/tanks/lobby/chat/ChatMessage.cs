using Tanks_Sever.tanks.users;

namespace Tanks_Sever.tanks.lobby.chat
{
    public class ChatMessage
    {
        public User User { get; set; }
        public string Message { get; set; }
        public bool Addressed { get; set; }
        public bool System { get; set; }
        public User UserTo { get; set; }
        public LobbyManager LocalLobby { get; set; }
        public bool YellowMessage { get; set; }

        public ChatMessage(User user, string message, bool addressed, User userTo, bool yellowMessage, LobbyManager localLobby)
        {
            User = user;
            Message = message;
            Addressed = addressed;
            UserTo = userTo;
            YellowMessage = yellowMessage;
            LocalLobby = localLobby;
        }

        public override string ToString()
        {
            return (System ? "SYSTEM: " : User.Nickname + ": ") + (Addressed ? "->" + UserTo.Nickname : "") + Message;
        }
    }
}