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
        public bool IsAiming => AimVector != Vector2.zero;
        public bool IsMoving => MoveInput != Vector2.zero;
        public Vector2 MoveInput { get => m_MoveInput; set => m_MoveInput = value; }

        public bool HasTarget { get; private set; }

        public Vector2 AimVector;
        public float rotationSpeed;

        private void Awake()
        {
            m_Rb = GetComponent<CharacterController>();
            m_Transform = transform;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            var normalizedMove = MoveInput.normalized;
            var moveVec3 = new Vector3(normalizedMove.x, 0, normalizedMove.y);

            Quaternion currentRotation = m_Transform.rotation;
            Quaternion newROtation = currentRotation;

            float rotaValue = Mathf.Clamp01(deltaTime * rotationSpeed);

            if (MoveInput != Vector2.zero)
            {
                newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(moveVec3), rotaValue);
            }
            else if(AimVector != Vector2.zero)
            {
                newROtation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(new Vector3(AimVector.x, 0, AimVector.y)), rotaValue);
            }
            
            m_Rb.SimpleMove(moveVec3 * speed);
            m_Transform.rotation = newROtation;
        }
    }
}