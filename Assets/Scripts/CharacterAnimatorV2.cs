using UnityEngine;

public class CharacterAnimatorV2 : MonoBehaviour
{
    private Animator m_Animator;
    private CustomCharacterController m_CharacterInput;
    private Transform m_ControllerTrans;

    private void Awake()
    {
       m_Animator = GetComponent<Animator>();
        m_CharacterInput = GetComponentInParent<CustomCharacterController>();
        m_ControllerTrans = m_CharacterInput.transform;
    }
    public void Move(Vector2 vector)
    {
        m_Animator.SetBool("Move", vector != Vector2.zero);
    }

    private void Update()
    {
        var delta = Time.deltaTime;

        Move(m_CharacterInput.MoveInput);

        if (m_CharacterInput.canAim)
        {
            m_Animator.SetBool("IsAim", m_CharacterInput.IsAiming);

            var vector = m_ControllerTrans.InverseTransformDirection(new Vector3(m_CharacterInput.MoveInput.x, 0, m_CharacterInput.MoveInput.y)).normalized;

            var forwardAmount = vector.z;
            var turnAmount = vector.x;

            m_Animator.SetFloat("Turn", turnAmount, 0.1f, delta);
            m_Animator.SetFloat("Forward", forwardAmount, 0.1f, delta);
        }
    }
}