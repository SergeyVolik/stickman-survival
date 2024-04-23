using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class DamageFeedback : MonoBehaviour
    {
        public MMF_Player damageFeedback;
        public MMF_Player critFeedback;

        public MMF_Player deathFeedback;
        private HealthData m_HealthData;

        private void Awake()
        {
            m_HealthData = GetComponent<HealthData>();        
        }

        private void OnEnable()
        {
            m_HealthData.onHealthChanged += M_HealthData_onHealthChanged;
            m_HealthData.onDeath += M_HealthData_onDeath;
        }

        private void M_HealthData_onDeath()
        {
            deathFeedback?.PlayFeedbacks();
        }

        private void M_HealthData_onHealthChanged(HealthChangeData obj)
        {         
            if (obj.IsDamage)
            {
                var diff = obj.PrevValue - obj.CurrentValue;
             
                if (critFeedback && obj.Source && obj.Source.TryGetComponent<IDamageData>(out var data) && data.IsCrit)
                {
                    var sourceForward = obj.Source.transform.forward;
                    critFeedback.transform.forward = -1 * sourceForward;
                    var floatingText = critFeedback.GetFeedbackOfType<MMF_FloatingText>();
                    floatingText.Value = $"{diff.ToString()}!";
                    critFeedback.StopFeedbacks();
                    critFeedback.PlayFeedbacks(damageFeedback.transform.position);
                }
                else if(damageFeedback)
                {
                    var sourceForward = obj.Source.transform.forward;
                    damageFeedback.transform.forward = -1 * sourceForward;
                    var floatingText = damageFeedback.GetFeedbackOfType<MMF_FloatingText>();
                    floatingText.Value = diff.ToString();
                    damageFeedback.StopFeedbacks();
                    damageFeedback.PlayFeedbacks(damageFeedback.transform.position);
                }

            }
        }
    }
}