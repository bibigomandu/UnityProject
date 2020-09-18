using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.Projectiles;

namespace Proj.CharacterControls.AttackBehaviours
{
    public class AttackBehaviourProjectile : AttackBehaviour
    {
        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            if(target == null)
            {
                return;
            }
            
            calcCoolTime = 0.0f;
            StartCoroutine(MakeProjectile(target, startPoint));
        }

        public IEnumerator MakeProjectile(GameObject target = null, Transform startPoint = null)
        {
            yield return new WaitForSeconds(1f);
            
            Vector3 projectilePosition = startPoint?.position ?? transform.position;
            if(effectPrefab != null)
            {
                GameObject projectileGO = GameObject.Instantiate<GameObject>(effectPrefab, projectilePosition, Quaternion.identity);
                projectileGO.transform.forward = transform.forward;

                Projectile projectile = projectileGO.GetComponent<Projectile>();
                if(projectile != null)
                {
                    projectile.owner = this.gameObject;
                    projectile.target = target;
                    projectile.attackBehaviour = this;
                }
            }
        }
    }
}