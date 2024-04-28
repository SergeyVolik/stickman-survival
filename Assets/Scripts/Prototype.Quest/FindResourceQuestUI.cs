using TMPro;
using UnityEngine.UI;

namespace Prototype
{
    public class FindResourceQuestUI : BaseQuestUI
    {
        public TextMeshProUGUI questText;
        public Image resourceSprite;

        private IQuest m_quest;
        private PlayerResources m_playerResoruces;
        private IRequiredResourceContainer m_requiredResources;

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
            if (m_playerResoruces == null)
            {
                return;
            }

            foreach (var item in m_requiredResources.RequiredResources.ResourceIterator())
            {
                var playerReosurce = m_playerResoruces.resources.GetResource(item.Key);
                if (item.Value != 0 && playerReosurce < item.Value)
                {
                    resourceSprite.sprite = item.Key.resourceIcon;
                    questText.text = $"Find {item.Value - playerReosurce,0}";
                }
            }
        }

        internal void Construct(PlayerResources playerResources, IRequiredResourceContainer requiredResources)
        {
            m_playerResoruces = playerResources;
            m_requiredResources = requiredResources;
        }
    }
}