using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.weapons.data;
using Tanks_Sever.tanks.utils;

namespace Tanks_Sever.tanks.battles.tanks.weapons.turrets.smoky
{
    public class SmokyModel : IWeapon
    {
        private BattlefieldModel bfModel;
        private BattlefieldPlayerController player;
        private SmokyEntity entity;
        private WeaponWeakeningData weakeningData;

        public SmokyModel(SmokyEntity entity, WeaponWeakeningData weakeningData, BattlefieldModel bfModel, BattlefieldPlayerController player)
        {
            this.entity = entity;
            this.bfModel = bfModel;
            this.player = player;
            this.weakeningData = weakeningData;
        }

        public void Fire(string json)
        {
            var jo = System.Text.Json.JsonDocument.Parse(json).RootElement;

            bfModel.Fire(player, json);
            var victim = bfModel.Players[jo.GetProperty("victimId").GetString()] as BattlefieldPlayerController;
            if (victim != null)
            {
                OnTarget(new[] { victim }, (int)jo.GetProperty("distance").GetDouble());
            }
        }

        public void StartFire(string json)
        {
        }

        public void OnTarget(BattlefieldPlayerController[] targetsTanks, int distance)
        {
            if (targetsTanks.Length != 0)
            {
                if (targetsTanks.Length > 1)
                {
                    Console.WriteLine("SmokyModel::OnTarget() Warning! targetsTanks length = " + targetsTanks.Length);
                }

                float damage = RandomUtils.GetRandom(entity.DamageMin, entity.DamageMax);
                if (distance >= weakeningData.MinimumDamageRadius)
                {
                    damage = WeaponUtils.CalculateDamageFromDistance(damage, (int)weakeningData.MinimumDamagePercent);
                }

                bfModel.tanksKillModel.DamageTank(targetsTanks[0], player, damage, true);
            }
        }

        public IEntity GetEntity()
        {
            return entity;
        }

        public void StopFire()
        {
        }
    }
}