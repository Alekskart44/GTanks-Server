using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.colormaps;
using Tanks_Sever.tanks.battles.tanks.hulls;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.battles.tanks.data;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.tanks.weapons;

namespace Tanks_Sever.tanks.battles.tanks
{
    public class Tank
    {
        public const int MAX_HEALTH_TANK = 10000;
        public Vector3 Position { get; set; }
        public Vector3 Orientation { get; set; }
        public Vector3 LinVel { get; set; }
        public Vector3 AngVel { get; set; }
        public double TurretDir { get; set; }
        public int ControllBits { get; set; }
        private IWeapon _weapon;
        private Hull _hull;
        private Colormap _colormap;
        public string Id { get; set; }
        public float Speed { get; set; }
        public float TurnSpeed { get; set; }
        public float TurretRotationSpeed { get; set; }
        public int Health { get; set; } = 10000;
        public int IncrationId { get; set; }
        public string State { get; set; } = "active";
        //public FrezeeEffectModel FrezeeEffect { get; set; }
        public List<Effect> ActiveEffects { get; set; }
        public Dictionary<BattlefieldPlayerController, DamageTankData> LastDamagers { get; set; }

        public Tank(Vector3 position)
        {
            Position = position;
            ActiveEffects = new List<Effect>();
            LastDamagers = new Dictionary<BattlefieldPlayerController, DamageTankData>();
        }

        public IWeapon GetWeapon()
        {
            return _weapon;
        }

        public Hull GetHull()
        {
            return _hull;
        }

        public void SetWeapon(IWeapon weapon)
        {
              _weapon = weapon;
              TurretRotationSpeed = weapon.GetEntity().GetShotData().TurretRotationSpeed;
        }

        public void SetHull(Hull hull)
        {
            _hull = hull;
            Speed = hull.Speed;
            TurnSpeed = hull.TurnSpeed;
        }

        public string Dump()
        {
            return $"-------TANK DUMP-------\n\t\ttank id: {Id}\n\t\thealth: {Health}\n\t\tweapon: {WeaponToString()}\n\t\thull: {HullToString()}\n\t\tstate: {State}";
        }

        private string WeaponToString()
        {
            return _weapon != null ? _weapon.ToString() : "None";
        }
        private string HullToString()
        {
            return _hull != null ? _hull.ToString() : "None";
        }

        public Colormap GetColormap()
        {
            return _colormap;
        }

        public void SetColormap(Colormap colormap)
        {
            _colormap = colormap;
        }

        public bool IsUsedEffect(EffectType type)
        {
            foreach (var effect in ActiveEffects)
            {
                if (effect.GetEffectType() == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}