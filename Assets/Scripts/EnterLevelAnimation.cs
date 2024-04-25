using DG.Tweening;
using MoreMountains.Tools;
using Prototype;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Prototype
{
    public class EnterLevelAnimation : MonoBehaviour
    {
        public Transform playerSpawnPoint;
        public CarControl carControl;
        private CameraController m_cameraController;
        public FadeScreenUI fader;
        private IPlayerFactory m_playerFactory;
        public float carEnterCarInput;
        public float carExitCarInput;
        public RequiredResourcesBehaviour requiredResourcesBehaviour;
        public PhysicsCallbacks stopCarTrigger;

        [Inject]
        void Construct(CameraController cameraController, IPlayerFactory playerFactory)
        {
            m_cameraController = cameraController;
            m_playerFactory = playerFactory;
        }

        private void Start()
        {
            EnterArea();
        }

        private void EnterArea()
        {
            fader.ShowInstant();
            fader.Hide();

            carControl.blockWheels = false;
            carControl.vInput = carEnterCarInput;
            m_cameraController.PushTarget(stopCarTrigger.transform, true);

            requiredResourcesBehaviour.onFinished += LeaveArea;
            requiredResourcesBehaviour.gameObject.SetActive(false);
            stopCarTrigger.onTriggerEnter += StopCarTrigger_onTriggerEnter;
        }

        private void StopCarTrigger_onTriggerEnter(Collider obj)
        {
            if (!obj.attachedRigidbody || !obj.attachedRigidbody.GetComponent<CarControl>())
                return;

            stopCarTrigger.gameObject.SetActive(false);
            carControl.blockWheels = true;
            DOVirtual.DelayedCall(1.5f, () =>
            {
                requiredResourcesBehaviour.gameObject.SetActive(true);
                carControl.Freeze(true);
                var instance = m_playerFactory.SpawnAtPosition(playerSpawnPoint.position);
                instance.transform.forward = playerSpawnPoint.forward;
                m_cameraController.PopTarget();
            });
        }

        private void LeaveArea()
        {
            m_cameraController.PushTarget(stopCarTrigger.transform);
            m_playerFactory.CurrentPlayerUnit.SetActive(false);

            DOVirtual.DelayedCall(1f, () =>
            {
                carControl.Freeze(false);
                carControl.blockWheels = false;
                carControl.vInput = carExitCarInput;
            });

            DOVirtual.DelayedCall(3f, () =>
            {
                fader.Show();
            });

            DOVirtual.DelayedCall(5f, () =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
