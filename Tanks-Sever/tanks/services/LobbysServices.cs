using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.services
{
    public class LobbysServices
    {
        private static readonly LobbysServices _instance = new LobbysServices();
        public Dictionary<string, LobbyManager> Lobbys { get; private set; } = new Dictionary<string, LobbyManager>();

        public static LobbysServices Instance => _instance;

        private LobbysServices() { }

        public void AddLobby(LobbyManager lobby)
        {
            if (!Lobbys.ContainsKey(lobby.GetLocalUser().Nickname))
            {
                Lobbys[lobby.GetLocalUser().Nickname] = lobby;
            }
        }

        public void RemoveLobby(LobbyManager lobby)
        {
            Lobbys.Remove(lobby.GetLocalUser().Nickname);
        }

        public bool ContainsLobby(LobbyManager lobby)
        {
            return Lobbys.ContainsKey(lobby.GetLocalUser().Nickname);
        }

        public LobbyManager GetLobbyByUser(User user)
        {
            Lobbys.TryGetValue(user.Nickname, out var lobby);
            return lobby;
        }

        public LobbyManager GetLobbyByNick(string nick)
        {
            Lobbys.TryGetValue(nick, out var lobby);
            return lobby;
        }

        public void SendCommandToAllUsers(CommandType type, UserLocation onlyFor, params string[] args)
        {
            try
            {
                foreach (var lobby in Lobbys.Values)
                {
                    if (lobby != null && (onlyFor == UserLocation.ALL || lobby.GetLocalUser().getUserLocation() == onlyFor))
                    {
                        lobby.transfer.Send(type, args);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void SendCommandToAllUsersBesides(CommandType type, UserLocation besides, params string[] args)
        {
            try
            {
                foreach (var lobby in Lobbys.Values)
                {
                    if (lobby != null && lobby.GetLocalUser().getUserLocation() != besides)
                    {
                        lobby.transfer.Send(type, args);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}