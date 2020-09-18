using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.CharacterControls.AttackBehaviours
{
    public abstract class AttackBehaviour : MonoBehaviour
    {
#if UNITY_EDITOR
        [Multiline]
        public string devDescription = "description!";
#endif

        public int animationIndex;
        public int priority;
        public int damage;
        public float range = 3f;
        public float coolTime;
        public GameObject effectPrefab;
        public float calcCoolTime = 0.0f;
        public LayerMask targetMask;
        public bool IsAvailable => calcCoolTime >= coolTime;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            calcCoolTime = coolTime;
        }

        // Update is called once per frame
        void Update()
        {
            if(calcCoolTime < coolTime)
            {
                calcCoolTime += Time.deltaTime;
            }
        }

        public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
    } // class AttackBehaviour
}