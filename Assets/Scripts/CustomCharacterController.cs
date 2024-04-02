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
    public Vector2 MoveInput { get => m_MoveInput; set => m_MoveInput = value; }

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Transform = transform;
    }
    private void Update()
    {
        var normalized = MoveInput.normalized;
        var moveVec = normalized * speed;
        m_Rb.velocity = new Vector3(moveVec.x, 0, moveVec.y);
        if (MoveInput != Vector2.zero)
        {
            m_Transform.forward = new Vector3(normalized.x, 0, normalized.y);
        }
    }
}
