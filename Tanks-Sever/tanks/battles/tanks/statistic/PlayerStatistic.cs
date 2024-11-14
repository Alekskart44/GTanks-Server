using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.tanks.statistic
{
    public class PlayerStatistic : IComparable<PlayerStatistic>
    {
        public long Kills { get; private set; }
        public int Deaths { get; private set; }
        public int Prize { get; set; }
        public long Score { get; private set; }

        public PlayerStatistic(int kills, int deaths, int score)
        {
            Kills = kills;
            Deaths = deaths;
            Score = score;
        }

        public void AddKills(bool killsEqualsScore)
        {
            Kills++;
            if (killsEqualsScore)
            {
                Score = Kills;
            }
        }

        public void AddDeaths()
        {
            Deaths++;
        }

        public void AddScore(int value)
        {
            Score += value;
        }

        public void SetScore(long value)
        {
            Score = value;
        }

        public void SetKills(long kills)
        {
            Kills = kills;
        }

        public void SetDeaths(int deaths)
        {
            Deaths = deaths;
        }

        public void Clear()
        {
            Kills = 0;
            Deaths = 0;
            Prize = 0;
            Score = 0;
        }

        public float GetKD()
        {
            return Deaths == 0 ? Kills : (float)Kills / Deaths;
        }

        public override string ToString()
        {
            return $"score: {Score} kills: {Kills} deaths: {Deaths} prize: {Prize}";
        }

        public int CompareTo(PlayerStatistic other)
        {
            return (int)(other.Score - Score);
        }
    }
}