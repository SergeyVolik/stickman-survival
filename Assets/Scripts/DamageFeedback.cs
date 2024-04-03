using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class DamageFeedback : MonoBehaviour
    {
        public MMF_Player damageFeedback;
        private HealthData m_HealthData;

        private void Awake()
        {
            m_HealthData = GetComponent<HealthData>();
        }

        private void OnEnable()
        {
            m_HealthData.onHealthChanged += M_HealthData_onHealthChanged;
        }

        private void M_HealthData_onHealthChanged(HealthChangeData obj)
        {
            var diff = obj.PrevValue - obj.CurrentValue;

            if (diff != 0)
            {
                damageFeedback.PlayFeedbacks(damageFeedback.transform.position, diff);
            }
        }
    }
}