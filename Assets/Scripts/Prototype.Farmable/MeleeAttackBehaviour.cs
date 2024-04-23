using UnityEngine;

namespace Prototype
{
    public class MeleeAttackBehaviour : MonoState
    {
        private Transform m_Transform;
        private CharacterInventory m_Inventory;
        [SerializeField]
        public LayerMask m_AttackableLayer;
        private Collider[] m_CastedColliders;
        private CharacterStats m_Stats;
        private MeleeCharacterAnimator m_CharAnimator;

        [SerializeField]
        public float m_AttackRange = 1.5f;

        private MeleeWeapon m_CurrentWeapon;

        private CustomCharacterController m_Controller;
        private bool m_Attaking;
        private Collider m_TargetCollider;
        private IRequiredMeleeWeapon m_RequiredData;

        public bool IsAttaking => m_Attaking;
        public bool CanAttack() => !m_Controller.IsMoving && m_Inventory.HasAnyMeleeWeapon() && m_TargetCollider;

        private void Awake()
        {
            m_Transform = transform;
            m_Inventory = GetComponent<CharacterInventory>();
            m_CastedColliders = new Collider[20];
            m_Stats = GetComponent<CharacterStats>();
            m_CharAnimator = GetComponentInChildren<MeleeCharacterAnimator>();
            m_Controller = GetComponent<CustomCharacterController>();

            m_CharAnimator.onEnableHitBox += M_CharAnimator_onEnableHitBox;
            m_CharAnimator.onDisableHitBox += M_CharAnimator_onDisableHitBox;
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            InterruptAttack();
        }

        public void InterruptAttack()
        {
            m_Attaking = false;
            M_CharAnimator_onDisableHitBox();
            m_CharAnimator.ResetAttack();
            m_Inventory.HideMeleeWeapon();
        }

        private void M_CharAnimator_onDisableHitBox()
        {
            if (m_CurrentWeapon)
            {
                m_CurrentWeapon.EnableHitBox(false);
            }
        }

        private void M_CharAnimator_onEnableHitBox()
        {
            m_CurrentWeapon.EnableHitBox(true);
        }

        private MeleeWeapon ActivateWeapon(MeleeWeaponType type)
        {
            return m_Inventory.ActivateMeleeWeapon(type);
        }

        private void Update()
        {
            if (m_Attaking)
            {
                var currentPos = m_Transform.position;
                m_CharAnimator.SetAttackSpeed(m_Stats.attackSpeedMult);
                m_CurrentWeapon = ActivateWeapon(m_RequiredData.RequiredWeapon);
                m_CurrentWeapon.SetDamageMult(m_Stats.meleeWeaponDamageMult);
                m_CharAnimator.AttackTrigger();
                var targetPos = m_TargetCollider.transform.position;
                //rotate
                var point = targetPos;
                point.y = currentPos.y;
                var vector = point - currentPos;
                const float rotationSpeed = 10f;

                float rotaValue = Mathf.Clamp01(Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), rotaValue);
            }
        }

        public void FindTarget()
        {
            var currentPos = m_Transform.position;
            m_TargetCollider = null;

            var castPos = currentPos;
            castPos.y += 0.5f;
            int count = Physics.OverlapSphereNonAlloc(
                currentPos,
                m_AttackRange,
                m_CastedColliders,
                m_AttackableLayer);

            float closestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var collider = m_CastedColliders[i];

                if (collider.TryGetComponent<HealthData>(out var healthData) &&  healthData.enabled && healthData.IsDamageable)
                {
                    var dist = Vector3.Distance(collider.transform.position, currentPos);

                    if (closestDist > dist)
                    {
                        m_TargetCollider = collider;
                        closestDist = dist;
                    }
                }
            }

            if (!m_TargetCollider || !m_TargetCollider.TryGetComponent<IRequiredMeleeWeapon>(out m_RequiredData))
            {
                m_TargetCollider = null;
                return;
            }

            if (!m_Inventory.HasMeleeWeaponByType(m_RequiredData.RequiredWeapon))
            {
                m_TargetCollider = null;
                return;
            }

            m_Attaking = true;
        }
    }
}