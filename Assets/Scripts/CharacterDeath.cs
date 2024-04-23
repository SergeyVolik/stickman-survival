using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class CharacterDeath : MonoBehaviour
    {
        public Ragdoll mMRagdoller;
        public MMF_Player deathFeedback;
        private HealthData m_hData;
        public float destroyDelay = 2f;
        public float moveUndergroundOffset = -1;
        public float moveDuration = 1;

        private void Awake()
        {
            m_hData = GetComponent<HealthData>();
            m_hData.onDeath += HData_onDeath;
        }

        private void HData_onDeath()
        {
            mMRagdoller.Ragdolling = true;
            deathFeedback?.PlayFeedbacks();

            var collider = GetComponent<Collider>();

            if (collider)
                collider.enabled = false;
            var rb = GetComponent<Rigidbody>();

            if (rb)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }

            var controller = GetComponent<CustomCharacterController>();

            if (controller)
                controller.enabled = false;


            if (m_hData.KilledBy.TryGetComponent<Gun>(out var gun))
            {
                var vec = gun.owner.transform.forward;

                var body = mMRagdoller.Chest;
                body.AddForce(vec * gun.killPushForce, mode: ForceMode.Impulse);
            }

            if (mMRagdoller.TryGetComponent<Outline>(out var outline))
            {
                GameObject.Destroy(outline);
            }

            DOVirtual.DelayedCall(2f, () =>
            {
                var y = transform.position.y;
                mMRagdoller.ForceKinematic();

                transform.DOMoveY(y + moveUndergroundOffset, duration: moveDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        }
    }
}