using Prototype;
using TMPro;

public class TextQuestUI : BaseQuestUI
{
   public TextMeshProUGUI questText;
    private IQuest m_quest;

    public override void Bind(IQuest quest)
    {
        m_quest = quest;
        UpdateDescription();
    }

    public override void UpdateDescription()
    {
        questText.text = m_quest.QuestName;
    }
}
