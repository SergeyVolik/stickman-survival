using Prototype;
using UnityEngine;

public class QuestQueue : MonoBehaviour
{
    public BaseQuest[] quests;
    public int currentQuest;
    public Transform questUISpawnPoint;

    private void Awake()
    {
        SetupCurrentQuest();      
    }

    private void SetupCurrentQuest()
    {
        if (currentQuest == quests.Length)
            return;
        quests[currentQuest].Setup(questUISpawnPoint);
        quests[currentQuest].onQuestFinished += NextQuest;
    }

    private void ClearPrevQuest()
    {
        quests[currentQuest].Clear();
        quests[currentQuest].onQuestFinished-= NextQuest;
    }

    private void NextQuest()
    {
        ClearPrevQuest();
        currentQuest++;
        SetupCurrentQuest();
    }
}
