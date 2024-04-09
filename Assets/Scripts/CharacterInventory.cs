using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterInventory : MonoBehaviour
    {
        private Gun m_PrevWeapon;

        public Gun CurrentGun { get; private set; }

        [SerializeField]
        private GameObject m_StartGunPrefab;

        [SerializeField]
        private Weapon[] m_MeleeWeaponsPrefabs;

        [SerializeField]
        private Weapon[] m_MeleeWeaponsInstances;
        public Weapon CurrentMeleeWeapon { get; private set; }

        public event Action<Gun> onGunChanged = delegate { };

        public Transform rightHand;

        public Transform handgunHidePoint;
        public Transform rifleHidePoint;

        private void Awake()
        {
            m_MeleeWeaponsInstances = new Weapon[m_MeleeWeaponsPrefabs.Length];
            for (int i = 0; i < m_MeleeWeaponsInstances.Length; i++)
            {
                var weapon = GameObject.Instantiate(m_MeleeWeaponsPrefabs[i], rifleHidePoint).GetComponent<Weapon>();
                m_MeleeWeaponsInstances[i] = weapon;

                weapon.HideWeapon();               
                weapon.SetupInHidePoint(rifleHidePoint);
                weapon.owner = gameObject;
            }

            CurrentMeleeWeapon = m_MeleeWeaponsInstances[0];
            m_MeleeWeaponsInstances[0].ShowWeapon();
        }

        private void Start()
        {
            SetupWeapon(m_StartGunPrefab);
        }

        public Weapon ActivateMeleeWeapon(WeaponType type)
        {
            Weapon findedWeapon = null;

            foreach (var item in m_MeleeWeaponsInstances)
            {
                if (item.Type == type)
                {
                    findedWeapon = item;
                    findedWeapon.ShowWeapon();
                    findedWeapon.SetupInHands(rightHand);               
                }
            }

            if (CurrentMeleeWeapon != null && findedWeapon != CurrentMeleeWeapon)
            {
                CurrentMeleeWeapon.SetupInHidePoint(rifleHidePoint);
                CurrentMeleeWeapon.HideWeapon();
            }

            CurrentMeleeWeapon = findedWeapon;

            return findedWeapon;
        }

        public void HideMeleeWeapon()
        {
            if (CurrentMeleeWeapon)
            {
                CurrentMeleeWeapon.SetupInHidePoint(rifleHidePoint);
                CurrentMeleeWeapon.HideWeapon(false);
            }
        }

        public void SetupWeapon(GameObject gunPrefab)
        {
            if (gunPrefab == null)
                return;

            if (m_PrevWeapon)
            {
                GameObject.Destroy(m_PrevWeapon.gameObject);
            }

            bool hasActiveWeapon = CurrentGun != null;

            if (CurrentGun)
            {
                GameObject.Destroy(CurrentGun.gameObject);
            }

            if (hasActiveWeapon)
            {
                CurrentGun = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
                CurrentGun.owner = gameObject;
                CurrentGun.SetupInHands(rightHand);
                onGunChanged.Invoke(CurrentGun);
            }
            else
            {
                m_PrevWeapon = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
                m_PrevWeapon.owner = gameObject;
                m_PrevWeapon.SetupInHidePoint(GetHidePoint(m_PrevWeapon.type));

                onGunChanged.Invoke(null);
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
            return CurrentGun || m_PrevWeapon;
        }

        public bool HasGunInInventory()
        {
            return m_PrevWeapon;
        }

        [Button]
        public void HideCurrentGun()
        {
            if (CurrentGun == null) return;

            m_PrevWeapon = CurrentGun;
            m_PrevWeapon.SetupInHidePoint(GetHidePoint(m_PrevWeapon.type));
            CurrentGun = null;
            onGunChanged.Invoke(null);
        }

        [Button]
        public void ActiveLastGun()
        {
            if (m_PrevWeapon == null)
                return;

            HideMeleeWeapon();

            CurrentGun = m_PrevWeapon;
            CurrentGun.SetupInHands(rightHand);
            onGunChanged.Invoke(CurrentGun);
        }
    }
}

