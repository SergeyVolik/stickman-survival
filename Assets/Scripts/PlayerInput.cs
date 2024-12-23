using UnityEngine;
using Zenject;

namespace Prototype
{
    public class PlayerInput : MonoBehaviour
    {
        private IPlayerInputReader m_reader;
        private CustomCharacterController m_Input;

        [Inject]
        void Construct(IPlayerInputReader reader)
        {
            m_reader = reader;
        }

        private void Awake()
        {
            m_Input = GetComponent<CustomCharacterController>();
            GetComponent<HealthData>().onDeath += () =>
            {
                enabled = false;
            };
        }

        private void Update()
        {
            var input = m_reader.ReadMoveInput();

            m_Input.MoveInput = input;
        }
    }
}