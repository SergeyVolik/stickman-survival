using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Prototype
{
    public class LootableObjectBehaviour : MonoBehaviour
    {
        public ResourceContainer[] lootTicksItems;
        public PhysicsCallbacks trigger;
        public float openDuration;
        private float openT;
        private IResourceHolder m_ResHolder;
        private Collider m_triggerdObject;
        bool continueOpening;
        bool isOpened;

        public int lootTicks => lootTicksItems.Length;
        public int executedLootTicks;
        public float finalDropDelay;

        [SerializeField]
        private Image m_OpenStateImage;
        private DropExecutor m_Drop;

        public MMF_Player startLootingFeedback;
        public MMF_Player endLootingFeedback;
        public MMF_Player lootTickFeedback;
        public MMF_Player lootEndedFeedback;

        public UnityEvent onLooted;

        private void Awake()
        {
            m_Drop = GetComponent<DropExecutor>();
            m_OpenStateImage.transform.parent.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            trigger.onTriggerEnter += Trigger_onTriggerEnter;
            trigger.onTriggerExit += Trigger_onTriggerExit;
        }

        private void OnDisable()
        {
            trigger.onTriggerEnter -= Trigger_onTriggerEnter;
            trigger.onTriggerExit -= Trigger_onTriggerExit;
        }

        private void Trigger_onTriggerExit(Collider obj)
        {
            if (isOpened)
                return;

            continueOpening = false;
            ResetCharacterData();
        }

        private void ResetCharacterData()
        {
            m_triggerdObject = null;
            endLootingFeedback?.PlayFeedbacks();
        }

        private void Trigger_onTriggerEnter(Collider obj)
        {
            if (isOpened)
                return;

            m_OpenStateImage.transform.parent.gameObject.SetActive(true);
            if (obj.TryGetComponent<CharacterCombatState>(out var combat))
            {
                if (combat.InCombat)
                    return;
            }

            m_ResHolder = obj.GetComponent<IResourceHolder>();
            startLootingFeedback?.PlayFeedbacks();

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

            m_OpenStateImage.fillAmount = (openT / openDuration) / lootTicks + executedLootTicks / (float)lootTicks;

            if (openT >= openDuration)
            {
                LootTickFinished();
            }
        }

        private void LootTickFinished()
        {
            lootTickFeedback?.PlayFeedbacks();
            m_Drop.ExecuteDrop(m_triggerdObject.gameObject, lootTicksItems[executedLootTicks]);

            executedLootTicks++;
            openT = 0;

            if (executedLootTicks == lootTicks)
            {
                m_OpenStateImage.transform.parent.gameObject.SetActive(false);
                continueOpening = false;
                isOpened = true;
                ResetCharacterData();
                lootEndedFeedback?.PlayFeedbacks();
                onLooted.Invoke();
                enabled = false;
            }
        }
    }
}