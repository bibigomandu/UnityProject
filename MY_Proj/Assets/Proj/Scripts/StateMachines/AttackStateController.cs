using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proj.CharacterControls.States;

namespace Proj.StateMachines
{
    public class AttackStateController : MonoBehaviour
    {
        public delegate void OnEnterAttackState();
        public delegate void OnExitAttackState();
        public OnEnterAttackState enterAttackStateHandler;
        public OnExitAttackState exitAttackStateHandler;

        public bool IsInAttackState
        {
            get;
            private set;
        }

        void Start()
        {
            enterAttackStateHandler = new OnEnterAttackState(EnterAttackState);
            exitAttackStateHandler = new OnExitAttackState(ExitAttackState);
        }

        public void OnStartOfAttackState()
        {
            IsInAttackState = true;
            enterAttackStateHandler();
        }

        public void OnEndOfAttackState()
        {
            IsInAttackState = false;
            exitAttackStateHandler();
        }

        private void EnterAttackState()
        {
            //
        }

        private void ExitAttackState()
        {
            //
        }

        // Add an event that calls this method to animations.
        public void OnCheckAttackCollider(int attackIndex)
        {
            GetComponent<IAttackable>()?.OnExecuteAttack(attackIndex);
        }
    }
}