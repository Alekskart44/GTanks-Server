namespace Tanks_Sever.tanks.utils
{
    public static class RankUtils
    {
        private static Rank[] ranks;

        public static void Init()
        {
            ranks = new Rank[27];
            ranks[0] = new Rank(0, 99, "Новобранец");
            ranks[1] = new Rank(100, 499, "Рядовой");
            ranks[2] = new Rank(500, 1499, "Ефрейтор");
            ranks[3] = new Rank(1500, 3699, "Капрал");
            ranks[4] = new Rank(3700, 7099, "Мастер-капрал");
            ranks[5] = new Rank(7100, 12299, "Сержант");
            ranks[6] = new Rank(12300, 19999, "Штаб-сержант");
            ranks[7] = new Rank(20000, 28999, "Мастер-сержант");
            ranks[8] = new Rank(29000, 40999, "Первый сержант");
            ranks[9] = new Rank(41000, 56999, "Сержант-майор");
            ranks[10] = new Rank(57000, 75999, "Уорэент-офицер 1");
            ranks[11] = new Rank(76000, 97999, "Уорэент-офицер 2");
            ranks[12] = new Rank(98000, 124999, "Уорэент-офицер 3");
            ranks[13] = new Rank(125000, 155999, "Уорэент-офицер 4");
            ranks[14] = new Rank(156000, 191999, "Уорэент-офицер 5");
            ranks[15] = new Rank(192000, 232999, "Младшый лейтенант");
            ranks[16] = new Rank(233000, 279999, "Лейтенант");
            ranks[17] = new Rank(280000, 331999, "Старший лейтенант");
            ranks[18] = new Rank(332000, 389999, "Капитан");
            ranks[19] = new Rank(390000, 454999, "Майор");
            ranks[20] = new Rank(455000, 526999, "Подполковник");
            ranks[21] = new Rank(527000, 605999, "Полковник");
            ranks[22] = new Rank(606000, 691999, "Бригадир");
            ranks[23] = new Rank(692000, 786999, "Генерал-майор");
            ranks[24] = new Rank(787000, 888999, "Генерал-лейтенант");
            ranks[25] = new Rank(889000, 999999, "Генерал");
            ranks[26] = new Rank(1000000, 0, "Маршал");
        }

        public static int GetUpdateNumber(int score)
        {
            Rank temp = GetRankByScore(score);
            int rankId = GetNumberRank(temp);
            int rank = rankId;
            int result;

            try
            {
                result = (int)((score - ranks[rank - 1].Max) * 1.0 / (temp.Max - ranks[rank - 1].Max) * 10000.0);
            }
            catch
            {
                result = (int)((score - 0) * 1.0 / (temp.Max - 0) * 10000.0);
            }

            if (score > ranks[ranks.Length - 1].Min - 1)
            {
                result = 10000;
            }
            else if (score < 0)
            {
                result = 0;
            }

            return result;
        }

        public static int GetNumberRank(Rank rank)
        {
            for (int i = 0; i < ranks.Length; ++i)
            {
                if (ranks[i] == rank)
                {
                    return i;
                }
            }

            return -1;
        }

        public static Rank GetRankByScore(int score)
        {
            Rank temp = ranks[0];

            if (score >= ranks[26].Max)
            {
                temp = ranks[26];
            }

            foreach (Rank rank in ranks)
            {
                if (score >= rank.Min && score <= rank.Max)
                {
                    temp = rank;
                }
            }

            return temp;
        }

        public static Rank GetRankByIndex(int index)
        {
            return ranks[index];
        }

        public static int StringToInt(string src)
        {
            try
            {
                int tempelate = int.Parse(src);
                if (tempelate <= 0)
                {
                    tempelate = 5000000;
                }

                return tempelate >= ranks[26].Min ? ranks[26].Min : tempelate;
            }
            catch
            {
                return 50000000;
            }
        }
    }
}
