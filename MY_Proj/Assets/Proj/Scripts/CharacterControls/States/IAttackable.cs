using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls.AttackBehaviours;

namespace Proj.CharacterControls.States
{
    public interface IAttackable
    {
        AttackBehaviour CurrentAttackBehaviour
        {
            get;
        }

        void OnExecuteAttack(int attackIndex);
    }
}