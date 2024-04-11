using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CameraForceOffsetTrigger : MonoBehaviour
    {
        private CameraController m_Camera;
        public float cameraXOffset;

        [Inject]
        void Construct(CameraController controller)
        {
            m_Camera = controller;
        }

        private void Awake()
        {
            var callbacks = GetComponent<PhysicsCallbacks>();
            callbacks.onTriggerEnter += CameraForceOffset_onTriggerEnter;
            callbacks.onTriggerExit += Callbacks_onTriggerExit;
        }

        private void Callbacks_onTriggerExit(Collider obj)
        {
            m_Camera.RemoveOffset(gameObject);
            Debug.Log("Camera Trigger Exit");
        }

        private void CameraForceOffset_onTriggerEnter(Collider obj)
        {
            m_Camera.FourceOffset(gameObject, cameraXOffset);
            Debug.Log("Camera Trigger Enter");
        }  
    }
}