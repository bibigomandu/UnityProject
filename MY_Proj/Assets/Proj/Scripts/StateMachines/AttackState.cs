using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls;
using Proj.CharacterControls.States;

namespace Proj.StateMachines {
    public class AttackState : State<EnemyControllerLich> {
        private Animator animator;
        private AttackStateController attackStateController;
        private IAttackable attackable;
        private int hashAttack = Animator.StringToHash("Attack");
        protected int hashAttackIndex = Animator.StringToHash("AttackIndex");

        public override void OnInitialized() {
            animator = context.GetComponent<Animator>();
            attackStateController = context.GetComponent<AttackStateController>();
            attackable = context.GetComponent<IAttackable>();
        }

        public override void OnEnter() {
            if(attackable == null || attackable.CurrentAttackBehaviour == null) {
                stateMachine.ChangeState<IdleState>();
                return;
            }

            attackStateController.enterAttackStateHandler += OnEnterAttackState;
            attackStateController.exitAttackStateHandler += OnExitAttackState;

            context.OnExecuteAttack(attackable.CurrentAttackBehaviour.animationIndex);

            animator?.SetInteger(hashAttackIndex, attackable.CurrentAttackBehaviour.animationIndex);
            animator?.SetTrigger(hashAttack);
        }

        public override void Update(float deltaTime) {
            //
        }

        public override void OnExit() {
            attackStateController.enterAttackStateHandler -= OnEnterAttackState;
            attackStateController.exitAttackStateHandler -= OnExitAttackState;
        }

        public void OnEnterAttackState() {
            //Debug.Log("OnEnterAttackState()");
        }

        public void OnExitAttackState() {
            //Debug.Log("OnExitAttackState()");
            stateMachine.ChangeState<IdleState>();
        }

        public override string GetStateName() {
            return "ATTACK";
        }
    } // class AttackState
}
