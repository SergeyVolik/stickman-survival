using Prototype;
using UnityEngine;

public interface IStateMachine
{
    public IState CurrentState { get; }
    public void ChangeState(IState state);
}

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
        else
        {
            if (state != null)
            {
                state.IsActive = true;
            }

            CurrentState = state;
        }
    }
}

public class MeleeRangeStateMachine : MonoStateMachine
{
    private MeleeAttackBehaviour m_MeleeAttack;
    private CustomCharacterController m_Controller;
    private CharacterGunBehaviourV2 m_RangeState;

    private void Awake()
    {
        m_MeleeAttack = GetComponent<MeleeAttackBehaviour>();
        m_Controller = GetComponent<CustomCharacterController>();
        m_RangeState = GetComponent<CharacterGunBehaviourV2>();

        ChangeState(m_MeleeAttack);
    }


    protected void Update()
    {
        if (m_RangeState.HasGunTarget())
        {
            ChangeState(m_MeleeAttack);
        }
        else if (m_MeleeAttack.HasTarget)
        {
            ChangeState(m_RangeState);
        }
    }
}
