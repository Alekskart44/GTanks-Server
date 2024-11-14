using Tanks_Sever.tanks.battles.maps;
using Tanks_Sever.tanks.battles.tanks.math;
using System;

namespace Tanks_Sever.tanks.battles.managers
{
    public class SpawnManager
    {
        private static readonly Random rand = new Random();

        public static Vector3 GetSpawnState(Map map, string forTeam)
        {
            Vector3 pos = null;

            try
            {
                if (forTeam.Equals("BLUE", StringComparison.OrdinalIgnoreCase))
                {
                    int index = rand.Next(map.SpawnPositionsBlue.Count);
                    pos = map.SpawnPositionsBlue[index];
                }
                else if (forTeam.Equals("RED", StringComparison.OrdinalIgnoreCase))
                {
                    int index = rand.Next(map.SpawnPositionsRed.Count);
                    pos = map.SpawnPositionsRed[index];
                }
                else
                {
                    int index = rand.Next(map.SpawnPositionsDM.Count);
                    pos = map.SpawnPositionsDM[index];
                }

                // проверка на null после выбора позиции
                if (pos == null)
                {
                    int index = rand.Next(map.SpawnPositionsDM.Count);
                    pos = map.SpawnPositionsDM[index];
                }
            }
            catch (Exception)
            {
                if (map.SpawnPositionsDM.Count > 0)
                {
                    int index = rand.Next(map.SpawnPositionsDM.Count);
                    pos = map.SpawnPositionsDM[index];
                }
            }

            return pos;
        }
    }
}
