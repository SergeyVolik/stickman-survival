using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Prototype
{
    public enum WeaponType
    {
        Axe,
        Crutch
    }

    public class Weapon : BaseWeapon, IOwnable
    {
        public WeaponType Type;

        public int damage;
        public TrailRenderer Trail;
        private Collider m_HitBox;

        private Transform m_Transform;
        private bool m_Showed;

        private void Awake()
        {
            m_Transform = transform;
            m_HitBox = GetComponent<Collider>();
           
            ActivateTrail(false);
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
            if (!m_Showed)
                return;

            m_HitBox.enabled = enable;
            ActivateTrail(enable);
        }

        public void HideWeapon(bool disable = true)
        {
            m_Showed = false;
            gameObject.SetActive(!disable);
            EnableHitBox(false);
            ActivateTrail(false);
        }

        public void ShowWeapon()
        {
            m_Showed = true;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Owner == other.gameObject)
                return;
        
            if (other.TryGetComponent<FarmableObject>(out var farmable) && farmable.enabled)
            {
                if (farmable.RequiredWeapon == Type)
                {
                    other.GetComponent<IDamageable>().DoDamage(damage, gameObject);
                }
            }
        }
    }
}
