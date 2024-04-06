using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterInventory : MonoBehaviour
    {
        private Gun m_PrevWeapon;

        public Gun CurrentWeapon { get; private set; }

        [SerializeField]
        private GameObject m_StartWeaponPrefab;

        public event Action<Gun> onMainWeaponChanged = delegate { };

        public Transform rightHand;

        public Transform handgunHidePoint;
        public Transform rifleHidePoint;

        private void Start()
        {
            SetupWeapon(m_StartWeaponPrefab);
        }

        public void SetupWeapon(GameObject gunPrefab)
        {
            if (gunPrefab == null)
                return;

            if (m_PrevWeapon)
            {
                GameObject.Destroy(m_PrevWeapon.gameObject);
            }

            bool hasActiveWeapon = CurrentWeapon != null;

            if (CurrentWeapon)
            {
                GameObject.Destroy(CurrentWeapon.gameObject);
            }

            if (hasActiveWeapon)
            {
                CurrentWeapon = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
                CurrentWeapon.owner = gameObject;
                CurrentWeapon.SetupInHands(rightHand);

                onMainWeaponChanged.Invoke(CurrentWeapon);
            }
            else {
                m_PrevWeapon = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
                m_PrevWeapon.owner = gameObject;
                m_PrevWeapon.SetupInHidePoint(GetHidePoint(m_PrevWeapon.type));

                onMainWeaponChanged.Invoke(null);
            }
        }

        private Transform GetHidePoint(GunType type)
        {
            switch (type)
            {
                case GunType.None:
                    return null;

                case GunType.Handgun:

                    return handgunHidePoint;
                case GunType.AssaultRifle:
                    return rifleHidePoint;
                default:
                    break;
            }

            return null;
        }

        public bool HasGun()
        {
            return CurrentWeapon || m_PrevWeapon;
        }

        public bool HasGunInInventory()
        {
            return m_PrevWeapon;
        }

        [Button]
        public void HideCurrentWeapon()
        {
            if (CurrentWeapon == null) return;

            m_PrevWeapon = CurrentWeapon;
            m_PrevWeapon.SetupInHidePoint(GetHidePoint(m_PrevWeapon.type));
            CurrentWeapon = null;
            onMainWeaponChanged.Invoke(null);
        }

        [Button]
        public void ActiveLastWeapon()
        {
            if (m_PrevWeapon == null)
                return;

            CurrentWeapon = m_PrevWeapon;
            CurrentWeapon.SetupInHands(rightHand);
            onMainWeaponChanged.Invoke(CurrentWeapon);
        }
    }
}

