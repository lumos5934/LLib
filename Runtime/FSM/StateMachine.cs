using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace LLib
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IState> _stateDict = new();

        public IState CurState { get; private set; }

        public event UnityAction<IState> OnExit;
        public event UnityAction<IState> OnEnter;

        public void Register(IState state)
        {
            _stateDict[state.GetType()] = state;
        }

        public void Update()
        {
            CurState?.Update();
        }

        public void ChangeState<T>() where T : IState
        {
            if (!_stateDict.TryGetValue(typeof(T), out var newState) ||
                CurState == newState) return;

            var prevState = CurState;

            if (prevState != null)
            {
                prevState.Exit();
                OnExit?.Invoke(prevState);
            }

            CurState = newState;
            CurState.Enter();
            OnEnter?.Invoke(CurState);
        }
    }
}