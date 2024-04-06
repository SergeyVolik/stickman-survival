using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;
using UnityEngine.Serialization;

namespace Prototype
{
    public enum GunType
    {
        None = 0,
        Handgun = 1,
        AssaultRifle = 2,
    }

    public interface IOwnable
    {
        public GameObject Owner { get; }
    }

    public class Gun : MonoBehaviour, IOwnable
    {
        public Vector3 handOffset;
        public Vector3 handRotation;

        public Vector3 hideOffset;
        public Vector3 hideRotation;

        public GunType type = GunType.Handgun;

        [FormerlySerializedAs("damage")]
        public int moveDamage;
        public float pushForce;
        public float killPushForce;
        [FormerlySerializedAs("shotInterval")]
        public float moveShotInterval = 1;

        public int standingDamage;
        public float standingshotInterval = 1;

        [SerializeField]
        private MMF_Player m_ShotFeedback;

        [SerializeField]
        private MMF_Player m_EquipFeedback;

        [SerializeField]
        private Transform projectileSpawnPoint;
       
        public float aimDistance = 3;
        public GameObject owner;
        public LayerMask physicsPlayer;
        public GameObject Owner => owner;

        public void SetupInHands(Transform hand)
        {
            var trans = transform;
            trans.parent = hand;
            trans.localPosition = handOffset;
            trans.localRotation = Quaternion.Euler(handRotation);
            m_EquipFeedback?.PlayFeedbacks();
        }

        public void SetupInHidePoint(Transform hidePoint)
        {
            var trans = transform;
            trans.parent = hidePoint;
            trans.localPosition = hideOffset;
            trans.localRotation = Quaternion.Euler(hideRotation);
        }

        public void Shot(bool isMoveing)
        {
            m_ShotFeedback?.PlayFeedbacks();

            var shotVector = owner.transform.forward;

            if (Physics.Raycast(projectileSpawnPoint.position, shotVector, out RaycastHit hit, 100f, physicsPlayer))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.DoDamage(isMoveing ? moveDamage : standingDamage, gameObject);
                }

                if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddForce(shotVector * pushForce, mode: ForceMode.Force);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Ray { origin = projectileSpawnPoint.position, direction = projectileSpawnPoint.forward });
        }
    }
}