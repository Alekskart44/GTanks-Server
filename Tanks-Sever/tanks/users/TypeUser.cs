using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tanks_Sever.tanks.users
{
    public enum TypeUser
    {
        DEFAULT,
        MODERATOR,
        ADMIN,
        TESTER
    }

    public static class TypeUserExtensions
    {
        public static string ToStringValue(this TypeUser typeUser)
        {
            switch (typeUser)
            {
                case TypeUser.DEFAULT:
                    return "default";
                case TypeUser.MODERATOR:
                    return "moderator";
                case TypeUser.ADMIN:
                    return "admin";
                case TypeUser.TESTER:
                    return "tester";
                default:
                    return "default";
            }
        }
    }
}
