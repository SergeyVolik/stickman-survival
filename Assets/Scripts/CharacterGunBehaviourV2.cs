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

        float stunAfterDamageDur = 1f;
        bool stunned = false;
        float stunT;
        private AimCirclerBehaviour m_AimCircle;
        private CharacterCombatState m_CombatState;

        private void Awake()
        {
            m_Inventory = GetComponent<CharacterInventory>();

            m_Inventory.onMainWeaponChanged += M_Inventory_onGunChanged;

            m_Controller = GetComponent<CustomCharacterController>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();
            var health = GetComponent<HealthData>();

            health.onDeath += () => { enabled = false; };
            health.onHealthChanged += Health_onHealthChanged;
         
            m_AimCircle = GetComponent<AimCirclerBehaviour>();

            m_CombatState = GetComponent<CharacterCombatState>();

            m_CombatState.onCombatState += (value) =>
            {
                UpdateCombatState(value);
            };

            UpdateCombatState(m_CombatState.InCombat);
        }

        private void UpdateCombatState(bool value)
        {
            if (m_Inventory.HasGun())
            {
                if (value)
                {
                    m_AimCircle?.Show();
                    m_Inventory.ActiveLastWeapon();
                }
                else
                {
                    m_AimCircle?.Hide();
                    m_Inventory.HideCurrentWeapon();
                }
            }
        }

        private void M_Inventory_onGunChanged(Gun obj)
        {
           //enabled = obj != null;
        }

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

            if (m_CombatState.InCombat && !currentGun && m_Inventory.HasGunInInventory())
            {
                m_Inventory.ActiveLastWeapon();
                m_AimCircle.Show();
            }

            if (currentGun)
            {
                m_Controller.aimDistance = currentGun.aimDistance;
            }
            else
            {
                m_Controller.aimDistance = 0;
                return;
            }

            bool isMoveing = m_Controller.MoveInput != Vector2.zero;
            float interval = isMoveing ? currentGun.moveShotInterval : currentGun.standingshotInterval;

            if (m_Controller.HasTarget)
            {
                m_ShotT += Time.deltaTime;

                if (m_ShotT > interval)
                {
                    m_ShotT = 0;
                    currentGun.Shot(isMoveing);
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
