using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.battles.tanks.math;

namespace Tanks_Sever.tanks.battles.effect
{
    public interface Effect
    {

        void Activate(BattlefieldPlayerController playerController, bool isActive, Vector3 position);

        void Deactivate();

        EffectType GetEffectType();

        int GetID();

        int GetDurationTime();
    }
}