using MoreMountains.Feedbacks;
using System;
using UnityEngine;

namespace Prototype
{
    public enum MeleeWeaponType
    {
        Axe,
        Crutch
    }

    public class MeleeWeapon : BaseWeapon, IOwnable
    {
        public MeleeWeaponType Type;
        public int damage;
        public float damageMult = 1;
        public int level = 1;
        public TrailRenderer Trail;
        private Collider m_HitBox;
        public float pushForce;

        public bool ignoreHighLevelObjects;
        public MMF_Player hitFeedback;
        private void Awake()
        {
            m_HitBox = GetComponent<Collider>();
           
            ActivateTrail(false);
            EnableHitBox(false);
        }

        public void ActivateTrail(bool activate)
        {
            if (Trail)
            {
                Trail.gameObject.SetActive(activate);
                Trail.Clear();
            }
        }

        public void EnableHitBox(bool enable)
        {
            m_HitBox.enabled = enable;
        }

        public void HideWeapon(bool disable = true)
        {
            gameObject.SetActive(!disable);
            EnableHitBox(false);
            ActivateTrail(false);
        }

        public void ShowWeapon()
        {
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Owner == other.gameObject)
                return;

            var damageable = other.GetComponent<IDamageable>();

            if (damageable == null || !damageable.IsDamageable)
                return;

            if (!other.TryGetComponent<IRequiredMeleeWeapon>(out var required))
            {
                return;
            }

            if (required.RequiredWeaponLevel > level && ignoreHighLevelObjects)
            {
                return;
            }

            if (required.Validate(level, Type))
            {
                other.GetComponent<IDamageable>().DoDamage((int)(damage * damageMult), gameObject);
                hitFeedback?.PlayFeedbacks();
                if (other.TryGetComponent<IPushable>(out var rb))
                {
                    var vector = other.transform.position - Owner.transform.position;
                    rb.Push(vector.normalized * pushForce);
                }
            }     
        }

        public void SetDamageMult(float newDamageMult)
        {
            damageMult = newDamageMult;
        }
    }
}
