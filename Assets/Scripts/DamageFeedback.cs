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
            if (obj.IsDamage)
            {
                var diff = obj.PrevValue - obj.CurrentValue;
                damageFeedback.GetFeedbackOfType<MMF_FloatingText>().Value = diff.ToString();
                damageFeedback.PlayFeedbacks(damageFeedback.transform.position); 
            }
        }
    }
}