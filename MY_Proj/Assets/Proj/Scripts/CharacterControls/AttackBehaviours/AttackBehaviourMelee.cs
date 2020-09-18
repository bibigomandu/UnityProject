using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls;
using Proj.CharacterControls.States;

namespace Proj.CharacterControls.AttackBehaviours
{
    public class AttackBehaviourMelee : AttackBehaviour
    {
        public ManualCollision attackCollision;

        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            Collider[] colliders = attackCollision?.CheckOverlapBox(targetMask);

            foreach(Collider col in colliders)
            {
                col.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage, effectPrefab);
            }

            calcCoolTime = 0.0f;
        }
    }
}