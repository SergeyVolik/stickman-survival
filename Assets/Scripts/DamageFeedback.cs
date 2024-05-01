using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    [RequireComponent(typeof(HealthData))]
    public class DamageFeedback : MonoBehaviour
    {
        public MMF_Player damageFeedback;
        public MMF_Player critFeedback;

        public MMF_Player deathFeedback;
        private HealthData m_HealthData;
        private HatSetup m_HatHolder;

        private void Awake()
        {
            m_HealthData = GetComponent<HealthData>();
            m_HatHolder = GetComponent<HatSetup>();
        }

        private void OnEnable()
        {
            m_HealthData.onHealthChanged += M_HealthData_onHealthChanged;
            m_HealthData.onDeath += M_HealthData_onDeath;
        }

        private void OnDisable()
        {
            m_HealthData.onHealthChanged -= M_HealthData_onHealthChanged;
            m_HealthData.onDeath -= M_HealthData_onDeath;
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
             
                if (obj.Source && obj.Source.TryGetComponent<IDamageData>(out var data) && data.IsCrit)
                {
                    if (critFeedback)
                    {
                        var sourceForward = obj.Source.transform.forward;
                        critFeedback.transform.forward = -1 * sourceForward;
                        var floatingText = critFeedback.GetFeedbackOfType<MMF_FloatingText>();
                        floatingText.Value = $"{diff.ToString()}!";
                        critFeedback.StopFeedbacks();
                        critFeedback.PlayFeedbacks(damageFeedback.transform.position);
                    }

                    if (m_HatHolder && m_HatHolder.CurrentHat && !m_HatHolder.CurrentHat.IsDropped)
                    {
                        if (obj.Source.TryGetComponent<Gun>(out var gun) && Random.Range(0f, 100f) > 50)
                        {
                            var shotVector = gun.owner.transform.forward;
                            m_HatHolder.CurrentHat.DropWithImpulse(shotVector*3);
                        }
                    }
                }
                else if(damageFeedback)
                {
                    var floatingText = damageFeedback.GetFeedbackOfType<MMF_FloatingText>();
                    floatingText.Value = diff.ToString();
                    damageFeedback.StopFeedbacks();
                    damageFeedback.PlayFeedbacks(damageFeedback.transform.position);
                }
            }
        }
    }
}