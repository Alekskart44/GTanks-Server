using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.loaders;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.users;

namespace Tanks_Sever.tanks.battles.spectator
{
    public class SpectatorController
    {
        private static readonly string NULL_JSON_STRING = "{}";
        private LobbyManager lobby;
        private BattlefieldModel bfModel;
        private SpectatorModel specModel;
        private bool inited;

        public SpectatorController(LobbyManager lobby, BattlefieldModel bfModel, SpectatorModel specModel)
        {
            this.lobby = lobby;
            this.bfModel = bfModel;
            this.specModel = specModel;
        }

        public void ExecuteCommand(Command cmd)
        {
            switch (cmd.Type)
            {
                case CommandType.BATTLE:
                    switch (cmd.Args[0])
                    {
                        case "spectator_user_init":
                            InitUser();
                            break;
                        case "i_exit_from_battle":
                            lobby.OnExitFromBattle();
                            break;
                        case "chat":
                            specModel.GetChatModel().OnMessage(cmd.Args[1], this);
                            break;
                    }
                    break;

                default:
                    Console.WriteLine("[ExecuteCommand(Command)::SpectatorController] : non-battle command \"" + cmd);
                    break;
            }
        }

        private void InitUser()
        {
            try
            {
                inited = true;
                SendShotsData();

                if (bfModel.battleInfo.BattleType == "CTF")
                {
                    SendCommand(CommandType.BATTLE, "init_ctf_model", JSONUtils.ParseCTFModelData(bfModel));
                }

                SendCommand(CommandType.BATTLE, "init_gui_model", JSONUtils.ParseBattleData(bfModel));
                SendCommand(CommandType.BATTLE, "init_inventory", NULL_JSON_STRING);
                bfModel.battleMinesModel.InitModel(this);
                bfModel.battleMinesModel.SendMines(this);
                bfModel.effectsModel.SendInitDataSpectator(this);
            }
            catch (Exception)
            {
                lobby.Kick();
            }
        }

        public string GetId()
        {
            return lobby.GetLocalUser().GetNickname();
        }

        public User GetUser()
        {
            return lobby.GetLocalUser();
        }

        public void SendCommand(CommandType type, params string[] args)
        {
            if (inited)
            {
                lobby.transfer.Send(type, args);
            }
        }

        private void SendShotsData()
        {
            SendCommand(CommandType.BATTLE, "init_shots_data", WeaponsFactory.GetJSONList());
        }

        public void OnDisconnect()
        {
            bfModel.spectatorModel.RemoveSpectator(this);
        }
    }
}