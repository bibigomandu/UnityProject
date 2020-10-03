using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Proj.CharacterControls;

namespace Proj.StateMachines {
    public class MoveState : State<EnemyControllerLich> {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized() {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter() {
            agent?.SetDestination(context.Target.position);
            animator?.SetBool(hashMove, true);
        }

        public override void Update(float deltaTime) {
            // enemy의 enemy. 즉, player.
            Transform enemy = context.SearchEnemy();

            if(enemy) {
                agent.SetDestination(context.Target.position);

                if(agent.remainingDistance > agent.stoppingDistance) {
                    controller.Move(agent.velocity * deltaTime);
                    animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, 1f, deltaTime);
                    return;
                }
            }

            stateMachine.ChangeState<IdleState>();
        }

        public override void OnExit() {
            animator?.SetBool(hashMove, false);
            animator?.SetFloat(hashMoveSpeed, 0f);
            agent.ResetPath();
        }

        public override string GetStateName() {
            return "MOVE";
        }
    } // class MoveState
}