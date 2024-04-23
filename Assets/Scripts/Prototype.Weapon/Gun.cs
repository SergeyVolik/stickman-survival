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

    public interface IDamageData
    {
        bool IsCrit { get; }
    }

    public class Gun : BaseWeapon, IOwnable, IDamageData
    {
        public GunType type = GunType.Handgun;

        public float pushForce;
        public float killPushForce;
        public int standingDamage;
        public float critMult = 1.1f;
        public float critChanse = 0.5f;

        public float standingshotInterval = 1;

        [SerializeField]
        private MMF_Player m_ShotFeedback;

        [SerializeField]
        private MMF_Player m_EquipFeedback;

        [SerializeField]
        private Transform projectileSpawnPoint;
       
        public float aimDistance = 3;

        public LayerMask physicsPlayer;
        public float damagMult = 1f;

        public bool IsCrit { 
            get;
            private set;
        }

        public void TryCrit()
        {
            IsCrit = Random.Range(0f, 100f) < critChanse * 100;
        }

        public override void SetupInHands(Transform hand)
        {
            base.SetupInHands(hand);
            m_EquipFeedback?.PlayFeedbacks();
        }

        public void ShotOnTarget(Transform target)
        {
            m_ShotFeedback?.PlayFeedbacks();

            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                var damage = standingDamage;
                damage = (int)(damage * damagMult);
                TryCrit();

                if (IsCrit)
                {
                    damage = (int)(damage * critMult);
                }

                damageable.DoDamage(damage, gameObject);
            }

            if (target.TryGetComponent<IPushable>(out var rb))
            {
                var shotVector = owner.transform.forward;
                rb.Push(shotVector * pushForce);
            }
        }

        public void ShotForward()
        {
            m_ShotFeedback?.PlayFeedbacks();

            var shotVector = owner.transform.forward;
            var shotPos = owner.transform.position;
            shotPos.y += 0.5f;

            if (Physics.Raycast(shotPos, shotVector, out RaycastHit hit, 100f, physicsPlayer))
            {
                ShotOnTarget(hit.transform);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Ray { origin = projectileSpawnPoint.position, direction = projectileSpawnPoint.forward });
        }
    }
}