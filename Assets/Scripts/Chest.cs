using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class Chest : MonoBehaviour
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

        private CharacterWithGunAnimator m_CharAnimator;
        private Outline[] m_Outlines;

        private void Awake()
        {
            m_Drop = GetComponent<DropExecutor>();

            trigger.onTriggerEnter += Trigger_onTriggerEnter;
            trigger.onTriggerExit += Trigger_onTriggerExit;

            m_Outlines = GetComponentsInChildren<Outline>();

            foreach (var item in m_Outlines)
            {
                item.enabled = false;
            }
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
            foreach (var item in m_Outlines)
            {
                item.enabled = false;
            }

            m_triggerdObject = null;

            if (m_CharAnimator)
            {
                m_CharAnimator.StartLooting(false);
                m_CharAnimator = null;
            }

            endLootingFeedback?.PlayFeedbacks();
        }

        private void Trigger_onTriggerEnter(Collider obj)
        {
            if (isOpened)
                return;

            if (obj.TryGetComponent<CharacterCombatState>(out var combat))
            {
                if (combat.InCombat)
                    return;
            }

            m_CharAnimator = obj.GetComponentInChildren<CharacterWithGunAnimator>();

            if (m_CharAnimator)
            {
                m_CharAnimator.StartLooting(true);
            }

            m_ResHolder = obj.GetComponent<IResourceHolder>();
            startLootingFeedback?.PlayFeedbacks();

            if (m_ResHolder == null)
            {
                return;
            }

            m_triggerdObject = obj;
            continueOpening = true;

            foreach (var item in m_Outlines)
            {
                item.enabled = true;
            }
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
                m_OpenStateImage.gameObject.SetActive(false);
                continueOpening = false;
                isOpened = true;
                ResetCharacterData();
                lootEndedFeedback?.PlayFeedbacks();
            }
        }
    }
}