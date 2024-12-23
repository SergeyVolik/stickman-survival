using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class CharacterInventory : MonoBehaviour
    {
        private Gun m_PrevGun;

        public Gun CurrentGun { get; private set; }

        [SerializeField]
        private GameObject m_StartGunPrefab;

        [SerializeField]
        private MeleeWeapon[] m_MeleeWeaponsPrefabs;

       
        private List<MeleeWeapon> m_MeleeWeaponsInstances;
        public MeleeWeapon CurrentMeleeWeapon { get; private set; }

        public event Action<Gun> onGunChanged = delegate { };

        public Transform rightHand;

        public Transform handgunHidePoint;
        public Transform rifleHidePoint;

        private void Awake()
        {
            InitMeleeWeapons();
            SetupGun(m_StartGunPrefab);
        }

        private void InitMeleeWeapons()
        {
            m_MeleeWeaponsInstances = new List<MeleeWeapon>();

            if (m_MeleeWeaponsPrefabs.Length != 0)
            {
                for (int i = 0; i < m_MeleeWeaponsPrefabs.Length; i++)
                {
                    var meleePrefab = m_MeleeWeaponsPrefabs[i];

                    InstantiateWeapon(meleePrefab);
                }

                CurrentMeleeWeapon = m_MeleeWeaponsInstances[0];
                m_MeleeWeaponsInstances[0].ShowWeapon();
            }
        }

        private MeleeWeapon InstantiateWeapon(MeleeWeapon meleePrefab)
        {
            var weapon = GameObject.Instantiate(meleePrefab, rifleHidePoint).GetComponent<MeleeWeapon>();
            m_MeleeWeaponsInstances.Add(weapon);

            weapon.HideWeapon();

            weapon.SetupInHidePoint(rifleHidePoint);
            weapon.owner = gameObject;

            return weapon;
        }

        public void SetupMeleeWeapon(MeleeWeapon meleeWeaponPrefab)
        {
            MeleeWeapon toRemove = null;

            foreach (var item in m_MeleeWeaponsInstances)
            {
                if (meleeWeaponPrefab.Type == item.Type)
                {
                    toRemove = item;
                    break;
                }
            }

            var weapon = InstantiateWeapon(meleeWeaponPrefab);

            if (toRemove)
            {
                if (toRemove.IsInHands)
                {
                    weapon.SetupInHands(rightHand);
                    weapon.ShowWeapon();
                }

                m_MeleeWeaponsInstances.Remove(toRemove);
                GameObject.Destroy(toRemove.gameObject);
            }

            if (m_MeleeWeaponsInstances.Count == 1)
            {
                CurrentMeleeWeapon = m_MeleeWeaponsInstances[0];
                m_MeleeWeaponsInstances[0].ShowWeapon();
            }
        }

        public MeleeWeapon ActivateMeleeWeapon(MeleeWeaponType type)
        {
            HideCurrentGun();

            MeleeWeapon findedWeapon = null;

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

        public void SetupGun(GameObject gunPrefab)
        {
            if (gunPrefab == null)
                return;

            if (m_PrevGun)
            {
                GameObject.Destroy(m_PrevGun.gameObject);
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
                m_PrevGun = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
                m_PrevGun.owner = gameObject;
                m_PrevGun.SetupInHidePoint(GetHidePoint(m_PrevGun.type));

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
            return CurrentGun || m_PrevGun;
        }

        public bool GunIsActive()
        {
            return CurrentGun;
        }

        public Gun GetGun() => CurrentGun ? CurrentGun : m_PrevGun;
        public bool HasGunInInventory()
        {
            return m_PrevGun;
        }

        [Button]
        public void HideCurrentGun()
        {
            if (CurrentGun == null) return;

            m_PrevGun = CurrentGun;
            m_PrevGun.SetupInHidePoint(GetHidePoint(m_PrevGun.type));
            CurrentGun = null;
            onGunChanged.Invoke(null);
        }

        [Button]
        public void ActiveLastGun()
        {
            if (m_PrevGun == null)
                return;

            HideMeleeWeapon();

            CurrentGun = m_PrevGun;
            CurrentGun.SetupInHands(rightHand);
            onGunChanged.Invoke(CurrentGun);
        }

        internal bool HasAnyMeleeWeapon()
        {
            return m_MeleeWeaponsInstances.Count > 0;
        }

        internal bool HasMeleeWeaponByType(MeleeWeaponType type)
        {
            foreach (var item in m_MeleeWeaponsInstances)
            {
                if (item.Type == type)
                    return
                        true;
            }

            return false;
        }
    }
}

