using Prototype;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform m_CameraTarget;

        public float lookAheadSpeed;
        public float lookAheadOffset;

        private IPlayerFactory m_PlayerFactory;

        [Inject]
        public void Construct(IPlayerFactory factory)
        {
            m_PlayerFactory = factory;
        }

        private void FixedUpdate()
        {
            if (m_PlayerFactory.CurrentPlayerUnit)
            {
                var uniTrans = m_PlayerFactory.CurrentPlayerUnit.transform;
                var unitPos = uniTrans.position;
                var unitForward = uniTrans.forward;
                var offset = unitForward * lookAheadOffset;

                m_CameraTarget.position = Vector3.Lerp(m_CameraTarget.position, unitPos + offset, Time.deltaTime * lookAheadSpeed);
            }
        }
    }

}