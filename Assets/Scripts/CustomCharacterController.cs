using System;
using UnityEngine;

namespace Prototype
{
    public interface ICharacterInput
    {
        public Vector2 MoveInput { get; set; }
    }

    public class CustomCharacterController : MonoBehaviour, ICharacterInput
    {
        private Vector2 m_MoveInput;
        private CharacterController m_Rb;
        private Transform m_Transform;
        public float speed = 2;
        public float accelerationSpeed = 30;
        public float decelerationSpeed = 30;

        public bool canAim;
        public bool IsAiming { get; private set; }
        public bool IsMoving => MoveInput != Vector2.zero;

        [field: SerializeField]
        public bool HasTarget { get; private set; }

        public LayerMask CastMask;
        public LayerMask WallMask;

        public Vector2 AimVector;
        public float aimDistance = 5f;
        public float rotationSpeed;
        public float criticalDistanceChangeTarget;
        public float swithTargetInterval = 0.5f;
        private float m_LastSwithTarget;
        public bool standingOnlyAim = false;

        public Vector2 MoveInput { get => m_MoveInput; set => m_MoveInput = value; }

        RaycastHit[] results;

        private void Awake()
        {
            m_Rb = GetComponent<CharacterController>();
            m_Transform = transform;
            results = new RaycastHit[10];
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

        float noTargetT = 100;
        float resetAimStateIfNoTarget = 1f;

        void UpdateTarget(Transform targetNew)
        {
            CurrentTargetHealth = targetNew.GetComponent<HealthData>();
            if (CurrentTargetHealth)
            {
                CurrentTarget = targetNew;
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            var normalizedMove = MoveInput.normalized;
            var moveVec3 = new Vector3(normalizedMove.x, 0, normalizedMove.y);

            noTargetT += deltaTime;
            m_LastSwithTarget += deltaTime;

            var currentPos = m_Transform.position;

            Quaternion currentRotation = m_Transform.rotation;
            Quaternion newROtation = currentRotation;

            float rotaValue = Mathf.Clamp01(deltaTime * rotationSpeed);

            if (IsAiming == false && MoveInput != Vector2.zero)
            {
                newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(moveVec3), rotaValue);
            }

            var needAim = standingOnlyAim && MoveInput == Vector2.zero || !standingOnlyAim;

            if (canAim && needAim)
            {
                //update target
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
                        //Debug.Log($"HasTarget {HasTarget}");
                    }

                    //change target
                    if (!HasTarget)
                    {
                        //Debug.Log("Change Target");
                        var data = GetTargetWithClosestDistance();
                        HasTarget = data.body;

                        if (HasTarget)
                        {
                            noTargetT = 0;

                            UpdateTarget(data.body.transform);
                        }
                    }
                }

                //update aim data
                if (HasTarget)
                {
                    var point = CurrentTargetHealth.transform.position;

                    point.y = currentPos.y;
                    var vector = point - currentPos;

                    AimVector = new Vector2(vector.x, vector.z);
                    newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(vector), rotaValue);
                }
            }
            else
            {
                HasTarget = false;
                noTargetT = resetAimStateIfNoTarget + 1;
            }

            IsAiming = HasTarget || noTargetT <= resetAimStateIfNoTarget;
            m_Rb.SimpleMove(moveVec3 * speed);
            m_Transform.rotation = newROtation;
        }

        private (Rigidbody body, float closestDistance) GetTargetWithClosestDistance()
        {
            Vector3 currentPos = m_Transform.position;

            int count =  PhysicsHelper.GetAllTargetWithoutWalls(m_Transform, results, aimDistance, CastMask, WallMask, 0.5f);

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
    }
}