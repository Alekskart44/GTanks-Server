using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.weapons.data;

namespace Tanks_Sever.tanks.battles.tanks.weapons
{
    public interface IEntity
    {
        int ChargingTime { get; } // Зарядное время
        int WeakeningCoeff { get; } // Коэффициент ослабления
        float DamageMin { get; } // Минимальный урон
        float DamageMax { get; } // Максимальный урон

        // Метод для получения данных выстрела
        ShotData GetShotData();

        // Метод для получения типа сущности
        EntityType GetType();

        // Метод для преобразования объекта в строку
        string ToString();
    }
}