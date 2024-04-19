using Prototype;
using System;
using TMPro;
using UnityEngine.UI;

public class FindResourceQuestUI : BaseQuestUI
{
    public TextMeshProUGUI questText;
    public Image resourceSprite;

    private IQuest m_quest;
    private PlayerResources m_resoruces;

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
        if (m_resoruces == null)
        {
            return;
        }

        if (m_quest is FindResourcesQuest data)
        {
            resourceSprite.sprite = data.resourcesToFind.resourceIcon;
            questText.text = $"Find {data.toFind - m_resoruces.resources.GetResource(data.resourcesToFind)}";
        }
    }

    internal void Construct(PlayerResources resoruces)
    {
        m_resoruces = resoruces;
    }
}
