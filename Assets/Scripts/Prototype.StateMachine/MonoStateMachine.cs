using UnityEngine;

namespace Prototype
{
    public abstract class MonoStateMachine : MonoBehaviour, IStateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState state)
        {
            if (CurrentState == state)
                return;

            if (CurrentState != null)
            {
                CurrentState.IsActive = false;
            }
            
            if (state != null)
            {
                state.IsActive = true;
            }

            CurrentState = state;            
        }
    }
}