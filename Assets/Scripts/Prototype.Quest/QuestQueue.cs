using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class QuestQueue : MonoBehaviour
    {
        public BaseQuest[] quests;
        public int currentQuest;
        public QuestPanel questPanel;
        private IPlayerFactory m_playerFactory;

        public event Action<BaseQuest> onQuestInited = delegate { };
        private void Awake()
        {
            questPanel.gameObject.SetActive(false);
        }

        [Inject]
        void Construct(IPlayerFactory playerFactory)
        {
            m_playerFactory = playerFactory;
            m_playerFactory.onPlayerSpawned += M_playerFactory_onPlayerSpawned;
        }

        private void M_playerFactory_onPlayerSpawned(GameObject obj)
        {
            StartQuests();
        }

        private void OnDestroy()
        {
            m_playerFactory.onPlayerSpawned -= M_playerFactory_onPlayerSpawned;
        }

        public void StartQuests()
        {
            questPanel.gameObject.SetActive(true);
            currentQuest = 0;
            SetupCurrentQuest();
        }

        public BaseQuest GetCurrentQuest() => currentQuest > quests.Length - 1 ? null : quests[currentQuest];
        private void SetupCurrentQuest()
        {
            if (GetCurrentQuest() == null)
                return;

            questPanel.Show();

            quests[currentQuest].Setup(questPanel.transform);
            quests[currentQuest].onQuestFinished += NextQuest;
            quests[currentQuest].UpdateQuest();
        }

        public void ShowCurrentQuestTarget()
        {
            GetCurrentQuest()?.ShowTarget();
        }
        private void ClearPrevQuest()
        {
            quests[currentQuest].Clear();
            quests[currentQuest].onQuestFinished -= NextQuest;
        }

        private void NextQuest()
        {
            questPanel.Hide(() => {
                ClearPrevQuest();
                currentQuest++;
                onQuestInited.Invoke(GetCurrentQuest());
                SetupCurrentQuest();
            });          
        }
    }
}
