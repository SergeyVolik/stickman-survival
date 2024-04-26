using DG.Tweening;
using MoreMountains.Feedbacks;
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
        private AudioListenerController m_audioListener;
        public float carEnterCarInput;
        public float carExitCarInput;
        public RequiredResourcesBehaviour requiredResourcesBehaviour;
        public PhysicsCallbacks stopCarTrigger;
        public MMF_Player startEngineFeedback;
        public MMF_Player openDoorFeedback;
        public MMF_Player closeDoorFeedback;

        [Inject]
        void Construct(CameraController cameraController, IPlayerFactory playerFactory, AudioListenerController listenerCOntroller)
        {
            m_cameraController = cameraController;
            m_playerFactory = playerFactory;
            m_audioListener = listenerCOntroller;
        }

        private void Start()
        {
            EnterArea();
        }

        private void EnterArea()
        {
            fader.ShowInstant();
            fader.Hide();
            m_audioListener.audioListenerTarget = stopCarTrigger.transform;
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
            carControl.vInput = 0;
            DOVirtual.DelayedCall(1.5f, () =>
            {
                openDoorFeedback?.PlayFeedbacks();
                requiredResourcesBehaviour.gameObject.SetActive(true);
                carControl.Freeze(true);
                var m_Playerinstance = m_playerFactory.SpawnAtPosition(playerSpawnPoint.position);
                m_audioListener.audioListenerTarget = m_Playerinstance.transform;
                m_Playerinstance.transform.forward = playerSpawnPoint.forward;
                m_cameraController.PopTarget();
            });
        }

        private void LeaveArea()
        {
            m_cameraController.PushTarget(stopCarTrigger.transform);
            m_playerFactory.CurrentPlayerUnit.SetActive(false);
            startEngineFeedback?.PlayFeedbacks();
            closeDoorFeedback?.PlayFeedbacks();
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
