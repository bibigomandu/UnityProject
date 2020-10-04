using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Proj.CharacterControls.AttackBehaviours;
using Proj.CharacterControls.States;
using Proj.StateMachines;
using Proj.UIs;

namespace Proj.CharacterControls {
    public class PlayerControl : MonoBehaviour, IAttackable, IDamagable {
#region
        public List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();
        private Camera mainCamera;
        public Animator animator;
        public LayerMask targetMask;
        private Transform hitPoint;
        private Rigidbody rigidbody_;
        public GameObject HPBarObj;
        private HPBarControl HPBar;

        bool isOnUI; // UI위에 마우스 커서가 위치했는지.
        public float maxHP = 100.0f;
        public float HP;
        [Range(15f, 30f)]
        public float moveSpeed = 20f;
        public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;
#endregion

#region     Animator Hashes
        readonly int moveHash           = Animator.StringToHash("Move");
        readonly int fallingHash        = Animator.StringToHash("Falling");
        readonly int attackTriggerHash  = Animator.StringToHash("AttackTrigger");
        readonly int attackIndexHash    = Animator.StringToHash("AttackIndex");
        readonly int hitTriggerHash     = Animator.StringToHash("HitTrigger");
        readonly int isAliveHash        = Animator.StringToHash("IsAlive");
#endregion  Animator Hashes

#region     Main Methods
        void Start() {
            rigidbody_ = GetComponent<Rigidbody>();
            mainCamera = Camera.main;
            HP = maxHP;
            HPBar = HPBarObj.GetComponent<HPBarControl>();

            InitHPBar();
            InitAttackBehaviour();
        }

        void Update() {
            // 중력.
            // Rigidbody가 있음에도 중력이 적용이 안 되는 기현상.
            rigidbody_.AddForce(Vector3.down * 500f, ForceMode.Acceleration);
            if(!IsAlive) return;

            CheckOnUI();

            if(CheckAttackInput()) {
                Stop();

                if(Input.GetMouseButtonDown(0) && attackBehaviours[0].IsAvailable) {
                    Attack(0); // Left button.
                } else if(Input.GetMouseButtonDown(1) && attackBehaviours[1].IsAvailable) {
                    Attack(1); // Right button.
                }
            } else if(CheckMoveInput() && !IsInAttackState)
                Move();
            else
                Stop();
        }
#endregion  Main Methods

#region     Helper Methods
        private void CheckOnUI() {
            isOnUI = EventSystem.current.IsPointerOverGameObject();
        }

        private void InitAttackBehaviour() {
            foreach(AttackBehaviour behaviour in attackBehaviours)
                behaviour.targetMask = targetMask;
        }

        private bool CheckMoveInput() {
            if( Input.GetAxis("Horizontal") != 0 ||
                Input.GetAxis("Vertical") != 0)
                return true;
            else
                return false;
        }

        private void Move() {
            Vector3 dir = Vector3.zero;

            dir = mainCamera.transform.forward * Input.GetAxis("Vertical") +
                mainCamera.transform.right * Input.GetAxis("Horizontal");
            dir.y = 0;
            dir = dir.normalized;

            animator.SetBool(moveHash, true);
            transform.rotation = Quaternion.LookRotation(dir);
            rigidbody_.MovePosition(transform.position + dir * moveSpeed * Time.deltaTime);
        }

        private void Stop() {
            animator.SetBool(moveHash, false);
            rigidbody_.velocity = Vector3.zero;
        }

        private bool CheckAttackInput() {
            // UI위에 커서가 위치하면 공격이 나가지 않도록.
            if( (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) &&
                !IsInAttackState && !isOnUI)
                return true;
            else
                return false;
        }

        private void Attack(int attackIndex) {
            animator.SetInteger(attackIndexHash, attackIndex);
            animator.SetTrigger(attackTriggerHash);
        }

        private void InitHPBar() {
            if(HPBar != null) {
                HPBar.SetMaxValue(maxHP);
                HPBar.SetValue(HP);
                Debug.Log("Get Max HP : " + HPBar.GetMaxValue());
                Debug.Log("Get HP : " + HPBar.GetValue());
            }
        }

        private void SetHPBar() {
            if(HPBar != null) {
                HPBar.SetValue(HP);
                Debug.Log("Get HP : " + HPBar.GetValue());
            }
        }
#endregion  Helper Methods

#region     IAttackable Interfaces
        public AttackBehaviour CurrentAttackBehaviour {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex) {
            attackBehaviours[attackIndex].ExecuteAttack();
        }
#endregion  IAttackable Ingerfaces

#region     IDamagable Interfaces
        public bool IsAlive => HP > 0;

        public void TakeDamage(int damage, GameObject damageEffectPrefab) {
            Debug.Log("Player TakeDamage : " + damage);

            if(!IsAlive) return;

            HP -= damage;

            if(damageEffectPrefab != null)
                Instantiate<GameObject>(damageEffectPrefab, hitPoint);

            if(IsAlive)
                animator?.SetTrigger(hitTriggerHash);
            else
                animator?.SetBool(isAliveHash, false);

            SetHPBar();
        }
#endregion  IDamagable Interfaces
    } // class PlayerControl
} // namespace