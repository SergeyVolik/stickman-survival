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

    public BaseQuest GetCurrentQuest() => currentQuest > quests.Length-1 ? null : quests[currentQuest];
    private void SetupCurrentQuest()
    {
        if (GetCurrentQuest() == null)
            return;

        quests[currentQuest].Setup(questUISpawnPoint);
        quests[currentQuest].onQuestFinished += NextQuest;
    }

    public void ShowCurrentQuestTarget()
    {
        GetCurrentQuest()?.ShowTarget();
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
