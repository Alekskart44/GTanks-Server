using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.main.procotol;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.system.localization;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.lobby.chat;
using Tanks_Sever.tanks.services;
using Repository;
using Tanks_Sever.tanks.users.garage;

namespace Tanks_Sever.tanks.auth
{
    public class Auth
    {

        private ProtocolTransfer transfer;
        private LocalizationHandler.Localization localization;
        private ChatLobby chatLobby = ChatLobby.Instance;
        private AutoEntryServices autoEntryServices = AutoEntryServices.Instance;

        public Auth(ProtocolTransfer transfer)
        {
            this.transfer = transfer;
        }

        public void ExecuteCommand(Command command) {
            string nickname;
            string password;
            User newUser;

            switch (command.Type) {
                case CommandType.AUTH:
                    nickname = command.Args[0];
                    password = command.Args[1];

                    if (nickname.Length > 50)
                    {
                        nickname = null;
                        return;
                    }
                    if (password.Length > 50)
                    {
                        password = null;
                        return;
                    }
                    newUser = GenericManager.repository.SingleByNickname<User>(nickname);
                    if (newUser == null) 
                    {
                        transfer.Send(CommandType.AUTH, "not_exist");
                        return;
                    }

                    if (!newUser.GetPassword().Equals(password))
                    {
                        Console.WriteLine($"The user " + newUser.GetNickname() + " has not been logged. Password denied.");
                        transfer.Send(CommandType.AUTH, "denied");
                        return;
                    }
                    OnPasswordAccept(newUser);
                    break;
                case CommandType.REGISTRATION:
                    if (command.Args[0].Equals("check_name"))
                    {
                        nickname = command.Args[1];
                        if (nickname.Length > 50)
                        {
                            nickname = null;
                            return;
                        }

                        Boolean callsignExist = GenericManager.repository.SingleByNickname<User>(nickname) != null;
                        Boolean callsignNormal = this.callsignNormal(nickname);
                        transfer.Send(CommandType.REGISTRATION, "check_name_result", !callsignExist && callsignNormal ? "not_exist" : "nickname_exist");
                    } 
                    else 
                    {

                        nickname = command.Args[0];
                        password = command.Args[1];

                        if (nickname.Length > 50)
                        {
                            nickname = null;
                            return;
                        }
                        if (password.Length > 50)
                        {
                            password = null;
                            return;
                        }

                        if (GenericManager.repository.SingleByNickname<User>(nickname) != null) 
                        {
                            transfer.Send(CommandType.REGISTRATION, "nickname_exist");
                            return;
                        }

                        if (callsignNormal(nickname))
                        {
                            newUser = new User(nickname, password);
                            newUser.setLastIP(transfer.GetIP());
                            GenericManager.repository.Add(newUser);
                            transfer.Send(CommandType.REGISTRATION, "info_done");
                        }
                        else
                        {
                            transfer.CloseConnection();
                        }

                    }
                    break;
                case CommandType.SYSTEM:
                    nickname = command.Args[0];
                    if (nickname.Equals("init_location")) 
                    {
                        localization = (LocalizationHandler.Localization)Enum.Parse(typeof(LocalizationHandler.Localization), command.Args[1].ToUpper());
                    }

                    if (nickname.Equals("c01")) 
                    {
                        transfer.CloseConnection();
                    }
                    break;
            }
        }

        private bool callsignNormal(String nick)
        {
            Regex pattern = new Regex("^[a-zA-Z]\\w{3,14}$");
            return pattern.IsMatch(nick);
        }

        private void OnPasswordAccept(User user)
        {

            if (user.Session != null)
            {
                transfer.CloseConnection();
                return;
            }

            user.getAntiCheatData().ip = transfer.GetIP();
            user.Session = new Session(transfer);

            user.SetGarage(GenericManager.repository.SingleByUserId<Garage>(user.GetNickname()));
            user.GetGarage().UnparseJSONData(); 

            Console.WriteLine("The user " + user.GetNickname() + " has been logged. Password accept.");
            
            transfer.lobby = new LobbyManager(transfer, user);
            if (localization == null)
            {
                localization = LocalizationHandler.Localization.EN;
            }
            user.SetLocalization(localization);

            transfer.Send(CommandType.AUTH, "accept");
            transfer.Send(CommandType.LOBBY, "init_panel", JSONUtils.parseUser(user));
            transfer.Send(CommandType.LOBBY, "update_rang_progress", RankUtils.GetUpdateNumber(user.GetScore()).ToString());
            if (!autoEntryServices.NeedEnterToBattle(user))
            {
                transfer.Send(CommandType.GARAGE, "init_garage_items", JSONUtils.ParseGarageUser(user).Trim());
                transfer.Send(CommandType.GARAGE, "init_market", JSONUtils.ParseMarketItems(user));
                transfer.Send(CommandType.LOBBY_CHAT, "init_chat");
                transfer.Send(CommandType.LOBBY_CHAT, "init_messages", JSONUtils.ParseChatLobbyMessages(chatLobby.GetMessages()));
            }
            else 
            {
                transfer.Send(CommandType.LOBBY, "init_battlecontroller");
                autoEntryServices.PrepareToEnter(transfer.lobby);   
            }


            user.setLastIP(user.getAntiCheatData().ip);
            GenericManager.repository.Update(user);
        }

    }
}
