using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj.StateMachines {
    public abstract class State<T> {
        //protected int mecanimStateHash;
        protected StateMachine<T> stateMachine;
        protected T context;

        public State() {}

        internal void SetMachineAndContext(StateMachine<T> stateMachine, T context) {
            this.stateMachine = stateMachine;
            this.context = context;

            OnInitialized();
        }

        public virtual void OnInitialized() {}

        public virtual void OnEnter() {}

        public virtual void PreUpdate() {}

        public abstract void Update(float deltaTime);

        public virtual void OnExit() {}

        public abstract string GetStateName();
    } // class State<T>

    public sealed class StateMachine<T> {
        private T context;
        private State<T> currentState;
        private State<T> CurrentState => currentState;
        private State<T> previousState;
        public State<T> PreviousState => previousState;
        private float elapsedTimeInState = 0.0f;
        public float ElapsedTimeInState => elapsedTimeInState;
        private Dictionary<System.Type, State<T>> states = new Dictionary<Type, State<T>>();

        public StateMachine(T context, State<T> initialState) {
            this.context = context;

            AddState(initialState);
            currentState = initialState;
            currentState.OnEnter();
        }

        public void AddState(State<T> state) {
            state.SetMachineAndContext(this, context);
            states[state.GetType()] = state;
        }

        public void Update(float deltaTime) {
            elapsedTimeInState += deltaTime;

            currentState.PreUpdate();
            currentState.Update(deltaTime);

            // Debug.Log(GetCurrentStateName() + " : " + elapsedTimeInState);
        }
        public R ChangeState<R>() where R : State<T> {
            var newType = typeof(R);
            if(currentState.GetType() == newType)
                return currentState as R;

            if(currentState != null)
                currentState.OnExit();

            previousState = currentState;
            currentState = states[newType];
            currentState.OnEnter();
            elapsedTimeInState = 0.0f;

            return currentState as R;
        }

        public string GetCurrentStateName() {
            if(currentState != null)
                return currentState.GetStateName();
            else
                return "???";
        }
    } // class StateMachine
} // namespace