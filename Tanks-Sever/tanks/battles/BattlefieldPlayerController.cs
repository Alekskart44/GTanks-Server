using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks;
using Tanks_Sever.tanks.battles.tanks.colormaps;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.loaders;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.users.garage;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.users.locations;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.battles.tanks.statistic;
using Tanks_Sever.tanks.battles.effect.controller;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.ctf.flags;
using System.Text.Json;

namespace Tanks_Sever.tanks.battles
{
    public class BattlefieldPlayerController
    {
        public LobbyManager parentLobby;
        public BattlefieldModel battleModel;
        public Tank tank;
        public PlayerStatistic statistic;
        private LobbysServices lobbysServices = LobbysServices.Instance;
        private AutoEntryServices autoEntryServices = AutoEntryServices.Instance;
        public string playerTeamType;
        public FlagServer flag;
        public InventoryController inventory;
        public bool userInited = false;

        public BattlefieldPlayerController(LobbyManager parent, BattlefieldModel battle, string playerTeamType)
        {
            parentLobby = parent;
            battleModel = battle;
            this.playerTeamType = playerTeamType;
            tank = new Tank(null);
            tank.SetHull(HullsFactory.GetHull(GetGarage().MountHull.GetId()));
            tank.SetWeapon(WeaponsFactory.GetWeapon(GetGarage().MountTurret.GetId(), this, battle));
            tank.SetColormap(ColormapsFactory.GetColormap(GetGarage().MountColormap.GetId()));

            statistic = new PlayerStatistic(0, 0, 0);
            inventory = new InventoryController(this);

            battleModel.AddPlayer(this);
            SendShotsData();
        }

        public User GetUser()
        {
            return parentLobby.GetLocalUser();
        }

        public Garage GetGarage() 
        {
            return parentLobby.GetLocalUser().GetGarage();
        }

        public void ExecuteCommand(Command command)
        {
            switch (command.Type)
            {
                case CommandType.BATTLE:
                    if (command.Args[0].Equals("get_init_data_local_tank"))
                    {
                        battleModel.InitLocalTank(this);
                    }
                    else if (command.Args[0].Equals("activate_tank"))
                    {
                        battleModel.ActivateTank(this);
                    }
                    else if (command.Args[0].Equals("suicide"))
                    {
                        battleModel.RespawnPlayer(this, true);
                    }
                    else if (command.Args[0].Equals("move"))
                    {
                        ParseAndMove(command.Args);
                    }
                    else if (command.Args[0].Equals("i_exit_from_battle"))
                    {
                        parentLobby.OnExitFromBattle();
                    }
                    else if (command.Args[0].Equals("exit_from_statistic"))
                    {
                        parentLobby.OnExitFromStatistic();
                    }
                    else if (command.Args[0].Equals("start_fire"))
                    {
                        if (tank.State.Equals("active"))
                        {
                            tank.GetWeapon().StartFire(command.Args.Length >= 2 ? command.Args[1] : "");     
                        }
                    }
                    else if (command.Args[0].Equals("fire"))
                    {
                        if (tank.State.Equals("active"))
                        {
                            tank.GetWeapon().Fire(command.Args[1]);
                        }
                    }
                    else if (command.Args[0].Equals("stop_fire"))
                    {
                        tank.GetWeapon().StopFire();
                    }
                    else if (command.Args[0].Equals("attempt_to_take_flag"))
                    {
                        battleModel.ctfModel.AttemptToTakeFlag(this, command.Args[1]);
                    }
                    else if (command.Args[0].Equals("flag_drop"))
                    {
                        ParseAndDropFlag(command.Args[1]);
                    }
                    else if (command.Args[0].Equals("activate_item"))
                    {
                        Vector3 _tankPos;
                        try
                        {
                            _tankPos = new Vector3(float.Parse(command.Args[2], CultureInfo.InvariantCulture), float.Parse(command.Args[3], CultureInfo.InvariantCulture), float.Parse(command.Args[4], CultureInfo.InvariantCulture));
                        }
                        catch (Exception ex)
                        {
                            _tankPos = new Vector3(0.0F, 0.0F, 0.0F);
                        }
                        inventory.ActivateItem(command.Args[1], _tankPos);
                    }
                    else if (command.Args[0].Equals("mine_hit"))
                    {
                        battleModel.battleMinesModel.HitMine(this, command.Args[1]);
                    }
                    break;
            }
        }

        private void ParseAndDropFlag(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;
                    float x = root.GetProperty("x").GetSingle();
                    float y = root.GetProperty("y").GetSingle();
                    float z = root.GetProperty("z").GetSingle();

                    Vector3 position = new Vector3(x, y, z);
                    battleModel.ctfModel.DropFlag(this, position);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SendShotsData()
        {
            Send(CommandType.BATTLE, "init_shots_data", WeaponsFactory.GetJSONList());
        }

        public void ParseAndMove(string[] args)
        {
            try
            {
                Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 orient = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 line = new Vector3(0.0f, 0.0f, 0.0f);
                Vector3 angVel = new Vector3(0.0f, 0.0f, 0.0f);
                float turretDir = 0.0f;

                string[] temp = args[1].Split("@");

                pos.X = float.Parse(temp[0], CultureInfo.InvariantCulture);
                pos.Y = float.Parse(temp[1], CultureInfo.InvariantCulture);
                pos.Z = float.Parse(temp[2], CultureInfo.InvariantCulture);

                if (temp.Length > 3)
                {
                    orient.X = float.Parse(temp[3], CultureInfo.InvariantCulture);
                    orient.Y = temp.Length > 4 ? float.Parse(temp[4], CultureInfo.InvariantCulture) : 0.0f;
                    orient.Z = temp.Length > 5 ? float.Parse(temp[5], CultureInfo.InvariantCulture) : 0.0f;
                }

                if (temp.Length > 6)
                {
                    line.X = float.Parse(temp[6], CultureInfo.InvariantCulture);
                    line.Y = temp.Length > 7 ? float.Parse(temp[7], CultureInfo.InvariantCulture) : 0.0f;
                    line.Z = temp.Length > 8 ? float.Parse(temp[8], CultureInfo.InvariantCulture) : 0.0f;
                }

                if (temp.Length > 9)
                {
                    angVel.X = float.Parse(temp[9], CultureInfo.InvariantCulture);
                    angVel.Y = temp.Length > 10 ? float.Parse(temp[10], CultureInfo.InvariantCulture) : 0.0f;
                    angVel.Z = temp.Length > 11 ? float.Parse(temp[11], CultureInfo.InvariantCulture) : 0.0f;
                }

                turretDir = float.Parse(args[2], CultureInfo.InvariantCulture);
                int bits = int.Parse(args[3]);

                tank.Position ??= new Vector3(0.0f, 0.0f, 0.0f);

                tank.Position = pos;
                tank.Orientation = orient;
                tank.LinVel = line;
                tank.AngVel = angVel;
                tank.TurretDir = turretDir;
                tank.ControllBits = bits;

                battleModel.MoveTank(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ClearEffects()
        {
            while (tank.ActiveEffects.Count() > 0)
            {
                tank.ActiveEffects[0].Deactivate();
            }

        }
        public void Destroy(bool cache)
        {
            battleModel.RemoveUser(this, cache);
            if (!cache)
            {
                lobbysServices.SendCommandToAllUsers(CommandType.BATTLE, UserLocation.BATTLESELECT, "remove_player_from_battle", JSONUtils.ParseRemovePlayerComand(this));
                if (!battleModel.battleInfo.Team)
                {
                    lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, StringUtils.ConcatStrings("update_count_users_in_dm_battle", ";", battleModel.battleInfo.BattleId, ";", battleModel.Players.Count().ToString()));
                }
                else
                {
                    lobbysServices.SendCommandToAllUsers(CommandType.LOBBY, UserLocation.BATTLESELECT, "update_count_users_in_team_battle", JSONUtils.ParseUpdateCountPeopleCommand(battleModel.battleInfo));
                }
            }

            parentLobby = null;
            battleModel = null;
            tank = null;
        }

        public void Send(CommandType type, params string[] args)
        {
            if (parentLobby != null)
            {
                parentLobby.transfer.Send(type, args);
            }
        }

        public void OnDisconnect()
        {
            autoEntryServices.UserExit(this);
            Destroy(true);
        }



    }
}
