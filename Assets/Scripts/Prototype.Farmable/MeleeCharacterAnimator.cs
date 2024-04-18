using System;
using UnityEngine;

namespace Prototype
{
    public interface IMeleeCharacterAnimator
    {
        public void OnBeginAttack();
        public void OnEndAttack();
        public void OnEnableHitBox();
        public void OnDisableHitBox();

        public event Action onBeginAttack;
        public event Action onEndAttack;
        public event Action onEnableHitBox;
        public event Action onDisableHitBox;
    }

    public class MeleeCharacterAnimator : MonoBehaviour, IMeleeCharacterAnimator
    {
        private Animator m_Animator;

        private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
        private static readonly int AttackSpeedHash = Animator.StringToHash("MeleeAttackSpeed");
        private static readonly int IsAttackingBoolHash = Animator.StringToHash("IsAttacking");
   
        public event Action onBeginAttack = delegate { };
        public event Action onEndAttack = delegate { };
        public event Action onEnableHitBox = delegate { };
        public event Action onDisableHitBox = delegate { };

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void SetAttackSpeed(float attackSpeed)
        {
            m_Animator.SetFloat(AttackSpeedHash, attackSpeed);
        }

        public void AttackTrigger()
        {
            if (!m_Animator.GetBool(IsAttackingBoolHash))
            {
                m_Animator.SetTrigger(AttackTriggerHash);
                m_Animator.SetBool(IsAttackingBoolHash, true);
            }
        }

        public void ResetAttack()
        {
            m_Animator.ResetTrigger(AttackTriggerHash);
            m_Animator.SetBool(IsAttackingBoolHash, false);
        }

        public void OnBeginAttack()
        {
            onBeginAttack.Invoke();
            //m_Animator.SetBool(IsAttackingBoolHash, true);
        }

        public void OnEndAttack()
        {
            onEndAttack.Invoke();
            //m_Animator.SetBool(IsAttackingBoolHash, false);
        }

        public void OnEnableHitBox()
        {
            onEnableHitBox.Invoke();
        }

        public void OnDisableHitBox()
        {
            onDisableHitBox.Invoke();
        }
    }
}
