using Prototype;
using UnityEngine;

public class MeleeRangeStateMachine : MonoBehaviour
{
    private MeleeAttackBehaviour m_MeleeAttack;
    private CustomCharacterController m_Controller;
    private CharacterGunBehaviourV2 m_GunBehaviour;
    private CharacterInventory m_Inventory;

    private void Awake()
    {
        m_MeleeAttack = GetComponent<MeleeAttackBehaviour>();
        m_Controller = GetComponent<CustomCharacterController>();
        m_GunBehaviour = GetComponent<CharacterGunBehaviourV2>();
        m_Inventory = GetComponent<CharacterInventory>();
        var m_Combat = GetComponent<CharacterCombatState>();

    }

    private void Update()
    {
        if (m_MeleeAttack.HasTarget)
        {
            m_MeleeAttack.blockAttack = false;
            m_GunBehaviour.blockAttack = true;
        }
        else {
            m_MeleeAttack.blockAttack = true;
            m_GunBehaviour.blockAttack = false;
        }
    }
}
