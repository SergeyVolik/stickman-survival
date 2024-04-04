using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;

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
        public GunType type = GunType.Handgun;
        public int damage;
        public float pushForce;
        public float killPushForce;
        [SerializeField]
        private MMF_Player feedback;
        [SerializeField]
        private Transform projectileSpawnPoint;
        public float shotInterval;
        public float aimDistance = 3;
        public GameObject owner;

        public GameObject Owner => owner;

        public void Shot()
        {
            feedback?.PlayFeedbacks();

            var shotVector = owner.transform.forward;

            if (Physics.Raycast(projectileSpawnPoint.position, shotVector, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.DoDamage(damage, gameObject);
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