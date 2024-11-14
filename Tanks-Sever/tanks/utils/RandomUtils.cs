namespace Tanks_Sever.tanks.utils
{
    public static class RandomUtils
    {
        private static readonly Random random = new Random();

        public static float GetRandom(float min, float max)
        {
            if (min == max)
            {
                return min;
            }
            return (float)(min + random.NextDouble() * (max - min));
        }
    }
}
