using Prototype;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputReader m_reader;
    private Rigidbody m_Rb;
    private Transform m_Transform;
    public float speed;

    [Inject]
    void Construct(PlayerInputReader reader)
    {
        m_reader = reader;
    }

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_Transform = transform;
    }

    private void Update()
    {
        var input = m_reader.ReadMoveInput();

        var vector = new Vector3(input.x, 0, input.y);
        m_Rb.velocity = vector * speed;

        if(vector != Vector3.zero)
            m_Transform.forward = vector;
    }
}
