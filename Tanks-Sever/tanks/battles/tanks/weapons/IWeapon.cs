namespace Tanks_Sever.tanks.battles.tanks.weapons
{
    public interface IWeapon
    {
        // Метод для выстрела
        void Fire(string parameter);

        // Метод для начала стрельбы
        void StartFire(string parameter);

        // Метод для остановки стрельбы
        void StopFire();

        // Метод для выполнения действия над врагом(целью)
        void OnTarget(BattlefieldPlayerController[] targets, int var2);

        // Метод для получения сущности оружия
        IEntity GetEntity();
    }
}