using UnityEngine;

namespace Prototype
{
    public class CharacterGunBehaviourV2 : MonoState
    {
        private CharacterInventory m_Inventory;
        private CustomCharacterController m_Controller;
        private MeleeRangeStateMachine m_SM;
        private CharacterWithGunAnimator m_CharacterAnimator;
        private HealthData m_Health;
        private float m_ShotT;
        private float stunAfterDamageDur = 1f;
        private bool stunned = false;
        private float stunT;
        private Transform m_Transform;
        private AimCirclerBehaviour m_AimCircle;
        private CharacterCombatState m_CombatState;    
        private Transform m_LastTarget;
        public float aimDistance = 5f;
        float noTargetT = 100;
        float resetAimStateIfNoTarget = 1f;
        public LayerMask CastMask;
        public LayerMask WallMask;
        public float criticalDistanceChangeTarget;
        public float swithTargetInterval = 0.5f;
        private float m_LastSwithTarget;
        public bool standingOnlyAim = false;
        public bool canAim;
        RaycastHit[] results;

        public bool HasTarget
        {
            get;
            private set;
        }

        public HealthData CurrentTargetHealth
        {
            get;
            private set;
        }

        public Transform CurrentTarget
        {
            get;
            private set;
        }
        private (Rigidbody body, float closestDistance) GetTargetWithClosestDistance()
        {
            Vector3 currentPos = m_Transform.position;

            int count = PhysicsHelper.GetAllTargetWithoutWalls(m_Transform, results, aimDistance, CastMask, WallMask, 0.5f);

            RaycastHit closest = default;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var dist = Vector3.Distance(currentPos, results[i].transform.position);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = results[i];
                }
            }

            return (closest.rigidbody, closestDistance);
        }

        void UpdateTarget(Transform targetNew)
        {
            CurrentTargetHealth = targetNew.GetComponent<HealthData>();

            if (CurrentTargetHealth)
            {
                CurrentTarget = targetNew;
            }
        }

        public bool IsAiming { get; private set; }

        public void Awake()
        {
            m_Inventory = GetComponent<CharacterInventory>();       
            m_Controller = GetComponent<CustomCharacterController>();
            m_SM = GetComponent<MeleeRangeStateMachine>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();
            m_Health = GetComponent<HealthData>();
            m_Transform = GetComponent<Transform>();
            m_AimCircle = GetComponent<AimCirclerBehaviour>();
            m_CombatState = GetComponent<CharacterCombatState>();

            m_Inventory.onGunChanged += M_Inventory_onMainWeaponChanged;
            m_CombatState.onCombatState += UpdateCombatState;
        }

        private void OnEnable()
        {                    
            m_Health.onDeath += OnDeath;
            m_Health.onHealthChanged += Health_onHealthChanged;           
        }

        void OnDeath()
        {
            enabled = false;
        }

        private void OnDisable()
        {
            m_Health.onDeath -= OnDeath;
            m_Health.onHealthChanged -= Health_onHealthChanged;         
        }

        private void M_Inventory_onMainWeaponChanged(Gun obj)
        {
            if (obj)
            {
                aimDistance = obj.aimDistance;
            }
            else aimDistance = 0;
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

        public void CheckRangeTarget()
        {
            var currentPos = m_Transform.position;
            var deltaTime = Time.deltaTime;

            noTargetT += deltaTime;
            m_LastSwithTarget += deltaTime;
            var MoveInput = m_Controller.MoveInput;

            var needAim = standingOnlyAim && MoveInput == Vector2.zero || !standingOnlyAim;
            m_Controller.AimVector = Vector2.zero;

            if (canAim && needAim)
            {
                if (CurrentTargetHealth)
                {
                    var dist = Vector3.Distance(CurrentTargetHealth.transform.position, currentPos);

                    if (CurrentTargetHealth.IsDead)
                    {
                        CurrentTargetHealth = null;
                    }
                    else if (dist > aimDistance)
                    {
                        CurrentTargetHealth = null;
                    }

                    var data = GetTargetWithClosestDistance();

                    Rigidbody newClosestUnit = null;
                    if (data.closestDistance < criticalDistanceChangeTarget)
                    {
                        newClosestUnit = data.body;
                    }

                    if (newClosestUnit != null && m_LastSwithTarget > swithTargetInterval)
                    {
                        m_LastSwithTarget = 0;
                        UpdateTarget(newClosestUnit.transform);
                    }
                    HasTarget = CurrentTargetHealth != null;
                }

                //change target
                if (!HasTarget)
                {
                    var data = GetTargetWithClosestDistance();
                    HasTarget = data.body;

                    if (HasTarget)
                    {
                        noTargetT = 0;

                        UpdateTarget(data.body.transform);
                    }
                }

                //update aim data
                if (HasTarget)
                {
                    var point = CurrentTargetHealth.transform.position;

                    point.y = currentPos.y;
                    var vector = point - currentPos;

                    m_Controller.AimVector = new Vector2(vector.x, vector.z);
                }
            }
            else
            {
                HasTarget = false;
                noTargetT = resetAimStateIfNoTarget + 1;
            }

            if (m_LastTarget != CurrentTarget)
            {
                SelectTarget(false);
                m_LastTarget = CurrentTarget;
                SelectTarget(true);
            }

            IsAiming = HasTarget || noTargetT <= resetAimStateIfNoTarget;
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

            if (CanAttack())
            {           
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

        public bool CanAttack()
        {
            if (m_Controller.HasTarget)
            {
                var gun = m_Inventory.GetGun();

                var targetPos = CurrentTarget.position;
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
