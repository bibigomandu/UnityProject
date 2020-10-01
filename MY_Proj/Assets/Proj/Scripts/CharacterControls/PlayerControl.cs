using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Proj.CharacterControls.AttackBehaviours;
using Proj.CharacterControls.States;
using Proj.StateMachines;

namespace Proj.CharacterControls
{
    public class PlayerControl : MonoBehaviour, IAttackable, IDamagable
    {
#region
        public List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();
        private CharacterController characterController;
        private NavMeshAgent agent;
        private Camera mainCamera;
        public Animator animator;
        public LayerMask groundLayerMask;
        public LayerMask targetMask;
        public Transform target;
        private Transform hitPoint;
        public GameObject mouseEffectPrefab;

        public float maxHp = 100.0f;
        public float hp;
        public float mouseEffectTime = 0.2f;
        public float defaultStoppingDistance = 0.1f;
        public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;
#endregion

#region     Animator Hashes
        readonly int moveHash           = Animator.StringToHash("Move");
        readonly int moveSpeedHash      = Animator.StringToHash("MoveSpeed");
        readonly int fallingHash        = Animator.StringToHash("Falling");
        readonly int attackTriggerHash  = Animator.StringToHash("AttackTrigger");
        readonly int attackIndexHash    = Animator.StringToHash("AttackIndex");
        readonly int hitTriggerHash     = Animator.StringToHash("HitTrigger");
        readonly int isAliveHash        = Animator.StringToHash("IsAlive");
#endregion  Animator Hashes

#region     Main Methods
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();

            agent.updatePosition = false;
            agent.updateRotation = true;

            mainCamera = Camera.main;

            hp = maxHp;

            InitAttackBehaviour();
        }

        // Update is called once per frame
        void Update()
        {
            if(!IsAlive)
            {
                return;
            }

            CheckAttackBehaviour();

            // Left button.
            if(Input.GetMouseButtonDown(0) && !IsInAttackState)
            {
                // Get world position corresponding to mouse pointer.
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 300, groundLayerMask))
                {
                    RemoveTarget();

                    agent.SetDestination(hit.point);

                    if(mouseEffectPrefab != null)
                    {
                        GameObject attackEffectGO = GameObject.Instantiate<GameObject>(mouseEffectPrefab, hit.point, Quaternion.identity);
                        Destroy(attackEffectGO, mouseEffectTime);
                    }
                }
            }
            else if(Input.GetMouseButtonDown(1)) // Right button.
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 300))
                {
                    IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                    if(damagable != null && damagable.IsAlive)
                    {
                        SetTarget(hit.collider.transform);
                    }
                }
            }

            if(target != null)
            {
                if(!(target.GetComponent<IDamagable>()?.IsAlive ?? false))
                {
                    RemoveTarget();
                }
                else
                {
                    agent.SetDestination(target.position);
                    FaceToTarget();
                }
            }

            // Animation 제어.
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                characterController.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                animator.SetBool(moveHash, true);
            }
            else
            {
                characterController.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, 0);
                animator.SetBool(moveHash, false);
                agent.ResetPath();
            }

            if (agent.isOnOffMeshLink)
            {
                animator.SetBool(fallingHash, agent.velocity.y != 0.0f);
            }
            else
            {
                animator.SetBool(fallingHash, false);
            }

            AttackTarget();
        }

        private void OnAnimatorMove()
        {
            /*
            Callback for processing animation movements for modifying
            root motion.

            This callback will be invoked at each frame after
            the state machines and the animations have been evaluated,
            but before OnAnimatorIK.
            */
            animator.rootPosition = agent.nextPosition;
            transform.position = agent.nextPosition;
        }
#endregion  Main Methods

#region     Helper Methods
        private void InitAttackBehaviour()
        {
            foreach(AttackBehaviour behaviour in attackBehaviours)
            {
                behaviour.targetMask = targetMask;
            }
        }

        private void CheckAttackBehaviour()
        {
            if(CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;
                foreach(AttackBehaviour behaviour in attackBehaviours)
                {
                    if(behaviour.IsAvailable)
                    {
                        if( (CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }

        void SetTarget(Transform newTarget)
        {
            target = newTarget;

            agent.stoppingDistance = CurrentAttackBehaviour?.range ?? defaultStoppingDistance;
            agent.updateRotation = false;
            agent.SetDestination(newTarget.transform.position);
        }

        void RemoveTarget()
        {
            target = null;

            agent.stoppingDistance = defaultStoppingDistance;
            agent.updateRotation = true;
        }

        void AttackTarget()
        {
            if(CurrentAttackBehaviour == null)
            {
                return;
            }

            if(target != null && !IsInAttackState && CurrentAttackBehaviour.IsAvailable)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if(distance <= CurrentAttackBehaviour?.range)
                {
                    animator.SetInteger(attackIndexHash, CurrentAttackBehaviour.animationIndex);
                    animator.SetTrigger(attackTriggerHash);
                }
            }
        }

        void FaceToTarget()
        {
            if(target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                //Debug.Log("x : " + direction.x + " | z : " + direction.z);
                if(direction.x != 0 && direction.z != 0)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
                }
            }
        }
#endregion  Helper Methods

#region     IAttackable Interfaces
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if(CurrentAttackBehaviour != null && target != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(target.gameObject);
            }
        }
#endregion  IAttackable Ingerfaces

#region     IDamagable Interfaces
        public bool IsAlive => hp > 0;

        public void TakeDamage(int damage, GameObject damageEffectPrefab)
        {
            Debug.Log("Player TakeDamage : " + damage);
            if(!IsAlive)
            {
                return;
            }

            hp -= damage;

            if(damageEffectPrefab != null)
            {
                Instantiate<GameObject>(damageEffectPrefab, hitPoint);
            }

            if(IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                animator?.SetBool(isAliveHash, false);
            }
        }
#endregion  IDamagable Interfaces
    }
}