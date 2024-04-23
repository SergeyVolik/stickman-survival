using Prototype;
using TMPro;

public class DestroyObjectsQuestUI : BaseQuestUI
{
   public TextMeshProUGUI questText;
   private IQuest m_quest;

    public override void Bind(IQuest quest)
    {
        m_quest = quest;
        quest.onQuestChanged += Quest_onQuestChanged;
        UpdateDescription();
    }

    private void Quest_onQuestChanged()
    {
        UpdateDescription();
    }

    public override void UpdateDescription()
    {
        if (m_quest is IDestroyObjectsQuest data)
        {
            questText.text = $"{m_quest.QuestName} {data.AlreadyKiller()}/{data.TargetKills()}";
        }
    }
}
