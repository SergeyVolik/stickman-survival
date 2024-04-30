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
           
            var collider = GetComponent<Collider>();
            var controller = GetComponent<CustomCharacterController>();
            var rb = GetComponent<Rigidbody>();
            var hatSetup = GetComponent<HatSetup>();
            var outline = mMRagdoller.GetComponent<Outline>();
            var trans = transform;

            m_hData.onDeath += () => {
                mMRagdoller.Ragdolling = true;
                deathFeedback?.PlayFeedbacks();
                
                if (collider)
                    collider.enabled = false;

                if (rb)
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                }

                if (controller)
                    controller.enabled = false;

                if (m_hData.KilledBy.TryGetComponent<Gun>(out var gun))
                {
                    var vec = gun.owner.transform.forward;

                    var body = mMRagdoller.Chest;
                    body.AddForce(vec * gun.killPushForce, mode: ForceMode.Impulse);
                }

                DOVirtual.DelayedCall(Random.Range(0.3f, 0.5f), () =>
                {
                    if (hatSetup && hatSetup.CurrentHat)
                    {
                        hatSetup.CurrentHat.Drop();
                    }
                });

                if (outline)
                {
                    GameObject.Destroy(outline);
                }

                DOVirtual.DelayedCall(2f, () =>
                {
                    var y = trans.position.y;
                    mMRagdoller.ForceKinematic();

                    trans.DOMoveY(y + moveUndergroundOffset, duration: moveDuration).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });
                });
            };
        }
    }
}