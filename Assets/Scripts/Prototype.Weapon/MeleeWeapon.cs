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
        public TrailRenderer Trail;
        private Collider m_HitBox;

        private Transform m_Transform;
        private bool m_Showed;
        public float pushForce;

        private void Awake()
        {
            m_Transform = transform;
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
        
            if (other.TryGetComponent<IRequiredMeleeWeapon>(out var required))
            {
                if (required.RequiredWeapon == Type)
                {
                    other.GetComponent<IDamageable>().DoDamage(damage, gameObject);
                }

                if (other.TryGetComponent<IPushable>(out var rb))
                {
                    var vector = other.transform.position - Owner.transform.position;
                    rb.Push(vector.normalized * pushForce);
                }
            }
        }
    }
}
