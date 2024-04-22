using Prototype;
using UnityEngine;

public class EmptyState : IState
{
    public bool IsActive { get; set; }
}
public class MeleeRangeStateMachine : MonoStateMachine
{
    private MeleeAttackBehaviour m_MeleeAttack;
    private CharacterGunBehaviourV2 m_RangeState;
    private EmptyState m_EmptyState;
    private void Awake()
    {
        m_MeleeAttack = GetComponent<MeleeAttackBehaviour>();
        m_RangeState = GetComponent<CharacterGunBehaviourV2>();
        m_EmptyState = new EmptyState();

        m_RangeState.IsActive = false;
        ChangeState(m_MeleeAttack);
    }

    protected void Update()
    {
        m_RangeState.CheckRangeTarget();
        m_MeleeAttack.UpdateCondition();

        var canMeleeAttack = m_MeleeAttack.CanAttack();
        var canRangeAttack = m_RangeState.CanAttack();

        Debug.Log($"canMeleeAttack {canMeleeAttack}");
        Debug.Log($"canRangeAttack {canRangeAttack}");

        if (canRangeAttack)
        {
            ChangeState(m_RangeState);
        }
        else if (canMeleeAttack)
        {
            ChangeState(m_MeleeAttack);
        }
        else
        {
            ChangeState(m_EmptyState);
        }
    }
}
