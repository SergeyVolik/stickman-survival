using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterGunBehaviourV2 : MonoBehaviour
    {
        private CharacterInventory m_Inventory;
        private CustomCharacterController m_Controller;
        private CharacterWithGunAnimator m_CharacterAnimator;
        private float m_ShotT;
        public Transform rightHand;

        private void Awake()
        {
            m_Inventory = GetComponent<CharacterInventory>();

            m_Inventory.onMainWeaponChanged += M_Inventory_onGunChanged;

            m_Controller = GetComponent<CustomCharacterController>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();
            var health = GetComponent<HealthData>();

            health.onDeath += () => { enabled = false; };
            health.onHealthChanged += Health_onHealthChanged;
        }

        private void M_Inventory_onGunChanged(Gun obj)
        {
           //enabled = obj != null;
        }

        float stunAfterDamageDur = 1f;
        bool stunned = false;
        float stunT;

        private void Health_onHealthChanged(HealthChangeData obj)
        {
            if (obj.IsDamage)
            {
                stunned = true;
                stunT = 0;
            }
        }

        private void Update()
        {
            if (stunned)
            {
                stunT += Time.deltaTime;

                if (stunT > stunAfterDamageDur)
                {
                    stunned = false;
                }

                return;
            }

            var currentGun = m_Inventory.CurrentWeapon;

            if (currentGun)
            {
                m_Controller.aimDistance = currentGun.aimDistance;
            }
            else
            {
                m_Controller.aimDistance = 3;
                return;
            }

            if (m_Controller.HasTarget)
            {
                m_ShotT += Time.deltaTime;

                if (m_ShotT > currentGun.shotInterval)
                {
                    m_ShotT = 0;
                    currentGun.Shot();
                    m_CharacterAnimator.Shot();
                }
            }
            else
            {
                m_ShotT = 0;
            }
        }
    }
}
