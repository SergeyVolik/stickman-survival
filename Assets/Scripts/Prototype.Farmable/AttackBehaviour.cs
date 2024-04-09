using UnityEngine;

namespace Prototype
{
    public class AttackBehaviour : MonoBehaviour
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

        public bool attackOnStanding;
        public bool rotateToTarget;
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
                //m_CurrentWeapon.HideWeapon();
            }
        }

        private void M_CharAnimator_onEnableHitBox()
        {
            //m_CurrentWeapon.ShowWeapon();
            m_CurrentWeapon.EnableHitBox(true);
        }

        private void M_CharAnimator_onEndAttack()
        {
            
        }

        private void M_CharAnimator_onBeginAttack()
        {
            //m_CurrentWeapon.ShowWeapon();
        }

        private Weapon GetWeaponByType(WeaponType type)
        {
           return m_Inventory.ActivateMeleeWeapon(type);
        }

        private void FixedUpdate()
        {
            bool canAttack = attackOnStanding && !m_Controller.IsMoving || !attackOnStanding;

            if (attackOnStanding && m_Controller.IsMoving)
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

                Vector3 targetPos = Vector3.zero;
                for (int i = 0; i < count; i++)
                {
                    var collider = m_CastedColliders[i];

                    if (collider.TryGetComponent<FarmableObject>(out var farmableObj))
                    {
                        m_CurrentWeapon = GetWeaponByType(farmableObj.RequiredWeapon);
                        m_CharAnimator.AttackTrigger();
                        m_Attaking = true;
                        targetPos = farmableObj.transform.position;
                    }
                }

                if (count == 0)
                {
                    m_CharAnimator.ResetAttack();
                }
                else if (rotateToTarget)
                {
                    var point = targetPos;
                    var currentPos = transform.position;
                    point.y = currentPos.y;
                    var vector = point - currentPos;

                    float rotaValue = Mathf.Clamp01(Time.deltaTime * 10);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector), rotaValue);
                }
            }
        }
    }
}