using Pathfinding;
using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterZombieAnimator : MonoBehaviour
    {
        private static readonly int MoveHash = Animator.StringToHash("Move");
        private static readonly int DamageHash = Animator.StringToHash("Damage");

        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private Animator m_Animator;

        private Animator Animator
        {
            get
            {
                if (!m_Animator)
                    m_Animator = GetComponent<Animator>();
                return m_Animator;
            }
        }

        private IAstarAI m_Path;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Path = GetComponentInParent<IAstarAI>();
            var Health = GetComponentInParent<HealthData>();

            if (Health)
            {
                Health.onHealthChanged += (data) =>
                {
                    if (data.IsDamage)
                    {
                        m_Animator.SetTrigger(DamageHash);
                    }
                };
            }
        }

        public event Action onAttackStarted = delegate { };
        public event Action onAttackEnded = delegate { };
        public event Action onEnableCollider = delegate { };
        public event Action onDisableCollider = delegate { };

        private void Update()
        {
            Animator.SetBool(MoveHash, !m_Path.reachedEndOfPath);
        }

        public void Attack()
        {
            Animator.SetTrigger(AttackHash);
        }

        public void AttackStarted()
        {
            onAttackStarted.Invoke();
        }

        public void AttackEnded()
        {
            onAttackEnded.Invoke();
        }

        public void EnableCollider()
        {
            onEnableCollider.Invoke();
        }

        public void DisableCollider()
        {
            onDisableCollider.Invoke();
        }

        internal void SetMoveSpeed(float speed)
        {
            Animator.SetFloat("MoveSpeed", speed);
        }
    }
}