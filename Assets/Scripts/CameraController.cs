using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform m_CameraTarget;

        public float lookAheadSpeed;
        public float lookAheadOffset;

        public float lookAheadSpeedIdle;
        public float lookAheadOffsetIdle;

        private float m_CurrentlookAheadSpeed;
        private float m_CurrentlookAheadOffset;

        public GameObject combatVCamera;
        public GameObject idleVCamera;

        private IPlayerFactory m_PlayerFactory;

        [Inject]
        public void Construct(IPlayerFactory factory)
        {
            m_PlayerFactory = factory;
        }

        private void Awake()
        {
            ActivateIdleCamera();
        }

        private void FixedUpdate()
        {
            if (m_PlayerFactory.CurrentPlayerUnit)
            {
                var uniTrans = m_PlayerFactory.CurrentPlayerUnit.transform;
                var unitPos = uniTrans.position;
                var unitForward = uniTrans.forward;
                var offset = unitForward * m_CurrentlookAheadOffset;

                m_CameraTarget.position = Vector3.Lerp(m_CameraTarget.position, unitPos + offset, Time.deltaTime * m_CurrentlookAheadSpeed);
            }
        }

        [Button]
        public void ActivateCombatCamera()
        {
            m_CurrentlookAheadSpeed = lookAheadSpeed;
            m_CurrentlookAheadOffset = lookAheadOffset;

            combatVCamera.SetActive(true);
            idleVCamera.SetActive(false);
        }

        [Button]
        public void ActivateIdleCamera()
        {
            m_CurrentlookAheadSpeed = lookAheadSpeedIdle;
            m_CurrentlookAheadOffset = lookAheadOffsetIdle;

            combatVCamera.SetActive(false);
            idleVCamera.SetActive(true);
        }
    }
}