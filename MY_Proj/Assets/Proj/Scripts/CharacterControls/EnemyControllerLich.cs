using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Proj.StateMachines;
using Proj.FieldOfVisions;
using Proj.CharacterControls.States;
using Proj.CharacterControls.AttackBehaviours;

namespace Proj.CharacterControls
{
    public class EnemyControllerLich : MonoBehaviour, IAttackable, IDamagable
    {
        public Transform projectilePoint;
        public List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();
        public Transform hitTransform;
        public int maxHealth = 100;
        public int health;
        public int Health {
            get;
            private set;
        }
        private int hitTriggerHash = Animator.StringToHash("HitTrigger");

        protected NavMeshAgent agent;
        protected Animator animator;
        protected StateMachine<EnemyControllerLich> stateMachine;
        public Collider weaponCollider;
        public GameObject hitEffect;
        private FieldOfVision fov;
        public float viewRadius = 5f;
        public float attackRange = 1.5f;
        public Transform[] waypoints;
        public Transform targetWaypoint = null;
        private int waypointIndex = 0;

        public Transform Target => fov.NearestTarget;
        public LayerMask TargetMask => fov.targetMask;

#region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            health = maxHealth;

            stateMachine = new StateMachine<EnemyControllerLich>(this, new IdleState());
            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new PatrolState());
            stateMachine.AddState(new DeadState());

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;

            animator = GetComponent<Animator>();
            fov = this.gameObject.GetComponent<FieldOfVision>();
        }

        public R ChangeState<R>() where R : State<EnemyControllerLich>
        {
            return stateMachine.ChangeState<R>();
        }

        // Update is called once per frame
        // 에너미의 행동은 각 State의 Update에서 처리하고 이 스크립트에서는 현 상태의 Update 함수를 호출.
        void Update()
        {
            CheckAttackBehaviour();

            float elapsedTime = Time.deltaTime;
            stateMachine.Update(elapsedTime);

            // 현재 상태를 출력.
            // Debug.Log("state : " + stateMachine.GetCurrentStateName());
        }

        public bool IsAvailableAttack
        {
            get
            {
                if(!Target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.position);

                return (distance <= attackRange);
            }
        }

        public Transform SearchEnemy()
        {
            return Target;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public Transform FindNextWaypoint()
        {
            targetWaypoint = null;
            
            if(waypoints.Length > 0)
            {
                targetWaypoint = waypoints[waypointIndex];
            }

            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            
            return targetWaypoint;
        }
#endregion Unity Methods

#region Helper Methods
        private void InitAttackBehaviour() {
            foreach (AttackBehaviour behaviour in attackBehaviours) {
                if(CurrentAttackBehaviour == null) {
                    CurrentAttackBehaviour = behaviour;
                }

                behaviour.targetMask = TargetMask;
            }
        }

        private void CheckAttackBehaviour() {
            CurrentAttackBehaviour = null;

            foreach(AttackBehaviour behaviour in attackBehaviours)
            {
                if(behaviour.IsAvailable)
                {
                    if(CurrentAttackBehaviour == null || CurrentAttackBehaviour.priority < behaviour.priority) {
                        CurrentAttackBehaviour = behaviour;
                    }
                }
            }
        }
#endregion Helper Methods

#region IAttackable Interfaces
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if(CurrentAttackBehaviour != null && Target != null)
            {
                projectilePoint = transform;
                CurrentAttackBehaviour.ExecuteAttack(Target.gameObject, projectilePoint);
            }
        }
#endregion IAttackable Interfaces

#region IDamagable Interfaces
        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab) {
            if(!IsAlive)    return;

            health -= damage;

            if(hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitTransform);
            }

            if(IsAlive) {
                animator?.SetTrigger(hitTriggerHash);
            }
            else 
            {
                stateMachine.ChangeState<DeadState>();
            }
        }
#endregion IDamagable Interfaces
    }
}