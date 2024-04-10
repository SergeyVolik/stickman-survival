using MoreMountains.Feedbacks;
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

    public class Gun : BaseWeapon, IOwnable
    {
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

        public LayerMask physicsPlayer;

        public override void SetupInHands(Transform hand)
        {
            base.SetupInHands(hand);
            m_EquipFeedback?.PlayFeedbacks();
        }

        public void Shot(bool isMoving)
        {
            m_ShotFeedback?.PlayFeedbacks();

            var shotVector = owner.transform.forward;
            var shotPos = owner.transform.position;
            shotPos.y += 0.5f;

            if (Physics.Raycast(shotPos, shotVector, out RaycastHit hit, 100f, physicsPlayer))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.DoDamage(isMoving ? moveDamage : standingDamage, gameObject);
                }

                if (hit.collider.TryGetComponent<IPushable>(out var rb))
                {
                    rb.Push(shotVector * pushForce);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Ray { origin = projectileSpawnPoint.position, direction = projectileSpawnPoint.forward });
        }
    }
}