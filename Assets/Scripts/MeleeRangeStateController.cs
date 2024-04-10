using Prototype;
using UnityEngine;

public class MeleeRangeStateController : MonoBehaviour
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

        //m_MeleeAttack.enabled = true;
        //m_GunBehaviour.enabled = false;
    }

    private void Update()
    {
        //m_MeleeAttack.enabled = !m_Inventory.GunIsActive();
    }
}
