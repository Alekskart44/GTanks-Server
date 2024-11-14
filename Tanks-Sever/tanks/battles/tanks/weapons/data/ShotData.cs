using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.tanks.weapons.data
{
    public class ShotData
    {
        public double AutoAimingAngleDown { get; set; }
        public double AutoAimingAngleUp { get; set; }
        public int NumRaysDown { get; set; }
        public int NumRaysUp { get; set; }
        public int ReloadMsec { get; set; }
        public float ImpactCoeff { get; set; }
        public float Kickback { get; set; }
        public float TurretRotationAccel { get; set; }
        public float TurretRotationSpeed { get; set; }
        public string Id { get; set; }

        public ShotData(string id, double autoAimingAngleDown, double autoAimingAngleUp, int numRaysDown, int numRaysUp, int reloadMsec, float impactCoeff, float kickback, float turretRotationAccel, float turretRotationSpeed)
        {
            Id = id;
            AutoAimingAngleDown = autoAimingAngleDown;
            AutoAimingAngleUp = autoAimingAngleUp;
            NumRaysDown = numRaysDown;
            NumRaysUp = numRaysUp;
            ReloadMsec = reloadMsec;
            ImpactCoeff = impactCoeff;
            Kickback = kickback;
            TurretRotationAccel = turretRotationAccel;
            TurretRotationSpeed = turretRotationSpeed;
        }
    }
}
