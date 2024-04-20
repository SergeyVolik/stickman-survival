using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Prototype
{
    public interface IQuest
    {
        public string QuestName { get; }
        public string QuestDescription { get; }
        public event Action onQuestFinished;
        public event Action onQuestChanged;
        public bool IsFinished();
        public void UpdateQuest();
        public void FinishQuest();

        public Transform GetQuestTargetObject();
        public IEnumerable<Transform> GetQuestTargetObjects();
    }

    public abstract class BaseQuest : MonoBehaviour, IQuest
    {
        public event Action onQuestFinished;
        public event Action onQuestChanged;
        public UnityEvent onQuestFinishedUE;
        [SerializeField]
        private GameObject questUI;
        private CameraController m_cameraContr;

        [field: SerializeField]
        public string QuestName { get; set; }

        [field:SerializeField]
        public string QuestDescription { get; set; }
        protected GameObject m_CurrentQuestUI;

        public virtual void FinishQuest()
        {
            onQuestFinished?.Invoke();
            onQuestFinishedUE.Invoke();
        }

        public virtual void UpdateQuest()
        {
            onQuestChanged?.Invoke();

            if (IsFinished())
            {
                FinishQuest();
            }
        }

        public void ShowTarget()
        {
            var questTarget = GetQuestTargetObject();
            if (questTarget)
            {
                m_cameraContr.PushTargetWithDuration(questTarget, 2f);


                var targets = GetQuestTargetObjects();

                foreach (var target in targets)
                {
                    var heighlight = target.GetComponentInChildren<HeighlightFeedback>();
                    if (heighlight)
                    {
                        heighlight.Feedback.PlayFeedbacks();
                    }
                }              
            }
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
                ShowTarget();
            });        
        }

        public virtual void Clear()
        {
            GameObject.Destroy(m_CurrentQuestUI);
        }

        protected virtual void Awake()
        { }

        public virtual Transform GetQuestTargetObject()
        {
            return null;
        }

        public abstract bool IsFinished();

        public virtual IEnumerable<Transform> GetQuestTargetObjects()
        {
            return Array.Empty<Transform>();
        }
    }

    public class CollectItemQuest : BaseQuest, IQuest
    {
        public GameObject CollectableObjectInstance;

        bool finished = false;
        protected override void Awake()
        {
            base.Awake();
            CollectableObjectInstance.GetComponent<ICollectable>().onCollected += CollectItemQuest_onCollected;
        }
        public override Transform GetQuestTargetObject()
        {
            if(CollectableObjectInstance == null)
                return null;

            return CollectableObjectInstance.transform;
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
           yield return CollectableObjectInstance.transform;
        }
        private void CollectItemQuest_onCollected()
        {
            finished = true;
            UpdateQuest();
        }

        public override bool IsFinished()
        {
            return finished;
        }
    }
}
