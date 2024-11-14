namespace Tanks_Sever.tanks.battles.tanks.hulls
{
    public class Hull
    {
        public float Mass { get; set; }
        public float Power { get; set; }
        public float Speed { get; set; }
        public float TurnSpeed { get; set; }
        public float Hp { get; set; }

        public Hull(float mass, float power, float speed, float turnSpeed, float hp)
        {
            Mass = mass;
            Power = power;
            Speed = speed;
            TurnSpeed = turnSpeed;
            Hp = hp;
        }
    }
}