using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls;

namespace Proj.StateMachines
{
    public class IdleState : State<EnemyControllerLich>
    {
        private Animator animator;
        private CharacterController controller;
        private bool isPatrol = false;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        protected int hashMove = Animator.StringToHash("Move");
        protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashMove, false);
            animator?.SetFloat(hashMoveSpeed, 0);
            controller?.Move(Vector3.zero);
            isPatrol = true;
            idleTime = Random.Range(minIdleTime, maxIdleTime);
        }

        public override void PreUpdate()
        {
            //
        }

        public override void Update(float deltaTime)
        {
            // enemy의 enemy. 즉, player.
            Transform enemy = context.SearchEnemy();
            
            if(enemy)
            {
                if(context.IsAvailableAttack)
                {
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else if(isPatrol && stateMachine.ElapsedTimeInState > idleTime)
            {
                stateMachine.ChangeState<PatrolState>();
            }
        }

        public override void OnExit()
        {
            //
        }

        public override string GetStateName()
        {
            return "IDLE";
        }
    }
}