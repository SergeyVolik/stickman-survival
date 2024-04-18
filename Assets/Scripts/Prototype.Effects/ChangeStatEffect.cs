using UnityEngine;

namespace Prototype
{
    public class ChangeStatEffect : MonoBehaviour, IEffectExtention
    {
        public float increaseAttackSpeedMult;
        public float increaseMeleeWeaponDamageMult;
        private CharacterStats m_stats;

        public void Clear()
        {
            m_stats.attackSpeedMult -= increaseAttackSpeedMult;
            m_stats.meleeWeaponDamageMult -= increaseMeleeWeaponDamageMult;
        }

        public void Setup(GameObject target)
        {
            m_stats = target.GetComponent<CharacterStats>();
            m_stats.attackSpeedMult += increaseAttackSpeedMult;
            m_stats.meleeWeaponDamageMult += increaseMeleeWeaponDamageMult;
        }
    }
}
