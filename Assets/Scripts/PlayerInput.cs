using Prototype;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputReader m_reader;
    private CustomCharacterController m_Input;
    private Transform m_Transform;
    public float speed;

    [Inject]
    void Construct(PlayerInputReader reader)
    {
        m_reader = reader;
    }

    private void Awake()
    {
        m_Input = GetComponent<CustomCharacterController>();

        m_Transform = transform;
    }

    private void Update()
    {
        var input = m_reader.ReadMoveInput();

        m_Input.MoveInput = input;
    }
}
