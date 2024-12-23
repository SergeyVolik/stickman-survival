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
        private CharacterStats m_Stats;
        private Transform m_LastTarget;
        public float AimDistance => m_Inventory.HasGun() ? m_Inventory.GetGun().aimDistance : 0;
        public LayerMask CastMask;
        public LayerMask WallMask;
        public bool canAim;
        RaycastHit[] results;
        public float aimSpeed;
        public bool HasTarget => CurrentTargetHealth != null;

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

            int count = PhysicsHelper.GetAllTargetWithoutWalls(m_Transform, results, AimDistance, CastMask, WallMask, 0.5f);

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
            var currentTargetHealth = targetNew.GetComponent<HealthData>();

            if (currentTargetHealth)
            {
                //Debug.Log("new target finded");
                CurrentTargetHealth = currentTargetHealth;
                CurrentTarget = targetNew;
            }
        }

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
            m_Stats = GetComponent<CharacterStats>();
            m_CombatState.onCombatState += UpdateCombatState;
            results = new RaycastHit[10];
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
            m_Controller.AimVector = Vector2.zero;
            m_Controller.ResetDefaultRotatonSpeed();
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

        public void FindTarget()
        {
            var currentPos = m_Transform.position;

            if (canAim)
            {
                //Debug.Log("Need Aim");
                if (CurrentTargetHealth)
                {
                    //Debug.Log("UpdateCurretn Target");
                    var dist = Vector3.Distance(CurrentTargetHealth.transform.position, currentPos);

                    if (CurrentTargetHealth.IsDead || dist > AimDistance)
                    {
                        ResetTarget();
                    }
                }

                //change target
                if (!HasTarget || m_Controller.IsMoving)
                {
                    //Debug.Log("try find new target");
                    var data = GetTargetWithClosestDistance();
                    if (data.body)
                    {
                        UpdateTarget(data.body.transform);
                    }
                }
            }
            else
            {
                ResetTarget();
            }

            if (m_LastTarget != CurrentTarget)
            {
                SelectTarget(false);
                m_LastTarget = CurrentTarget;
                SelectTarget(true);
            }
        }

        private void ResetTarget()
        {
            m_Controller.AimVector = Vector2.zero;
            CurrentTargetHealth = null;
            CurrentTarget = null;
        }

        public bool IsLookingOnTarget()
        {
            if(m_Controller.AimVector == Vector2.zero)
                return false;

            var selfAimVector = m_Transform.forward;        
            var aimVec = new Vector3(m_Controller.AimVector.x, 0, m_Controller.AimVector.y);
            var dot = Vector3.Dot(selfAimVector.normalized, aimVec.normalized);
            return dot >= 0.95f;
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;

            m_Controller.rotationSpeed = aimSpeed;
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

            if (!currentGun && m_Inventory.HasGunInInventory())
            {
                m_Inventory.ActiveLastGun();
                m_AimCircle.Show();
            }

            currentGun = m_Inventory.CurrentGun;

            if (currentGun == null)
                return;

            bool isMoving = m_Controller.IsMoving;
            m_ShotT += deltaTime;

            if (!isMoving)
            {
                float interval = currentGun.standingshotInterval / m_Stats.attackSpeedMult;
                AimOnTarget();

                if (HasTarget && IsLookingOnTarget())
                {
                    if (m_ShotT > interval)
                    {
                        m_ShotT = 0;
                        currentGun.critChanse = m_Stats.critChance;
                        currentGun.critMult = m_Stats.critMult;
                        currentGun.damagMult = m_Stats.rangeWeaponDamageMult;
                        currentGun.ShotOnTarget(CurrentTarget);
                        m_CharacterAnimator.Shot();
                    }
                }
            }
        }

        private void AimOnTarget()
        {
            if (HasTarget)
            {
                var point = CurrentTargetHealth.transform.position;
                m_Controller.AimVector = Vector2.zero;
                var currentPos = m_Transform.position;
                point.y = currentPos.y;
                var vector = point - currentPos;
                m_Controller.AimVector = new Vector2(vector.x, vector.z);
            }
            else 
            {
                m_Controller.AimVector = new Vector2();
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
    }
}
