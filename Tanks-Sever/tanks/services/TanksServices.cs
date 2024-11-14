using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.users;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.main.procotol.commands;
using Repository;

namespace Tanks_Sever.tanks.services
{
    public class TanksServices
    {
        private static readonly TanksServices instance = new TanksServices();

        public static TanksServices getInstance()
        {
            return instance;
        }

        public void AddScore(LobbyManager lobby, int score)
        {
            User user = lobby.GetLocalUser();
            user.AddScore(score);
            bool increase = user.GetScore() >= user.GetNextScore();
            bool fall = user.GetScore() < RankUtils.GetRankByIndex(user.GetRang()).Min;
            if (increase || fall)
            {
                user.SetRang(RankUtils.GetNumberRank(RankUtils.GetRankByScore(user.GetScore())));
                user.SetNextScore(user.GetRang() == 26 ? RankUtils.GetRankByIndex(user.GetRang()).Max : RankUtils.GetRankByIndex(user.GetRang()).Max + 1);
                lobby.transfer.Send(CommandType.LOBBY, "update_rang_progress", "10000");
                lobby.transfer.Send(CommandType.LOBBY, "update_rang", (user.GetRang() + 1).ToString(), user.GetNextScore().ToString());
            }
            int update = RankUtils.GetUpdateNumber(user.GetScore());
            lobby.transfer.Send(CommandType.LOBBY, "update_rang_progress", update.ToString());
            lobby.transfer.Send(CommandType.LOBBY, "add_score", user.GetScore().ToString());
            GenericManager.repository.Update(user);
        }

        public void AddCrystall(LobbyManager lobby, int crystall)
        {
            User user = lobby.GetLocalUser();
            user.AddCrystall(crystall);
            lobby.transfer.Send(CommandType.LOBBY, "add_crystall", user.GetCrystall().ToString());
            GenericManager.repository.Update(user);
        }


    }
}
