using Prototype;
using UnityEngine;

public class MeleeRangeStateController : MonoBehaviour
{
    private MeleeAttackBehaviour m_MeleeAttack;
    private CustomCharacterController m_Controller;
    private CharacterGunBehaviourV2 m_GunBehaviour;

    private void Awake()
    {
        m_MeleeAttack = GetComponent<MeleeAttackBehaviour>();
        m_Controller = GetComponent<CustomCharacterController>();
        m_GunBehaviour = GetComponent<CharacterGunBehaviourV2>();
        var m_Combat = GetComponent<CharacterCombatState>();

        m_Combat.onCombatState += M_Combat_onCombatState;

        m_MeleeAttack.enabled = true;
        m_GunBehaviour.enabled = false;
    }

    private void M_Combat_onCombatState(bool obj)
    {
        m_MeleeAttack.enabled = !obj;
        m_GunBehaviour.enabled = obj;
    }
}
