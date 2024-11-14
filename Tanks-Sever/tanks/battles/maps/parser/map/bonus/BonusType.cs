using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.battles.maps.parser.map.bonus
{
    public class BonusType
    {
        public static readonly BonusType NITRO = new BonusType();
        public static readonly BonusType DAMAGE = new BonusType();
        public static readonly BonusType ARMOR = new BonusType();
        public static readonly BonusType HEAL = new BonusType();
        public static readonly BonusType CRYSTALL = new BonusType();
        public static readonly BonusType CRYSTALL_100 = new BonusType();

        private readonly string value;



        public string GetValue()
        {
            return value;
        }

        public static BonusType GetType(string value)
        {
            return value switch
            {
                "medkit" => HEAL,
                "armorup" => ARMOR,
                "damageup" => DAMAGE,
                "nitro" => NITRO,
                "crystal" => CRYSTALL,
                "crystal_100" => CRYSTALL_100,
                _ => null,
            };
        }
    }
}