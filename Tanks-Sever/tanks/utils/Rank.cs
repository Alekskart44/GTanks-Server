namespace Tanks_Sever.tanks.utils
{
    public class Rank
    {
        public int Min { get; }
        public int Max { get; }
        public string Name { get; }

        public Rank(int min, int max, string name)
        {
            Min = min;
            Max = max;
            Name = name;
        }
    }
}
