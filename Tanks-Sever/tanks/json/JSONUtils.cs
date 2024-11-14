using System.Text.Json;
using Tanks_Sever.tanks.battles.maps;
using Tanks_Sever.tanks.lobby.chat;
using Tanks_Sever.tanks.lobby.top;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.users.garage;
using Tanks_Sever.tanks.users.garage.enums;
using Tanks_Sever.tanks.users.garage.items;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.battles;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.tanks;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.battles.tanks.weapons.data;
using Tanks_Sever.tanks.battles.tanks.weapons;
using Tanks_Sever.tanks.battles.ctf.flags;
using Tanks_Sever.tanks.battles.ctf;
using Tanks_Sever.tanks.battles.bonuses;
using Tanks_Sever.tanks.battles.mine;

namespace Tanks_Sever.tanks.json
{

    public class JSONUtils
    {

        public static string parseUser(User user)
        {
            var obj = new Dictionary<string, object> { };
            obj["name"] = user.GetNickname();
            obj["crystall"] = user.GetCrystall();
            obj["email"] = user.GetEmail();
            obj["tester"] = user.Type != TypeUser.DEFAULT;
            obj["next_score"] = user.GetNextScore();
            obj["place"] = user.GetPlace();
            obj["rang"] = user.GetRang() + 1;
            obj["rating"] = user.GetRating();
            obj["score"] = user.GetScore();

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseAddPlayerCommand(BattlefieldPlayerController player, BattleInfo battleInfo)
        {
            var obj = new Dictionary<string, object> {};
            obj["battleId"] = battleInfo.BattleId;
            obj["id"] = player.GetUser().GetNickname();
            obj["kills"] = player.statistic.Score;
            obj["name"] = player.GetUser().GetNickname();
            obj["rank"] = player.GetUser().GetRang() + 1;
            obj["type"] = player.playerTeamType;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseRemovePlayerComand(BattlefieldPlayerController player)
        {
            var obj = new Dictionary<string, object> { };
            obj["battleId"] = player.battleModel.battleInfo.BattleId;
            obj["id"] = player.GetUser().GetNickname();

            return JsonSerializer.Serialize(obj);
        }

        public static Dictionary<string, object> ParseUserToJSONObject(User user)
        {
            var obj = new Dictionary<string, object> { };
            obj["name"] = user.GetNickname();
            obj["score"] = user.GetScore();
            obj["rang"] = user.GetRang();
            obj["crystall"] = user.GetCrystall();
            obj["rating"] = user.GetRating();

            return obj;
        }

        public static string ParseHallOfFame(HallOfFame top)
        {
            var obj = new Dictionary<string, object>();
            var array = new List<Dictionary<string, object>>();

            foreach (var user in top.GetData())
            {
                array.Add(ParseUserToJSONObject(user));
            }

            obj["users_data"] = array;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseGarageUser(User user)
        {
            try
            {
                var garage = user.GetGarage();
                var obj = new Dictionary<string, object>();
                var itemsArray = new List<Dictionary<string, object>>();

                foreach (var item in garage.Items)
                {
                    var itemObj = new Dictionary<string, object>
                    {
                        { "id", item.Id },
                        { "name", item.Name.GetLocalizedString(user.GetLocalization()) },
                        { "description", item.Description.GetLocalizedString(user.GetLocalization()) },
                        { "isInventory", BoolToString(item.IsInventory) },
                        { "index", item.Index },

                        { "type", item.ItemType.ToStringValue() },
                        { "modificationID", item.ModificationIndex },
                        { "next_price", item.NextPrice },
                        { "next_rank", item.NextRankId },
                        { "price", item.Price },
                        { "rank", item.RankId },
                        { "count", item.Count },
                        { "properts", new List<Dictionary<string, object>>() },
                        { "modification", new List<Dictionary<string, object>>() }
                    };

                    if (item.Properties != null)
                    {
                        foreach (var prop in item.Properties)
                        {
                            if (prop != null && prop.Property != null)
                            {
                                ((List<Dictionary<string, object>>)itemObj["properts"]).Add(ParseProperty(prop));
                            }
                        }
                    }

                    if (item.Modifications != null)
                    {
                        foreach (var mod in item.Modifications)
                        {
                            var modificationItem = new Dictionary<string, object>
                            {
                                { "previewId", mod.PreviewId },
                                { "price", mod.Price },
                                { "rank", mod.Rank },
                                { "properts", new List<Dictionary<string, object>>() }
                            };

                            if (mod.Propertys != null)
                            {
                                foreach (var a in mod.Propertys)
                                {
                                    if (a != null && a.Property != null)
                                    {
                                        ((List<Dictionary<string, object>>)modificationItem["properts"]).Add(ParseProperty(a));
                                    }
                                }
                            }

                            ((List<Dictionary<string, object>>)itemObj["modification"]).Add(modificationItem);
                        }
                    }

                    itemsArray.Add(itemObj);
                }

                obj["items"] = itemsArray;
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static string ParseMarketItems(User user)
        {
            var garage = user.GetGarage();
            var json = new Dictionary<string, object>();
            var itemsArray = new List<Dictionary<string, object>>();

            foreach (var item in GarageItemsLoader.Items.Values)
            {
                if (!garage.ContainsItem(item.Id) && !item.SpecialItem)
                {
                    var itemObj = new Dictionary<string, object>
                    {
                        { "id", item.Id },
                        { "name", item.Name.GetLocalizedString(user.GetLocalization()) },
                        { "description", item.Description.GetLocalizedString(user.GetLocalization()) },
                        { "isInventory", item.IsInventory },
                        { "index", item.Index },
                        { "type", item.ItemType.ToStringValue() },
                        { "modificationID", 0 },
                        { "next_price", item.NextPrice },
                        { "next_rank", item.NextRankId },
                        { "price", item.Price },
                        { "rank", item.RankId },
                        { "properts", new List<Dictionary<string, object>>() },
                        { "modification", new List<Dictionary<string, object>>() }
                    };

                    if (item.Properties != null)
                    {
                        foreach (var prop in item.Properties)
                        {
                            if (prop != null && prop.Property != null)
                            {
                                ((List<Dictionary<string, object>>)itemObj["properts"]).Add(ParseProperty(prop));
                            }
                        }
                    }

                    if (item.Modifications != null)
                    {
                        foreach (var mod in item.Modifications)
                        {
                            var modificationItem = new Dictionary<string, object>
                            {
                                { "previewId", mod.PreviewId },
                                { "price", mod.Price },
                                { "rank", mod.Rank },
                                { "properts", new List<Dictionary<string, object>>() }
                            };

                            if (mod.Propertys != null)
                            {
                                foreach (var a in mod.Propertys)
                                {
                                    if (a != null && a.Property != null)
                                    {
                                        ((List<Dictionary<string, object>>)modificationItem["properts"]).Add(ParseProperty(a));
                                    }
                                }
                            }

                            ((List<Dictionary<string, object>>)itemObj["modification"]).Add(modificationItem);
                        }
                    }

                    itemsArray.Add(itemObj);
                }
            }

            json["items"] = itemsArray;
            return JsonSerializer.Serialize(json);
        }

        public static string ParseItemInfo(Item item)
        {
            var obj = new Dictionary<string, object>();
            obj["itemId"] = item.Id;
            obj["count"] = item.Count;
            return JsonSerializer.Serialize(obj);
        }

        private static Dictionary<string, object> ParseProperty(PropertyItem item)
        {
            var propertyItem = new Dictionary<string, object>();
            propertyItem["property"] = item.Property.ToStringValue();
            propertyItem["value"] = item.Value;
            return propertyItem;
        }

        public static string ParseTankSpec(Tank tank, bool notSmooth)
        {
            var obj = new Dictionary<string, object>();
            obj["speed"] = tank.Speed;
            obj["turnSpeed"] = tank.TurnSpeed;
            obj["turretRotationSpeed"] = tank.TurretRotationSpeed;
            obj["immediate"] = notSmooth;

            return JsonSerializer.Serialize(obj);
        }

        private static string BoolToString(bool value)
        {
            return value ? "true" : "false";
        }

        public static string parseChatLobbyMessage(ChatMessage msg)
        {
            var obj = new Dictionary<string, object>();
            obj["name"] = msg.User?.GetNickname() ?? "";
            obj["rang"] = msg.User != null ? msg.User.GetRang() + 1 : 0;
            obj["message"] = msg.Message;
            obj["addressed"] = msg.Addressed;
            obj["nameTo"] = msg.UserTo?.GetNickname() ?? "";
            obj["rangTo"] = msg.UserTo != null ? msg.UserTo.GetRang() + 1 : 0;
            obj["system"] = msg.System;
            obj["yellow"] = msg.YellowMessage;

            return JsonSerializer.Serialize(obj);
        }

        public static Dictionary<string, object> ParseChatLobbyMessageObject(ChatMessage msg)
        {
            var obj = new Dictionary<string, object> { };
            obj["name"] = msg.User?.GetNickname() ?? "";
            obj["rang"] = msg.User != null ? msg.User.GetRang() + 1 : 0;
            obj["message"] = msg.Message;
            obj["addressed"] = msg.Addressed;
            obj["nameTo"] = msg.UserTo?.GetNickname() ?? "";
            obj["rangTo"] = msg.UserTo != null ? msg.UserTo.GetRang() + 1 : 0;
            obj["system"] = msg.System;
            obj["yellow"] = msg.YellowMessage;

            return obj;
        }

        public static string ParseChatLobbyMessages(IEnumerable<ChatMessage> messages)
        {
            var obj = new Dictionary<string, object>();
            var array = new List<Dictionary<string, object>>();
            foreach (var msg in messages)
            {
                array.Add(ParseChatLobbyMessageObject(msg));
            }
            obj["messages"] = array;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseBattleMapList()
        {
            var result = new Dictionary<string, object>
            {
                ["items"] = new List<Dictionary<string, object>>(),
                ["battles"] = new List<Dictionary<string, object>>()
            };

            foreach (var map in MapsLoader.Maps.Values)
            {
                var jmap = new Dictionary<string, object>
                {
                    ["id"] = map.Id.Replace(".xml", ""),
                    ["name"] = map.Name,
                    ["gameName"] = "тип gameName",
                    ["maxPeople"] = map.MaxPlayers,
                    ["maxRank"] = map.MaxRank,
                    ["minRank"] = map.MinRank,
                    ["themeName"] = map.ThemeId,
                    ["skyboxId"] = map.SkyboxId,
                    ["ctf"] = map.Ctf,
                    ["tdm"] = map.Tdm
                };
                ((List<Dictionary<string, object>>)result["items"]).Add(jmap);
            }

            foreach (var battle in BattlesList.GetList())
            {
                ((List<Dictionary<string, object>>)result["battles"]).Add(ParseBattleInfo(battle));
            }

            return JsonSerializer.Serialize(result);
        }

        public static string ParseBattleInfoCreate(BattleInfo battle)
        {
            var obj = new Dictionary<string, object>();
            obj["battleId"] = battle.BattleId;
            obj["mapId"] = battle.Map.Id;
            obj["name"] = battle.Name;
            obj["previewId"] = battle.Map.Id + "_preview";
            obj["team"] = battle.Team;
            obj["redPeople"] = battle.RedPeople;
            obj["bluePeople"] = battle.BluePeople;
            obj["countPeople"] = battle.CountPeople;
            obj["maxPeople"] = battle.MaxPeople;
            obj["minRank"] = battle.MinRank;
            obj["maxRank"] = battle.MaxRank;
            obj["isPaid"] = battle.IsPaid;

            return JsonSerializer.Serialize(obj);
        }

        public static Dictionary<string, object> ParseBattleInfo(BattleInfo battle)
        {
            var obj = new Dictionary<string, object>();
            obj["battleId"] = battle.BattleId;
            obj["mapId"] = battle.Map.Id;
            obj["name"] = battle.Name;
            obj["previewId"] = battle.Map.Id + "_preview";
            obj["team"] = battle.Team;
            obj["redPeople"] = battle.RedPeople;
            obj["bluePeople"] = battle.BluePeople;
            obj["countPeople"] = battle.CountPeople;
            obj["maxPeople"] = battle.MaxPeople;
            obj["minRank"] = battle.MinRank;
            obj["maxRank"] = battle.MaxRank;
            obj["isPaid"] = battle.IsPaid;
            return obj;
        }

        public static string ParseBattleInfoShow(BattleInfo battle, bool spectator)
        {
            var json = new Dictionary<string, object>();

            if (battle == null)
            {
                json["null_battle"] = true;
                return JsonSerializer.Serialize(json);
            }

            try
            {
                var users = new List<Dictionary<string, object>>();

                if (battle != null && battle.Model != null && battle.Model.Players != null)
                {
                    // Перебор игроков в битве
                    foreach (var player in battle.Model.Players.Values)
                    {
                        var objUser = new Dictionary<string, object>
                        {
                            { "nickname", player.parentLobby.GetLocalUser().GetNickname() },
                            { "rank", player.parentLobby.GetLocalUser().GetRang() + 1 },
                             //{ "kills", player.Statistic.Kills },
                            { "team_type", player.playerTeamType }

                        };
                        users.Add(objUser);
                    }


                    // Перебор игроков из AutoEntryServices
                    foreach (var player in AutoEntryServices.Instance.GetPlayersByBattle(battle.Model))
                    {
                        var objUser = new Dictionary<string, object>();
                        var user = Repository.GenericManager.repository.SingleByNickname<User>(player.UserId);

                        objUser["nickname"] = user.Nickname;
                        objUser["rank"] = user.Rang + 1;
                        objUser["kills"] = player.Statistic.Kills;
                        objUser["team_type"] = player.TeamType;

                        users.Add(objUser);
                    }
                }

                json["users_in_battle"] = users;
                json["name"] = battle.Name;
                json["maxPeople"] = battle.MaxPeople;
                json["type"] = battle.BattleType;
                json["battleId"] = battle.BattleId;
                json["minRank"] = battle.MinRank;
                json["maxRank"] = battle.MaxRank;
                json["timeLimit"] = battle.Time;
                json["timeCurrent"] = battle.Model.GetTimeLeft();
                json["killsLimit"] = (battle.BattleType == "CTF") ? battle.NumFlags : battle.NumKills;
                json["scoreRed"] = battle.ScoreRed;
                json["scoreBlue"] = battle.ScoreBlue;
                json["autobalance"] = battle.Autobalance;
                json["friendlyFire"] = battle.FriendlyFire;
                json["paidBattle"] = battle.IsPaid;
                json["withoutBonuses"] = true;
                json["userAlreadyPaid"] = true;
                json["fullCash"] = true;
                json["spectator"] = spectator;
                json["previewId"] = battle.Map.Id + "_preview";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return JsonSerializer.Serialize(json);
            }

            return JsonSerializer.Serialize(json);
        }

        public static string ParseUpdateCountPeopleCommand(BattleInfo battle)
        {
            var obj = new Dictionary<string, object>();
            obj["battleId"] = battle.BattleId;
            obj["redPeople"] = battle.RedPeople;
            obj["bluePeople"] = battle.BluePeople;
            return JsonSerializer.Serialize(obj);
        }

        public static string ParseRemovePlayerCommand(string userId, string battleid)
        {
            var obj = new Dictionary<string, object>();
            obj["battleId"] = battleid;
            obj["id"] = userId;
            return JsonSerializer.Serialize(obj);
        }

        public static string ParseBattleModelInfo(BattleInfo battle, bool spectatorMode)
        {
            var obj = new Dictionary<string, object>();
            obj["kick_period_ms"] = 125000;
            obj["map_id"] = battle.Map.Id.Replace(".xml", "");
            obj["invisible_time"] = 3500;
            obj["skybox_id"] = battle.Map.SkyboxId;
            obj["spectator"] = spectatorMode;
            obj["sound_id"] = battle.Map.MapTheme.GetAmbientSoundId();
            obj["game_mode"] = battle.Map.MapTheme.GetGameModeId();
            return JsonSerializer.Serialize(obj);
        }

        public static string ParseBattleData(BattlefieldModel model)
        {
            var obj = new Dictionary<string, object>
        {
            { "name", model.battleInfo.Name },
            { "fund", model.tanksKillModel.GetBattleFund() },
            { "scoreLimit", (model.battleInfo.BattleType == "CTF") ? model.battleInfo.NumFlags : model.battleInfo.NumKills },
            { "timeLimit", model.battleInfo.Time },
            { "currTime", model.GetTimeLeft() },
            { "score_red", model.battleInfo.ScoreRed },
            { "score_blue", model.battleInfo.ScoreBlue },
            { "team", model.battleInfo.Team }
        };

            var users = new List<Dictionary<string, object>>();

            foreach (var player in model.Players.Values)
            {
                var usr = new Dictionary<string, object>
            {
                { "nickname", player.parentLobby.GetLocalUser().GetNickname() },
                { "rank", player.parentLobby.GetLocalUser().GetRang() + 1 },
                { "teamType", player.playerTeamType }
            };
                users.Add(usr);
            }

            obj.Add("users", users);

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseTankData(BattlefieldModel player, BattlefieldPlayerController controller, Garage garageUser, Vector3 pos, bool stateNull, int incration, string idTank, string nickname, int rank)
        {
            var obj = new Dictionary<string, object>();
            obj["battleId"] = player.battleInfo.BattleId;
            obj["colormap_id"] = garageUser.MountColormap.Id + "_m0";
            obj["hull_id"] = garageUser.MountHull.Id + "_m" + garageUser.MountHull.ModificationIndex;
            obj["turret_id"] = garageUser.MountTurret.Id + "_m" + garageUser.MountTurret.ModificationIndex;
            obj["team_type"] = controller.playerTeamType;
            if (pos == null)
            {
                pos = new Vector3(0, 0, 0);
            }
            obj["position"] = pos.X + "@" + pos.Y + "@" + pos.Z + "@" + pos.Rot;
            obj["incration"] = incration;
            obj["tank_id"] = idTank;
            obj["nickname"] = nickname;
            obj["state"] = controller.tank.State;
            obj["turn_speed"] = controller.tank.GetHull().TurnSpeed;
            obj["speed"] = controller.tank.GetHull().Speed;
            obj["turret_turn_speed"] = controller.tank.TurretRotationSpeed;
            obj["health"] = controller.tank.Health;
            obj["rank"] = rank + 1;
            obj["mass"] = controller.tank.GetHull().Mass;
            obj["power"] = controller.tank.GetHull().Power;
            obj["kickback"] = controller.tank.GetWeapon().GetEntity().GetShotData().Kickback;
            obj["turret_rotation_accel"] = controller.tank.GetWeapon().GetEntity().GetShotData().TurretRotationAccel;
            obj["impact_force"] = controller.tank.GetWeapon().GetEntity().GetShotData().ImpactCoeff;
            obj["state_null"] = stateNull;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseMoveCommand(BattlefieldPlayerController player)
        {
            Tank tank = player.tank;
            var json = new Dictionary<string, object>{};
            var pos = new Dictionary<string, float>{};
            var orient = new Dictionary<string, float>{};
            var line = new Dictionary<string, float>{};
            var angle = new Dictionary<string, float>{};

            pos["x"] = tank.Position.X;
            pos["y"] = tank.Position.Y;
            pos["z"] = tank.Position.Z;

            orient["x"] = tank.Orientation.X;
            orient["y"] = tank.Orientation.Y;
            orient["z"] = tank.Orientation.Z;

            line["x"] = tank.LinVel.X;
            line["y"] = tank.LinVel.Y;
            line["z"] = tank.LinVel.Z;

            angle["x"] = tank.AngVel.X;
            angle["y"] = tank.AngVel.Y;
            angle["z"] = tank.AngVel.Z;

            json["position"] = pos;
            json["orient"] = orient;
            json["line"] = line;
            json["angle"] = angle;
            json["turretDir"] = tank.TurretDir;
            json["ctrlBits"] = tank.ControllBits;
            json["tank_id"] = tank.Id;

            return JsonSerializer.Serialize(json);
        }

        public static string ParsePlayerStatistic(BattlefieldPlayerController player)
        {
            var obj = new Dictionary<string, object>();
            obj["kills"] = player.statistic.Kills;
            obj["deaths"] = player.statistic.Deaths;
            obj["id"] = player.GetUser().GetNickname();
            obj["rank"] = player.GetUser().GetRang() + 1;
            obj["team_type"] = player.playerTeamType;
            obj["score"] = player.GetUser().GetScore();

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseSpawnCommand(BattlefieldPlayerController bpc, Vector3 pos)
        {
            var obj = new Dictionary<string, object>();
            if (bpc != null && bpc.tank != null)
            {
                obj["tank_id"] = bpc.tank.Id;
                obj["health"] = bpc.tank.Health;
                obj["speed"] = bpc.tank.Speed;
                obj["turn_speed"] = bpc.tank.TurnSpeed;
                obj["turret_rotation_speed"] = bpc.tank.TurretRotationSpeed;
                obj["incration_id"] = bpc.battleModel.incration;
                obj["team_type"] = bpc.playerTeamType;
                obj["x"] = pos.X;
                obj["y"] = pos.Y;
                obj["z"] = pos.Z;
                obj["rot"] = pos.Rot;
                return JsonSerializer.Serialize(obj);
            }
            else 
            {
                 return null;
            }
        }

        public static string ParseFinishBattle(Dictionary<string, BattlefieldPlayerController> players, int timeToRestart)
        {
            var obj = new Dictionary<string, object>
            {
                { "time_to_restart", timeToRestart }
            };

            // Проверка наличия игроков
            if (players == null || players.Count == 0)
            {
                return JsonSerializer.Serialize(obj);
            }

            var users = new List<Dictionary<string, object>>();
            foreach (var bpc in players.Values)
            {
                var stat = new Dictionary<string, object>
                {
                    { "kills", bpc.statistic.Kills },
                    { "deaths", bpc.statistic.Deaths },
                    { "id", bpc.GetUser().GetNickname() },
                    { "rank", bpc.GetUser().GetRang() + 1 },
                    { "prize", bpc.statistic.Prize },
                    { "team_type", bpc.playerTeamType },
                    { "score", bpc.statistic.Score }
                };

                users.Add(stat);
            }

            obj["users"] = users;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseInitInventoryCommand(Garage garage)
        {
            var itemsList = new List<Dictionary<string, object>>();
            var obj = new Dictionary<string, object>();

            foreach (var item in garage.GetInventoryItems())
            {
                var itemDict = new Dictionary<string, object>();
                itemDict["id"] = item.Id;
                itemDict["count"] = item.Count;
                itemDict["slotId"] = item.Index;
                itemDict["itemEffectTime"] = item.Id == "mine" ? 20 : (item.Id == "health" ? 20 : 55);
                itemDict["itemRestSec"] = 10;

                itemsList.Add(itemDict);
            }
            obj["items"] = itemsList;

            return JsonSerializer.Serialize(obj);
        }

        private static object ParseSpecialEntity(IEntity entity)
        {
            return new { };
        }

        public static string ParseWeapons(IEnumerable<IEntity> weapons, Dictionary<string, WeaponWeakeningData> wwds)
        {
            var weaponList = new List<Dictionary<string, object>>();

            foreach (var entity in weapons)
            {
                var shotData = entity.GetShotData();
                var wwd = wwds.ContainsKey(shotData.Id) ? wwds[shotData.Id] : null;

                var weapon = new Dictionary<string, object>
                {
                    ["auto_aiming_down"] = shotData.AutoAimingAngleDown,
                    ["auto_aiming_up"] = shotData.AutoAimingAngleUp,
                    ["num_rays_down"] = shotData.NumRaysDown,
                    ["num_rays_up"] = shotData.NumRaysUp,
                    ["reload"] = shotData.ReloadMsec,
                    ["id"] = shotData.Id,
                    ["has_wwd"] = wwd != null
                };

                if (wwd != null)
                {
                    weapon["max_damage_radius"] = wwd.MaximumDamageRadius;
                    weapon["min_damage_radius"] = wwd.MinimumDamageRadius;
                    weapon["min_damage_percent"] = wwd.MinimumDamagePercent;
                }

                weapon["special_entity"] = ParseSpecialEntity(entity);
                weaponList.Add(weapon);
            }

            var obj = new Dictionary<string, object>
            {
                ["weapons"] = weaponList
            };

            return JsonSerializer.Serialize(obj);
        }


        public static string ParseDropFlagCommand(FlagServer flag)
        {
            var obj = new Dictionary<string, object>();
            obj["x"] = flag.Position.X;
            obj["y"] = flag.Position.Y;
            obj["z"] = flag.Position.Z;
            obj["flagTeam"] = flag.FlagTeamType;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseCTFModelData(BattlefieldModel model)
        {
            var obj = new Dictionary<string, object>();
            CTFModel ctfModel = model.ctfModel;

            var basePosBlue = new Dictionary<string, object>();
            basePosBlue["x"] = model.battleInfo.Map.FlagBluePosition.X;
            basePosBlue["y"] = model.battleInfo.Map.FlagBluePosition.Y;
            basePosBlue["z"] = model.battleInfo.Map.FlagBluePosition.Z;

            var basePosRed = new Dictionary<string, object>();
            basePosRed["x"] = model.battleInfo.Map.FlagRedPosition.X;
            basePosRed["y"] = model.battleInfo.Map.FlagRedPosition.Y;
            basePosRed["z"] = model.battleInfo.Map.FlagRedPosition.Z;

            var posBlue = new Dictionary<string, object>();
            posBlue["x"] = ctfModel.GetBlueFlag().Position.X;
            posBlue["y"] = ctfModel.GetBlueFlag().Position.Y;
            posBlue["z"] = ctfModel.GetBlueFlag().Position.Z;

            var posRed = new Dictionary<string, object>();
            posRed["x"] = ctfModel.GetRedFlag().Position.X;
            posRed["y"] = ctfModel.GetRedFlag().Position.Y;
            posRed["z"] = ctfModel.GetRedFlag().Position.Z;

            obj["basePosBlueFlag"] = basePosBlue;
            obj["basePosRedFlag"] = basePosRed;
            obj["posBlueFlag"] = posBlue;
            obj["posRedFlag"] = posRed;
            obj["blueFlagCarrierId"] = ctfModel.GetBlueFlag().Owner == null ? null : ctfModel.GetBlueFlag().Owner.tank.Id;
            obj["redFlagCarrierId"] = ctfModel.GetRedFlag().Owner == null ? null : ctfModel.GetRedFlag().Owner.tank.Id;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseBonusInfo(Bonus bonus, int inc, int disappearingTime)
        {
            var obj = new Dictionary<string, object> { };
            obj["id"] = bonus.Type.ToStringType() + "_" + inc;
            obj["x"] = bonus.Position.X;
            obj["y"] = bonus.Position.Y;
            obj["z"] = bonus.Position.Z;
            obj["disappearing_time"] = disappearingTime;

            return JsonSerializer.Serialize(obj);
        }

        public static string ParseInitMinesCommand(Dictionary<BattlefieldPlayerController, List<ServerMine>> mines)
        {
            var obj = new Dictionary<string, object>();
            var minesArray = new List<Dictionary<string, object>>();

            foreach (var userMines in mines.Values)
            {
                foreach (var mine in userMines)
                {
                    var mineInfo = new Dictionary<string, object>
                    {
                        { "ownerId", mine.GetOwner().tank.Id },
                        { "mineId", mine.GetId() },
                        { "x", mine.GetPosition().X },
                        { "y", mine.GetPosition().Y },
                        { "z", mine.GetPosition().Z }
                    };
                    minesArray.Add(mineInfo);
                }
            }

            obj["mines"] = minesArray;
            return JsonSerializer.Serialize(obj);
        }

        public static string ParsePutMineCommand(ServerMine mine)
        {
            var obj = new Dictionary<string, object>();
            obj["mineId"] = mine.GetId();
            obj["userId"] = mine.GetOwner().tank.Id;
            obj["x"] =  mine.GetPosition().X;
            obj["y"] = mine.GetPosition().Y;
            obj["z"] = mine.GetPosition().Z;

            return JsonSerializer.Serialize(obj);
        }

    }

}
