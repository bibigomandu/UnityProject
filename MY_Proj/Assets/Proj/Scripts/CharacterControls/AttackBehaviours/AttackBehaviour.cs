using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.UIs;

namespace Proj.CharacterControls.AttackBehaviours {
    public abstract class AttackBehaviour : MonoBehaviour {
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
        public BarControl skillCoolBar;

        protected virtual void Start() {
            calcCoolTime = coolTime;
            
            InitSkillCoolBar();
        }

        // Update is called once per frame
        void Update() {
            if(calcCoolTime < coolTime)
                calcCoolTime += Time.deltaTime;
            SetSkillCoolBar();
        }

        public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
                
        private void InitSkillCoolBar() {
            if(skillCoolBar != null) {
                skillCoolBar = skillCoolBar.GetComponent<BarControl>();

                if(skillCoolBar != null) {
                    skillCoolBar.MinimumValue = 0;
                    skillCoolBar.MaximumValue = coolTime;
                    skillCoolBar.value = calcCoolTime;
                }
            }
        }
        
        private void SetSkillCoolBar() {
            if(skillCoolBar != null) {
                skillCoolBar.value = calcCoolTime;
            }
        }
    } // class AttackBehaviour
}