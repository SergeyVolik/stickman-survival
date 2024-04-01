using MoreMountains.Feedbacks;
using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    public MMF_Player damageFeedback;
    private HealthData m_HealthData;

    private void Awake()
    {
        damageFeedback.PlayFeedbacks();

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
