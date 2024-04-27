using UnityEngine;
using Zenject;

namespace Prototype
{
    public class QuestQueue : MonoBehaviour
    {
        public BaseQuest[] quests;
        public int currentQuest;
        public Transform questUISpawnPoint;
        private IPlayerFactory m_playerFactory;

        private void Awake()
        {
            questUISpawnPoint.gameObject.SetActive(false);
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
            questUISpawnPoint.gameObject.SetActive(true);
            currentQuest = 0;
            SetupCurrentQuest();
        }

        public BaseQuest GetCurrentQuest() => currentQuest > quests.Length - 1 ? null : quests[currentQuest];
        private void SetupCurrentQuest()
        {
            if (GetCurrentQuest() == null)
                return;
         
            quests[currentQuest].Setup(questUISpawnPoint);
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
            ClearPrevQuest();
            currentQuest++;
            SetupCurrentQuest();
        }
    }
}
