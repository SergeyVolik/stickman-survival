using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public interface IQuest
    {
        public string QuestName { get; }
        public string QuestDescription { get; }

        public event Action onQuestFinished;
        public void FinishQuest();

        public GameObject GetQuestTargetObject();
    }

    public abstract class BaseQuest : MonoBehaviour, IQuest
    {
        public event Action onQuestFinished;
        [SerializeField]
        private GameObject questUI;
        private CameraController m_cameraContr;

        [field: SerializeField]
        public string QuestName { get; set; }

        [field:SerializeField]
        public string QuestDescription { get; set; }
        private GameObject m_CurrentQuestUI;

        public virtual void FinishQuest()
        {
            onQuestFinished.Invoke();
        }

        [Inject]
        void Construct(CameraController cameraContr)
        {
            m_cameraContr = cameraContr;
        }

        public virtual void Setup(Transform questUISpawnPoint)
        {
            m_CurrentQuestUI = GameObject.Instantiate(questUI, questUISpawnPoint);
            var questUIItem = m_CurrentQuestUI.GetComponent<BaseQuestUI>();
            questUIItem.GetComponent<BaseQuestUI>().Bind(this);

            questUIItem.showTargetButton.onClick.AddListener(() => {
                var traget = GetQuestTargetObject();
                if (traget)
                {
                    m_cameraContr.PushTargetWithDuration(traget.transform, 2f);
                }
            });
        }

        public virtual void Clear()
        {
            GameObject.Destroy(m_CurrentQuestUI);
        }

        protected virtual void Awake()
        { }

        public virtual GameObject GetQuestTargetObject()
        {
            return null;
        }
    }

    public class CollectItemQuest : BaseQuest, IQuest
    {
        public GameObject CollectableObjectInstance;

        protected override void Awake()
        {
            base.Awake();
            CollectableObjectInstance.GetComponent<ICollectable>().onCollected += CollectItemQuest_onCollected;
        }
        public override GameObject GetQuestTargetObject()
        {
            return CollectableObjectInstance;
        }
        private void CollectItemQuest_onCollected()
        {
            FinishQuest();
        }
    }
}
