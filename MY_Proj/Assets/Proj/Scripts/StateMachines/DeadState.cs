using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls;

namespace Proj.StateMachines
{
    public class DeadState : State<EnemyControllerLich>
    {
        private Animator animator;
        protected int isAliveHash = Animator.StringToHash("IsAlive");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isAliveHash, false);
        }

        public override void Update(float deltaTime)
        {
            if(stateMachine.ElapsedTimeInState > 3.0f)
            {
                GameObject.Destroy(context.gameObject);
            }
        }

        public override void OnExit()
        {
            //
        }

        public override string GetStateName()
        {
            return "DEAD";
        }
    }
}