using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Proj.CharacterControls;

namespace Proj.StateMachines {
    public class PatrolState : State<EnemyControllerLich> {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private EnemyControllerLich enemyController;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized() {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter() {
            if(context?.targetWaypoint == null)
                context?.FindNextWaypoint();

            if(context?.targetWaypoint != null) {
                Vector3 destination = context.targetWaypoint.position;
                agent?.SetDestination(destination);
                animator?.SetBool(hashMove, true);
            }
        }

        public override void Update(float deltaTime) {
            Transform enemy = context.SearchEnemy();

            if(enemy) {
                if(context.IsAvailableAttack)
                    stateMachine.ChangeState<AttackState>();
                else
                    stateMachine.ChangeState<MoveState>();
            } else {
                if(!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance)) {
                    context.FindNextWaypoint();
                    stateMachine.ChangeState<IdleState>();
                } else {
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, 0.1f, Time.deltaTime);
                }
            }
        }

        public override void OnExit() {
            animator?.SetBool(hashMove, false);
            agent.ResetPath();
        }

        private void FindNextWaypoint() {
            Transform targetWaypoint = context.FindNextWaypoint();
            if(targetWaypoint != null)
                agent?.SetDestination(targetWaypoint.position);
        }

        public override string GetStateName() {
            return "Patrol";
        }
    } // class PatrolState
}