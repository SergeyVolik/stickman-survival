using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CameraForceOffsetTrigger : MonoBehaviour
    {
        private CameraController m_Camera;
        public float cameraXOffset;

        private void Awake()
        {
            var callbacks = GetComponent<PhysicsCallbacks>();
            callbacks.onTriggerEnter += CameraForceOffset_onTriggerEnter;
            callbacks.onTriggerExit += Callbacks_onTriggerExit;
        }

        private void Callbacks_onTriggerExit(Collider obj)
        {
            m_Camera.RemoveOffset(gameObject);
        }

        private void CameraForceOffset_onTriggerEnter(Collider obj)
        {
            m_Camera.FourceOffset(gameObject, cameraXOffset);
        }

        [Inject]
        void Construct(CameraController controller)
        {
            m_Camera = controller;
        }
    }
}