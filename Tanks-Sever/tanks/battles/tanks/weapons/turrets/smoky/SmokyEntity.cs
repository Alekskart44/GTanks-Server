using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.weapons.data;

namespace Tanks_Sever.tanks.battles.tanks.weapons.turrets.smoky
{
    public class SmokyEntity : IEntity
    {
        public float DamageMin { get; private set; } // Минимальный урон
        public float DamageMax { get; private set; } // Максимальный урон
        private readonly ShotData shotData; // Данные о выстреле
        public readonly EntityType Type; // Тип сущности

        public SmokyEntity(ShotData shotData, float damageMin, float damageMax)
        {
            Type = EntityType.SMOKY; // Установка типа
            DamageMin = damageMin; // Установка минимального урона
            DamageMax = damageMax; // Установка максимального урона
            this.shotData = shotData; // Установка данных о выстреле
        }

        // Реализация свойств интерфейса IEntity
        public int ChargingTime => 0;
        public int WeakeningCoeff => 0;

        // Метод для получения данных о выстреле
        public ShotData GetShotData()
        {
            return shotData;
        }

        public EntityType GetType()
        {
            return Type;
        }

        public override string ToString()
        {
            return $"SmokyEntity: DamageMin = {DamageMin}, DamageMax = {DamageMax}, Type = {Type}";
        }
    }
}