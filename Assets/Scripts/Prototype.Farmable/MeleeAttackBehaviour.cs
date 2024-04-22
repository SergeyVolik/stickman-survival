using UnityEngine;

namespace Prototype
{
    public class MeleeAttackBehaviour : MonoState, IState
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
        private float m_HastTargetTime;

        public bool IsAttaking => m_Attaking;
        public bool HasTarget => m_HastTargetTime + 0.1f > Time.time;

        public bool IsActive { get => enabled; set => enabled = false; }

        public bool blockAttack;

        private void Awake()
        {
            m_Transform = transform;

            m_Inventory = GetComponent<CharacterInventory>();
            m_CastedColliders = new Collider[20];

            m_Stats = GetComponent<CharacterStats>();
            m_CharAnimator = GetComponentInChildren<MeleeCharacterAnimator>();
            m_CharAnimator.onBeginAttack += M_CharAnimator_onBeginAttack;
            m_CharAnimator.onEndAttack += M_CharAnimator_onEndAttack;
            m_CharAnimator.onEnableHitBox += M_CharAnimator_onEnableHitBox;
            m_CharAnimator.onDisableHitBox += M_CharAnimator_onDisableHitBox;

            m_Controller = GetComponent<CustomCharacterController>();
        }

        public void InterruptAttack()
        {
            if (m_Attaking)
            {
                m_Attaking = false;
                M_CharAnimator_onDisableHitBox();
                m_CharAnimator.ResetAttack();
                m_Inventory.HideMeleeWeapon();
            }
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

        private void M_CharAnimator_onEndAttack()
        {

        }

        private void M_CharAnimator_onBeginAttack()
        {

        }

        private MeleeWeapon ActivateWeapon(MeleeWeaponType type)
        {
            return m_Inventory.ActivateMeleeWeapon(type);
        }

        private void FixedUpdate()
        {
            bool canAttack = !m_Controller.IsMoving && m_Inventory.HasAnyMeleeWeapon();

            if (m_Controller.IsMoving)
            {
                InterruptAttack();
            }

            HealthData closestFarmable = null;

            var currentPos = m_Transform.position;
            m_TargetCollider = null;

            if (canAttack)
            {
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

                    if (collider.TryGetComponent<HealthData>(out var farmableObj) && farmableObj.enabled)
                    {
                        var dist = Vector3.Distance(collider.transform.position, currentPos);

                        if (closestDist > dist)
                        {
                            m_TargetCollider = collider;
                            m_HastTargetTime = Time.time;
                            closestDist = dist;
                            closestFarmable = farmableObj;
                        }
                    }
                }
            }

            if (blockAttack)
            {
                return;
            }

            if (m_TargetCollider && m_TargetCollider.TryGetComponent<IRequiredMeleeWeapon>(out var requiredData))
            {
                if (!m_Inventory.HasMeleeWeaponByType(requiredData.RequiredWeapon))
                {
                    return;
                }
                m_CharAnimator.SetAttackSpeed(m_Stats.attackSpeedMult);
                m_CurrentWeapon = ActivateWeapon(requiredData.RequiredWeapon);
                m_CurrentWeapon.SetDamageMult(m_Stats.meleeWeaponDamageMult);
                m_CharAnimator.AttackTrigger();
                m_Attaking = true;
                var targetPos = closestFarmable.transform.position;
                //rotate
                var point = targetPos;
                point.y = currentPos.y;
                var vector = point - currentPos;
                const float rotationSpeed = 10f;

                float rotaValue = Mathf.Clamp01(Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), rotaValue);
            }
            else
            {
                m_CharAnimator.ResetAttack();
                m_TargetCollider = null;
            }
        }
    }
}