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
using Proj.Dialogues;

namespace Proj.CharacterControls {
    public class PlayerControl : MonoBehaviour, IAttackable, IDamagable {
#region
        public List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();
        private Camera mainCamera;
        public Animator animator;
        public LayerMask targetMask;
        private Transform hitPoint;
        private Rigidbody rigidbody_;
        public BarControl HPBar;
        private Transform prevNearestNPC;
        private Transform nearestNPC;

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

            InitHPBar();
            InitAttackBehaviour();
        }

        void Update() {
            // 중력.
            // Rigidbody가 있음에도 중력이 적용이 안 되는 기현상.
            rigidbody_.AddForce(Vector3.down * 500f, ForceMode.Acceleration);

            if(CheckDialogueON()) return;
            if(!IsAlive) return;

            CheckOnUI(); // 마우스 커서가 UI위에 있는지 확인.

            if(CheckNPC()) return; // 대화를 시작하면 이후는 무시.

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
                HPBar = HPBar.GetComponent<BarControl>();

                if(HPBar != null) {
                    HPBar.MinimumValue = 0;
                    HPBar.MaximumValue = maxHP;
                    HPBar.value = HP;
                }
            }
        }

        private void SetHPBar() {
            if(HPBar != null) {
                HPBar.value = HP;
            }
        }

        private bool CheckNPC() {
            nearestNPC = null;
            float dist = 0;
		    Collider[] colls = Physics.OverlapSphere (transform.position, 3.0f);

            // 가장 가까운 상호작용 가능한 NPC를 찾는다.
            foreach(Collider coll in colls) {
                if(coll.CompareTag("InteractableNPC")) {
                    if(nearestNPC == null) {
                        dist = Vector3.Distance(transform.position, coll.transform.position);
                        nearestNPC = coll.transform;
                    } else {
                        if(Vector3.Distance(transform.position, coll.transform.position) < dist) {
                            nearestNPC = coll.transform;
                            dist = Vector3.Distance(transform.position, coll.transform.position);
                        }
                    }
                }
            }

            // 대상이 변경되면 새로 버튼을 생성.
            if(prevNearestNPC != nearestNPC) {
                prevNearestNPC = nearestNPC;
                if(nearestNPC != null) nearestNPC.GetComponent<DialogueControl>().ShowDialogueBtn();
            }

            // 대화시작.
            if(nearestNPC != null) {
                if(Input.GetKeyDown(KeyCode.F)) {
                    nearestNPC.GetComponent<DialogueControl>().OpenDialogueUI();
                    return true;
                }
            }

            return false;
        }

        private bool CheckDialogueON() {
            GameObject obj = GameObject.FindWithTag("DialogueUI");

            if(obj != null) return true;
            else            return false;
        }
        //
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