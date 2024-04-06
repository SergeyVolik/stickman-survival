using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterWithGunAnimator : MonoBehaviour
    {
        private static readonly int IsAimHash = Animator.StringToHash("IsAim");
        private static readonly int TurnHash = Animator.StringToHash("Turn");
        private static readonly int ForwardHash = Animator.StringToHash("Forward");
        private static readonly int ShotHash = Animator.StringToHash("Shot");

        private static readonly int MoveHash = Animator.StringToHash("Move");
        private static readonly int DamageHash = Animator.StringToHash("Damage");

        private static readonly int GunTypeHash = Animator.StringToHash("GunType");

        private Animator m_Animator;
        private CustomCharacterController m_CharacterInput;
        private Transform m_ControllerTrans;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_CharacterInput = GetComponentInParent<CustomCharacterController>();
            var Health = GetComponentInParent<HealthData>();

            if (Health)
            {
                Health.onHealthChanged += (data) =>
                {
                    if (data.IsDamage)
                    {
                        m_Animator.SetTrigger(DamageHash);
                    }
                };
            }

            var gunBehaviour = GetComponentInParent<CharacterInventory>();

            if (gunBehaviour)
            {
                gunBehaviour.onMainWeaponChanged += (gun) =>
                {
                    if (gun == null)
                    {
                        m_Animator.SetInteger(GunTypeHash, 0);
                        return;
                    }

                    m_CharacterInput.canAim = gun != null;
                    m_Animator.SetInteger(GunTypeHash, (int)gun.type);
                };
            }

            m_ControllerTrans = m_CharacterInput.transform;
        }

        public void Move(Vector2 vector)
        {
            m_Animator.SetBool(MoveHash, vector != Vector2.zero);
        }

        private void Update()
        {
            var delta = Time.deltaTime;
            Vector2 moveVec2d = m_CharacterInput.MoveInput;

            Move(moveVec2d);

            if (m_CharacterInput.canAim)
            {
                m_Animator.SetBool(IsAimHash, m_CharacterInput.IsAiming);

                var vector = m_ControllerTrans.InverseTransformDirection(new Vector3(m_CharacterInput.MoveInput.x, 0, m_CharacterInput.MoveInput.y)).normalized;

                var forwardAmount = vector.z * 10;
                var turnAmount = vector.x * 10;

                m_Animator.SetFloat(TurnHash, turnAmount, 0.1f, delta);
                m_Animator.SetFloat(ForwardHash, forwardAmount, 0.1f, delta);
            }
        }

        internal void Shot()
        {
            m_Animator.SetTrigger(ShotHash);
        }
    }
}