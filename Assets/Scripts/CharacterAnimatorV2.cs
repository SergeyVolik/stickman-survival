using UnityEngine;

public class CharacterAnimatorV2 : MonoBehaviour
{
    private Animator m_Animator;
    private ICharacterInput m_CharacterInput;

    private void Awake()
    {
       m_Animator = GetComponent<Animator>();
        m_CharacterInput = GetComponentInParent<ICharacterInput>();
    }
    public void Move(Vector2 vector)
    {
        m_Animator.SetBool("Move", vector != Vector2.zero);
    }

    private void Update()
    {
        Move(m_CharacterInput.MoveInput);
    }
}