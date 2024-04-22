using UnityEngine;

namespace Prototype
{
    public interface IState
    {
        public bool IsActive { get; set; }
        public void Awake() { }
        public void Start() { }
        public void OnEnable() { }
        public void OnDisable() { }
        public void Update() { }
        public void FixedUpdate() { }
    }

    public abstract class MonoState : MonoBehaviour, IState
    {
        public bool IsActive { get => enabled; set => enabled = false; }
    }

    public class CharacterGunBehaviourV2 : MonoState
    {
        private CharacterInventory m_Inventory;
        private CustomCharacterController m_Controller;
        private CharacterWithGunAnimator m_CharacterAnimator;
        private float m_ShotT;
        private float stunAfterDamageDur = 1f;
        private bool stunned = false;
        private float stunT;
        private Transform m_Transform;
        private AimCirclerBehaviour m_AimCircle;
        private CharacterCombatState m_CombatState;    
        private Transform m_LastTarget;

        public bool blockAttack;

        public void Awake()
        {
            m_Inventory = GetComponent<CharacterInventory>();
            m_Inventory.onGunChanged += M_Inventory_onMainWeaponChanged;
            m_Controller = GetComponent<CustomCharacterController>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();

            var health = GetComponent<HealthData>();
            health.onDeath += () => { enabled = false; };
            health.onHealthChanged += Health_onHealthChanged;

            m_Transform = GetComponent<Transform>();
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
                m_Controller.aimDistance = obj.aimDistance;
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

        public void FixedUpdate()
        {          
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

            if (blockAttack)
            {
                return;
            }

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

            if (HasGunTarget())
            {
                if (m_LastTarget != m_Controller.CurrentTarget)
                {
                    SelectTarget(false);
                    m_LastTarget = m_Controller.CurrentTarget;
                    SelectTarget(true);
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

        private void SelectTarget(bool activate)
        {
            if (m_LastTarget && m_LastTarget.TryGetComponent<SelectionTarget>(out var selection))
            {
                if (activate)
                {
                    selection.Activate();
                }
                else
                {
                    selection.Deactivate();
                }
            }
        }

        public bool HasAimTarget()
        {
            return m_Controller.HasTarget;
        }

        public bool HasGunTarget()
        {
            if (m_Controller.HasTarget)
            {
                var gun = m_Inventory.GetGun();

                var targetPos = m_Controller.CurrentTarget.position;
                var distance = Vector3.Distance(m_Transform.position, targetPos);


                if (gun == null)
                    return false;

                if (distance > gun.aimDistance)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
