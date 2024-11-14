using Tanks_Sever.tanks.battles.tanks;

namespace Tanks_Sever.tanks.utils
{
    public static class WeaponUtils
    {

        // Метод для расчета здоровья танка после получения урона
        public static int CalculateHealth(Tank tank, float damage)
        {
            float calculatedDamage = 10000.0f / (tank.GetHull().Hp / damage);
            return (int)calculatedDamage;
        }

        // Метод для расчета урона с учетом дистанции (процентное уменьшение)
        public static float CalculateDamageFromDistance(float damage, int percent)
        {
            // Преобразование процента в дробное число и уменьшение урона
            return damage - damage * (percent / 100.0f);
        }

        // Метод для расчета урона с учетом резиста
        public static float CalculateDamageWithResistance(float damage, int resistancePercent)
        {
            // Уменьшение урона в зависимости от процента сопротивления
            return damage - (damage * resistancePercent / 100.0f);
        }

        // Метод для преобразования урона, нанесенного танку, в корректные значения
        public static float TransformDamageToTank(float hullHp, float damage)
        {
            // Преобразование входного урона в значение в рамках здоровья корпуса
            return 10000.0f / (hullHp / damage);
        }

        // Тестовый метод для расчета остаточного здоровья танка
        public static int CalculateHealthTest(int hp, int hullHp, float damage)
        {
            float calculatedDamage = 10000.0f / (hullHp / damage);
            return (int)(hp - calculatedDamage);
        }

    }
}
