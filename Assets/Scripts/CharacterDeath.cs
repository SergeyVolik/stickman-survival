using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Prototype;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterDeath : MonoBehaviour
{
    public Ragdoll mMRagdoller;
    public MMF_Player deathFeedback;
    private HealthData m_hData;
    public float destroyDelay = 2f;

    private void Awake()
    {
        m_hData = GetComponent<HealthData>();
        m_hData.onDeath += HData_onDeath;
    }

    private void HData_onDeath()
    {
        mMRagdoller.Ragdolling = true;
        deathFeedback?.PlayFeedbacks();

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CustomCharacterController>().enabled = false;

        if (m_hData.KilledBy.TryGetComponent<Gun>(out var gun))
        {
            var vec = gun.owner.transform.forward;

            var body = mMRagdoller.RagdollBodies[Random.Range(0, mMRagdoller.RagdollBodies.Length)];
            body.AddForce(vec * gun.killPushForce, mode: ForceMode.Force);
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            GameObject.Destroy(gameObject);
        });
    }
}
