using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.tanks.data
{
    public class DamageTankData
    {
        public float Damage { get; set; }
        public long TimeDamage { get; set; }
        public BattlefieldPlayerController Damager { get; set; }

    }
}