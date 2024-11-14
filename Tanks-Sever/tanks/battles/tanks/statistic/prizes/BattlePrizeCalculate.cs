using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.services;

namespace Tanks_Sever.tanks.battles.tanks.statistic.prizes
{
    public static class BattlePrizeCalculate
    {
        private static readonly TanksServices TankServices = TanksServices.getInstance();

        public static void Calc(List<BattlefieldPlayerController> users, int fund)
        {
            if (users == null || users.Count == 0) return;

            var _first = users.OrderByDescending(u => u.statistic.Score).FirstOrDefault();
            var first = _first.statistic;
            double sumSquare = 0.0;
            int countFirstUsers = users.Count(user => user.statistic.Score == first.Score);

            foreach (var user in users)
            {
                long value = user.statistic.Score;
                if (value != first.Score)
                {
                    sumSquare += value * value;
                }
            }

            sumSquare += first.Score * first.Score * (long)countFirstUsers * countFirstUsers;
            int allSum = 0;

            foreach (var user in users)
            {
                if (user.statistic.Score != first.Score)
                {
                    int prize = (int)((fund * (double)(user.statistic.Score * user.statistic.Score)) / sumSquare);
                    prize = Math.Abs(prize); 

                    allSum += prize;
                    user.statistic.Prize = prize;
                    TankServices.AddCrystall(user.parentLobby, prize);
                }
            }

            int delta = (fund - allSum) / countFirstUsers;

            foreach (var user in users)
            {
                var _user = user.statistic;
                if (_user.Score == first.Score && user != _first)
                {
                    _user.Prize = delta;
                    TankServices.AddCrystall(user.parentLobby, delta);
                    allSum += delta;
                }
            }

            first.Prize += (fund - allSum);
            TankServices.AddCrystall(_first.parentLobby, first.Prize);
        }

        public static void CalculateForTeam(
            List<BattlefieldPlayerController> redUsers,
            List<BattlefieldPlayerController> blueUsers,
            int scoreRed,
            int scoreBlue,
            double looseKoeff,
            int fund)
        {
            List<BattlefieldPlayerController> usersWin;
            List<BattlefieldPlayerController> usersLoose;
            int prizeWin, prizeLoose;

            if (scoreRed != scoreBlue)
            {
                int scoreWin = Math.Max(scoreRed, scoreBlue);
                int scoreLoose = Math.Min(scoreRed, scoreBlue);
                prizeLoose = (int)(fund * looseKoeff * scoreLoose / scoreWin);
                prizeWin = fund - prizeLoose;
                usersWin = scoreRed > scoreBlue ? redUsers : blueUsers;
                usersLoose = scoreRed > scoreBlue ? blueUsers : redUsers;
            }
            else
            {
                prizeLoose = (int)Math.Ceiling((double)fund / 2);
                prizeWin = (int)Math.Ceiling((double)fund / 2);
                usersWin = redUsers;
                usersLoose = blueUsers;
            }

            Calc(usersWin, prizeWin);
            Calc(usersLoose, prizeLoose);
        }
    }
}