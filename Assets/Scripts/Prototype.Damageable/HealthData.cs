using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class HealthChangeData
    {
        public int PrevValue;
        public int CurrentValue;
        public GameObject Source;
        public bool IsDamage => CurrentValue < PrevValue;
    }

    public class HealthData : MonoBehaviour, IResurrectable, IKillable, IDamageable, IHealable
    {
        public int maxHealth = 10;
        public int currentHealth = 10;

        public event Action<HealthChangeData> onHealthChanged = delegate { };
        public event Action onDeath = delegate { };
        public UnityEvent onDeathUE;

        public event Action onResurrected = delegate { };

        public bool IsDead => currentHealth == 0;

        public GameObject KilledBy => m_KilledBy;

        [field: SerializeField]
        public bool IsDamageable
        {
            get;
            set;
        } = true;

        private GameObject m_KilledBy;
        public bool HasMaxHealth()
        {
            return currentHealth == maxHealth;
        }

        public void Resurrect()
        {
            DoHeal(maxHealth, null);
            onResurrected.Invoke();
        }

        public void DoHeal(int heal, GameObject source)
        {
            if (maxHealth == currentHealth)
                return;

            ChangeHealth(heal, source);
        }

        private void ChangeHealth(int value, GameObject source)
        {
            var prev = currentHealth;
            currentHealth = currentHealth + value;

            currentHealth = math.clamp(currentHealth, 0, maxHealth);

            if (currentHealth != prev)
            {
                onHealthChanged.Invoke(new HealthChangeData
                {
                    CurrentValue = currentHealth,
                    PrevValue = prev,
                    Source = source
                });
            }

            if (currentHealth == 0 && prev != 0)
            {
                m_KilledBy = source;
                onDeath.Invoke();
                onDeathUE.Invoke();
            }
        }

        public void DoDamage(int damage, GameObject source)
        {
            if (!IsDamageable)
                return;

            ChangeHealth(-damage, source);
        }

        public void Kill()
        {
            ChangeHealth(0, null);
        }
    }
}
