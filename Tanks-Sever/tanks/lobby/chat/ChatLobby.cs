using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.services;

namespace Tanks_Sever.tanks.lobby.chat
{
    public class ChatLobby
    {
        private static readonly ChatLobby _instance = new ChatLobby();
        private readonly List<ChatMessage> _chatMessages = new List<ChatMessage>();

        private LobbysServices _lobbyServices = LobbysServices.Instance;

        public static ChatLobby Instance => _instance;

        private ChatLobby()
        {
        }

        public void AddMessage(ChatMessage msg)
        {
            CheckSyntax(msg);
        }

        private void CheckSyntax(ChatMessage msg)
        {
            msg.Message = msg.Message.Trim();
            msg.Message = GetNormalMessage(msg.Message.Trim());
            if (_chatMessages.Count >= 50)
            {
                _chatMessages.RemoveAt(0);
            }

            _chatMessages.Add(msg);
            SendMessageToAll(msg);
        }

        public void Clear()
        {
            _lobbyServices.SendCommandToAllUsers(CommandType.LOBBY_CHAT, UserLocation.ALL, "clear_all");
            _chatMessages.Clear();
            SendSystemMessageToAll("Чат очищен", false);
        }

        public void SendSystemMessageToAll(string msg, bool yellow)
        {
            var sysMsg = new ChatMessage(null, msg, false, null, yellow, null)
            {
                System = true
            };
            _chatMessages.Add(sysMsg);
            if (_chatMessages.Count >= 50)
            {
                _chatMessages.RemoveAt(0);
            }

            _lobbyServices.SendCommandToAllUsersBesides(CommandType.LOBBY_CHAT, UserLocation.BATTLE, "system", msg.Trim());
        }

        public void SendMessageToAll(ChatMessage msg)
        {
            _lobbyServices.SendCommandToAllUsersBesides(CommandType.LOBBY_CHAT, UserLocation.BATTLE, JSONUtils.parseChatLobbyMessage(msg));
        }

        public string GetNormalMessage(string src)
        {
            var str = new System.Text.StringBuilder();
            char[] mass = src.ToCharArray();

            for (int i = 0; i < mass.Length; i++)
            {
                if (mass[i] == ' ')
                {
                    if (i + 1 < mass.Length && mass[i] != mass[i + 1])
                    {
                        str.Append(" ");
                    }
                }
                else
                {
                    str.Append(mass[i]);
                }
            }
            return str.ToString();
        }

        public IEnumerable<ChatMessage> GetMessages()
        {
            return _chatMessages;
        }
    }
}