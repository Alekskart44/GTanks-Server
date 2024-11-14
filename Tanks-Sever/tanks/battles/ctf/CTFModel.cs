using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.battles.ctf.flags;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.main.procotol.commands;
using gtanks.battles.ctf;

namespace Tanks_Sever.tanks.battles.ctf
{
    public class CTFModel
    {
        private readonly BattlefieldModel bfModel;
        private readonly FlagServer blueFlag = new FlagServer();
        private readonly FlagServer redFlag = new FlagServer();
        private readonly TanksServices tanksServices = TanksServices.getInstance();

        public CTFModel(BattlefieldModel bfModel)
        {
            this.bfModel = bfModel;
            blueFlag.FlagTeamType = "BLUE";
            redFlag.FlagTeamType = "RED";
            blueFlag.State = FlagState.BASE;
            redFlag.State = FlagState.BASE;
            blueFlag.Position = bfModel.battleInfo.Map.FlagBluePosition;
            blueFlag.BasePosition = blueFlag.Position;
            redFlag.Position = bfModel.battleInfo.Map.FlagRedPosition;
            redFlag.BasePosition = redFlag.Position;
        }

        public void AttemptToTakeFlag(BattlefieldPlayerController taker, string flagTeamType)
        {
            FlagServer flag = GetTeamFlag(flagTeamType);
            if (flag.Owner == null)
            {
                if (taker.playerTeamType.Equals(flagTeamType))
                {
                    FlagServer enemyFlag = GetEnemyTeamFlag(flagTeamType);
                    if (flag.State == FlagState.DROPED)
                    {
                        ReturnFlag(taker, flag);
                        return;
                    }

                    if (enemyFlag.Owner == taker)
                    {

                        bfModel.SendToAllPlayers(CommandType.BATTLE, "deliver_flag", taker.playerTeamType, taker.tank.Id);
                        enemyFlag.State = FlagState.BASE;
                        enemyFlag.Owner = null;
                        taker.flag = null;
                        if (enemyFlag.ReturnTimer != null)
                        {
                            enemyFlag.ReturnTimer.stop = true;
                            enemyFlag.ReturnTimer = null;
                        }

                        int score = (taker.playerTeamType == "BLUE" ? bfModel.battleInfo.RedPeople : bfModel.battleInfo.BluePeople) * 10;
                        tanksServices.AddScore(taker.parentLobby, score);
                        taker.statistic.AddScore(score);
                        bfModel.statistics.ChangeStatistic(taker);

                        double fund = 0.0;
                        List<BattlefieldPlayerController> otherTeam = new List<BattlefieldPlayerController>();
                        var players = bfModel.Players.GetEnumerator();

                        while (players.MoveNext()) // Перебираем игроков
                        {
                            BattlefieldPlayerController otherPlayer = players.Current.Value; // Получаем текущего игрока
                            if (!otherPlayer.playerTeamType.Equals(taker.playerTeamType) && !otherPlayer.playerTeamType.Equals("NONE"))
                            {
                                otherTeam.Add(otherPlayer);
                            }
                        }

                        foreach (var otherPlayer in otherTeam)
                        {
                            fund += Math.Sqrt(otherPlayer.GetUser().Rang * 0.125);
                        }

                        bfModel.tanksKillModel.AddFund(fund);
                        if (taker.playerTeamType == "BLUE")
                        {
                            bfModel.battleInfo.ScoreBlue++;
                            bfModel.SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "BLUE", bfModel.battleInfo.ScoreBlue.ToString());
                            if (bfModel.battleInfo.NumFlags == bfModel.battleInfo.ScoreBlue)
                            {
                                bfModel.tanksKillModel.RestartBattle(false);
                            }
                        }
                        else
                        {
                            bfModel.battleInfo.ScoreRed++;
                            bfModel.SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "RED", bfModel.battleInfo.ScoreRed.ToString());
                            if (bfModel.battleInfo.NumFlags == bfModel.battleInfo.ScoreRed)
                            {
                                bfModel.tanksKillModel.RestartBattle(false);
                            }
                        }
                    }
                }
                else
                {

                    bfModel.SendToAllPlayers(CommandType.BATTLE, "flagTaken", taker.tank.Id, flagTeamType);
                    flag.State = FlagState.TAKEN_BY;
                    flag.Owner = taker;
                    taker.flag = flag;
                    if (flag.ReturnTimer != null)
                    {
                        flag.ReturnTimer.stop = true;
                        flag.ReturnTimer = null;
                    }
                }
            }
        }

        public void DropFlag(BattlefieldPlayerController following, Vector3 posDrop)
        {
            FlagServer flag = GetEnemyTeamFlag(following.playerTeamType);
            flag.State = FlagState.DROPED;
            flag.Position = posDrop;
            flag.Owner = null;
            following.flag = null;
            flag.ReturnTimer = new FlagReturnTimer(this, flag);
            flag.ReturnTimer.Start();
            bfModel.SendToAllPlayers(CommandType.BATTLE, "flag_drop", JSONUtils.ParseDropFlagCommand(flag));
        }

        public void ReturnFlag(BattlefieldPlayerController following, FlagServer flag)
        {
            flag.State = FlagState.BASE;
            if (flag.Owner != null)
            {
                flag.Owner.flag = null;
                flag.Owner = null;
            }

            flag.Position = flag.BasePosition;
            if (flag.ReturnTimer != null)
            {
                flag.ReturnTimer.stop = true;
                flag.ReturnTimer = null;
            }

            string id = following?.tank.Id;
            bfModel.SendToAllPlayers(CommandType.BATTLE, "return_flag", flag.FlagTeamType, id);
            int score = 5;
            if (following != null)
            {
                tanksServices.AddScore(following.parentLobby, score);
                following.statistic.AddScore(score);
                bfModel.statistics.ChangeStatistic(following);
            }
        }

        private FlagServer GetTeamFlag(string teamType)
        {
            return teamType.Equals("BLUE") ? blueFlag : teamType.Equals("RED") ? redFlag : null;
        }

        private FlagServer GetEnemyTeamFlag(string teamType)
        {
            return teamType.Equals("BLUE") ? redFlag : teamType.Equals("RED") ? blueFlag : null;
        }

        public FlagServer GetRedFlag() => redFlag;

        public FlagServer GetBlueFlag() => blueFlag;
    }
}