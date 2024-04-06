using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class Chest : MonoBehaviour
    {
        public PhysicsCallbacks trigger;
        public float openDuration;
        private float openT;
        private IResourceHolder m_ResHolder;
        private Collider m_triggerdObject;
        bool continueOpening;
        bool isOpened;
      
        private TransferMoveManager m_movManager;

        public Animator chestAnimator;
      

        [SerializeField]
        private Image m_OpenStateImage;
        private DropExecutor m_Drop;
        public MMF_Player openFeedback;
        private void Awake()
        {
            m_Drop = GetComponent<DropExecutor>();

            trigger.onTriggerEnter += Trigger_onTriggerEnter;
            trigger.onTriggerExit += Trigger_onTriggerExit;
        }

        private void Trigger_onTriggerExit(Collider obj)
        {
            if (isOpened)
                return;

            continueOpening = false;
            m_triggerdObject = null;
        }

        private void Trigger_onTriggerEnter(Collider obj)
        {
            if (isOpened)
                return;

            m_ResHolder = obj.GetComponent<IResourceHolder>();


            if (m_ResHolder == null)
            {
                return;
            }

            m_triggerdObject = obj;
            continueOpening = true;
        }

        private void Update()
        {
            if (!continueOpening)
                return;

            openT += Time.deltaTime;

            m_OpenStateImage.fillAmount = openT / openDuration;

            if (openT >= openDuration)
            {
                OpenInternal();
            }
        }

        private void OpenInternal()
        {
            continueOpening = false;
            isOpened = true;
           
            m_OpenStateImage.gameObject.SetActive(false);
            chestAnimator.SetTrigger("Open");
        }

        private void OpenAfterAnimation()
        {
            openFeedback?.PlayFeedbacks();
            m_Drop.ExecuteDrop(m_triggerdObject.gameObject);
        }
    }
}


