using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    public float speed;

    public bool canAim;
    public bool IsAiming;

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
    private void Update()
    {
        var normalized = MoveInput.normalized;
        var moveVec = normalized * speed;
        m_Rb.velocity = new Vector3(moveVec.x, 0, moveVec.y);

        var currentPos = m_Transform.position;

        if (MoveInput != Vector2.zero)
        {
            m_Transform.forward = new Vector3(normalized.x, 0, normalized.y);
        }

        if (canAim)
        {
            int count = Physics.SphereCastNonAlloc(currentPos, castRadius, Vector3.up, results, 10, CastMask);
            IsAiming = count != 0;
            if (IsAiming)
            {
                RaycastHit closest = default;
                float closestDistance = float.MaxValue;

                for (int i = 0; i < count; i++)
                {
                    var dist = Vector3.Distance(currentPos, results[i].point);

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
                m_Transform.forward = vector.normalized;
            }
        }
    }
}
