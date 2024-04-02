using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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
    public float castRadius;
    public float rotationSpeed;

    public Vector2 MoveInput { get => m_MoveInput; set => m_MoveInput = value; }
    RaycastHit[] results;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Transform = transform;
        results = new RaycastHit[10];
    }

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
            int count = Physics.SphereCastNonAlloc(currentPos, castRadius, Vector3.up, results, 10, CastMask);

            HasTarget = count != 0;
            if (HasTarget)
            {
                noTargetT = 0;
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

                var point = closest.transform.position;
                point.y = currentPos.y;
                var vector = point - currentPos;

                AimVector = new Vector2(vector.x, vector.z);
                newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(vector), rotaValue);
            }
        }

        IsAiming = HasTarget || noTargetT < resetAimStateIfNoTarget;
        m_Rb.AddForce(moveVec3 * accelerationSpeed, mode: ForceMode.Acceleration);
        m_Rb.velocity = Vector3.ClampMagnitude(m_Rb.velocity, speed);
        m_Transform.rotation = newROtation;
    }
}
