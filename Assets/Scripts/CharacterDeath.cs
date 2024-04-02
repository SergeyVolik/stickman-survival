using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeath : MonoBehaviour
{
    public MMRagdoller mMRagdoller;
    public MMF_Player deathFeedback;
    private void Awake()
    {
        var hData = GetComponent<HealthData>();
        hData.onDeath += HData_onDeath;
    }

    private void HData_onDeath()
    {
        mMRagdoller.Ragdolling = true;
        deathFeedback?.PlayFeedbacks();
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CustomCharacterController>().enabled = false;
    }
}
