using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.mine
{
    public class ServerMine
    {
        private string id;
        private Vector3 position;
        private BattlefieldPlayerController owner;

        public string GetId()
        {
            return id;
        }

        public void SetId(string id)
        {
            this.id = id;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public BattlefieldPlayerController GetOwner()
        {
            return owner;
        }

        public void SetOwner(BattlefieldPlayerController owner)
        {
            this.owner = owner;
        }
    }
}