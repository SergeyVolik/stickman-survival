using Prototype;
using UnityEngine;

namespace Prototype
{
    public class HealthRegen : MonoBehaviour
    {
        public int regenPerSec;
        public int tickInterval = 5;

        private void Awake()
        {
            m_Health = GetComponent<HealthData>();
        }

        float regenT;
        private HealthData m_Health;
        private float healError;
        private void Update()
        {
            if (m_Health.IsDead)
                return;

            regenT += Time.deltaTime;

            if (regenT > tickInterval)
            {
                regenT = 0;
                var heal = regenPerSec * tickInterval;               
                healError += heal;

                if (healError > 1)
                {
                    var realHeal = (int)healError;
                    healError -= realHeal;
                    m_Health.DoHeal(realHeal, gameObject);
                }
            }
        }
    }
}
