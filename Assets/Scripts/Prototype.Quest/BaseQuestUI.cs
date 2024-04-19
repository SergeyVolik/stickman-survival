using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public abstract class BaseQuestUI : MonoBehaviour
    {
        public Button showTargetButton;
        public abstract void Bind(IQuest quest);

        public abstract void UpdateDescription();
    }
}
