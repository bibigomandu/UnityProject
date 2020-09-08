using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Proj.StateMachines;
using Proj.FieldOfVisions;

namespace Proj.CharacterControls
{
    public class EnemyControllerLich : MonoBehaviour
    {
        protected StateMachine<EnemyControllerLich> stateMachine;
        public Collider weaponCollider;
        public GameObject hitEffect;
        private FieldOfVision fov;
        public float viewRadius = 5f;
        public float attackRange = 1.5f;
        public Transform[] waypoints;
        public Transform targetWaypoint = null;
        private int waypointIndex = 0;

        public Transform Target => fov?.NearestTarget;

        // Start is called before the first frame update
        void Start()
        {
            stateMachine = new StateMachine<EnemyControllerLich>(this, new IdleState());
            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new PatrolState());

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
    }
}