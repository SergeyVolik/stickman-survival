using UnityEngine;

namespace Prototype
{
    public class MeleeAttackBehaviour : MonoBehaviour
    {
        private Transform m_Transform;
        private CharacterInventory m_Inventory;
        [SerializeField]
        public LayerMask m_AttackableLayer;
        private Collider[] m_CastedColliders;
        private CharacterAnimator m_CharAnimator;

        [SerializeField]
        public BoxCollider m_CastCollider;

        private Weapon m_CurrentWeapon;

        private CustomCharacterController m_Controller;
        private bool m_Attaking;

        private void Awake()
        {
            m_Transform = transform;

            m_Inventory = GetComponent<CharacterInventory>();
            m_CastedColliders = new Collider[20];
            
            m_CharAnimator = GetComponentInChildren<CharacterAnimator>();

            m_CharAnimator.onBeginAttack += M_CharAnimator_onBeginAttack;
            m_CharAnimator.onEndAttack += M_CharAnimator_onEndAttack;
            m_CharAnimator.onEnableHitBox += M_CharAnimator_onEnableHitBox;
            m_CharAnimator.onDisableHitBox += M_CharAnimator_onDisableHitBox;

            m_Controller = GetComponent<CustomCharacterController>();            
        }

        private void InterruptAttack()
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

        private Weapon GetWeaponByType(WeaponType type)
        {
           return m_Inventory.ActivateMeleeWeapon(type);
        }

        private void FixedUpdate()
        {
            bool canAttack = !m_Controller.IsMoving;

            if (m_Controller.IsMoving)
            {
                InterruptAttack();
            }

            if (canAttack)
            {
                var castTrans = m_CastCollider.transform;

                var transScale = m_CastCollider.transform.lossyScale;
                var boxSize = m_CastCollider.size;

                var size = new Vector3(boxSize.x * transScale.x, boxSize.y * transScale.y, boxSize.z * transScale.z);
                var halfSize = size / 2f;

                int count = Physics.OverlapBoxNonAlloc(
                    castTrans.position + m_CastCollider.center,
                    halfSize,
                    m_CastedColliders,
                    castTrans.rotation,
                    m_AttackableLayer);

                float closestDist = float.MaxValue;
                HealthData closestFarmable = null;

                var currentPos = m_Transform.position;
                Collider closestCollider = null;

                for (int i = 0; i < count; i++)
                {
                    var collider = m_CastedColliders[i];

                    if (collider.TryGetComponent<HealthData>(out var farmableObj) && farmableObj.enabled)
                    {
                        var dist = Vector3.Distance(collider.transform.position, currentPos);

                        if (closestDist > dist)
                        {
                            closestCollider = collider;
                            closestDist = dist;
                            closestFarmable = farmableObj;
                        }
                    }
                }

                if (closestCollider && closestCollider.TryGetComponent<IRequiredMeleeWeapon>(out var requiredData))
                {
                    m_CurrentWeapon = GetWeaponByType(requiredData.RequiredWeapon);
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
                }
            }
        }
    }
}