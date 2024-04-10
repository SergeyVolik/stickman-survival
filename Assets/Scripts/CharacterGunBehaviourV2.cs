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
        private Transform m_Transform;
        private AimCirclerBehaviour m_AimCircle;
        private CharacterCombatState m_CombatState;
        public bool blockAttack;

        private void Awake()
        {
            m_Inventory = GetComponent<CharacterInventory>();
            m_Inventory.onGunChanged += M_Inventory_onMainWeaponChanged;
            m_Controller = GetComponent<CustomCharacterController>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();

            var health = GetComponent<HealthData>();
            health.onDeath += () => { enabled = false; };
            health.onHealthChanged += Health_onHealthChanged;

            m_Transform = transform;
            m_AimCircle = GetComponent<AimCirclerBehaviour>();
            m_CombatState = GetComponent<CharacterCombatState>();
            m_CombatState.onCombatState += (value) =>
            {
                UpdateCombatState(value);
            };

            UpdateCombatState(m_CombatState.InCombat);
        }

        private void M_Inventory_onMainWeaponChanged(Gun obj)
        {
            if (obj)
            {
                m_Controller.aimDistance = obj.aimDistance + 1;
            }
            else 
            {
                m_Controller.aimDistance = 0;
            }
        }

        private void UpdateCombatState(bool value)
        {
            if (m_Inventory.HasGun())
            {
                if (value)
                {
                    m_AimCircle?.Show();
                    m_Inventory.ActiveLastGun();
                }
                else
                {
                    m_AimCircle?.Hide();
                    m_Inventory.HideCurrentGun();
                }
            }
        }

        private void Health_onHealthChanged(HealthChangeData obj)
        {
            if (obj.IsDamage)
            {
                stunned = true;
                stunT = 0;
            }
        }

        private void FixedUpdate()
        {
            if (blockAttack)
            {
                return;
            }

            var deltaTime = Time.fixedDeltaTime;

            if (stunned)
            {
                stunT += deltaTime;

                if (stunT > stunAfterDamageDur)
                {
                    stunned = false;
                }

                return;
            }

            var currentGun = m_Inventory.CurrentGun;


            if (m_CombatState.InCombat && !currentGun && m_Inventory.HasGunInInventory())
            {
                m_Inventory.ActiveLastGun();
                m_AimCircle.Show();
            }
            currentGun = m_Inventory.CurrentGun;

            if (currentGun == null)
                return;

            bool isMoveing = m_Controller.MoveInput != Vector2.zero;
            float interval = isMoveing ? currentGun.moveShotInterval : currentGun.standingshotInterval;

            if (m_Controller.HasTarget)
            {
                var targetPos = m_Controller.CurrentTarget.position;
                var distance = Vector3.Distance(m_Transform.position, targetPos);

                if (distance > currentGun.aimDistance)
                {
                    return;
                }

                m_ShotT += deltaTime;

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
