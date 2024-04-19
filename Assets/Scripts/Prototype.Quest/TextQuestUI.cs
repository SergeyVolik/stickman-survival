using Prototype;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextQuestUI : BaseQuestUI
{
   public TextMeshProUGUI questText;

    public override void Bind(IQuest quest)
    {
        questText.text = quest.QuestName;
    }
}
