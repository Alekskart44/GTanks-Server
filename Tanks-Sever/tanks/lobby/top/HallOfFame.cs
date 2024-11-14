using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.users;

namespace Tanks_Sever.tanks.lobby.top
{
    public class HallOfFame
    {
        private static readonly HallOfFame _instance = new HallOfFame();
        private List<User> _top = new List<User>(100);

        public static HallOfFame GetInstance()
        {
            return _instance;
        }

        private HallOfFame()
        {
        }

        public void AddUser(User user)
        {
            _top.Add(user);
        }

        public void RemoveUser(User user)
        {
            _top.Remove(user);
        }

        public void InitHallFromCollection(IEnumerable<User> collection)
        {
            _top = new List<User>(collection);
        }

        public List<User> GetData()
        {
            return _top;
        }
    }
}
