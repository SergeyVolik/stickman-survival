using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
        private Rigidbody m_Rb;
        private Transform m_Transform;
        public float speed = 2;
        public float accelerationSpeed = 30;
        public bool canAim;
        public bool IsAiming { get; private set; }
        public bool HasTarget { get; private set; }

        public LayerMask CastMask;
        public Vector2 AimVector;
        public float aimDistance = 5f;
        public float rotationSpeed;
        public float criticalDistanceChangeTarget;

        public Vector2 MoveInput { get => m_MoveInput; set => m_MoveInput = value; }
        RaycastHit[] results;

        private void Awake()
        {
            m_Rb = GetComponent<Rigidbody>();
            m_Transform = transform;
            results = new RaycastHit[10];
        }

        HealthData m_LastTargetUnit;

        float noTargetT = 100;
        float resetAimStateIfNoTarget = 1f;

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            var normalized = MoveInput.normalized;
            var moveVec = normalized;
            var moveVec3 = new Vector3(moveVec.x, 0, moveVec.y);

            noTargetT += deltaTime;

            var currentPos = m_Transform.position;

            Quaternion currentRotation = m_Transform.rotation;
            Quaternion newROtation = currentRotation;

            float rotaValue = Mathf.Clamp01(deltaTime * rotationSpeed);

            if (IsAiming == false && MoveInput != Vector2.zero)
            {
                newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(moveVec3), rotaValue);
            }
          
            if (canAim)
            {
                //update target
                {
                    if (m_LastTargetUnit)
                    {
                        var dist = Vector3.Distance(m_LastTargetUnit.transform.position, currentPos);

                        if (m_LastTargetUnit.IsDead)
                        {
                            m_LastTargetUnit = null;
                        }
                        else if (dist > aimDistance)
                        {
                            m_LastTargetUnit = null;
                        }

                        var data = GetTargetWithClosestDistance();

                        if (data.closestDistance < criticalDistanceChangeTarget)
                        {
                            m_LastTargetUnit = data.body.transform.GetComponent<HealthData>();
                        }

                        HasTarget = m_LastTargetUnit != null;
                    }

                    //change target
                    if (!HasTarget)
                    {                       
                        var data = GetTargetWithClosestDistance();

                        HasTarget = data.body;

                        if (HasTarget)
                        {
                            noTargetT = 0;
                           
                            m_LastTargetUnit = data.body.transform.GetComponent<HealthData>();
                        }
                    }
                }

                //update aim data
                if (HasTarget)
                {
                    var point = m_LastTargetUnit.transform.position;

                    point.y = currentPos.y;
                    var vector = point - currentPos;

                    AimVector = new Vector2(vector.x, vector.z);
                    newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(vector), rotaValue);
                }
            }

            IsAiming = HasTarget || noTargetT < resetAimStateIfNoTarget;

            var currentVel = m_Rb.velocity;
            currentVel.y = 0;


          
            m_Rb.AddForce(moveVec3 * accelerationSpeed, mode: ForceMode.Acceleration);

            if (Vector3.Dot(currentVel.normalized, moveVec3) < -0.8f)
            {
                m_Rb.velocity = Vector3.ClampMagnitude(m_Rb.velocity, speed*10);
            }
            else {
                m_Rb.velocity = Vector3.ClampMagnitude(m_Rb.velocity, speed);
            }
          
            m_Transform.rotation = newROtation;
        }

        private (Rigidbody body, float closestDistance) GetTargetWithClosestDistance()
        {
            Vector3 currentPos = m_Transform.position;

            int count = Physics.SphereCastNonAlloc(currentPos, aimDistance, Vector3.up, results, 10, CastMask);

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