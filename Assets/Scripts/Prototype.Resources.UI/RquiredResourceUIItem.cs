using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class RquiredResourceUIItem : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI itemText;
        public Image spriteImage;
        public Button addResource;
        public MMF_Player feedback;

        public void DoAnimation()
        {
            feedback?.PlayFeedbacks();
        }

        public void SetText(string value)
        {
            itemText.text = value;
        }

        public void SetSprite(Sprite sprite, Color color)
        {
            spriteImage.color = color;
            spriteImage.sprite = sprite;
        }
        public void SetSprite(Sprite sprite)
        {
            spriteImage.sprite = sprite;
        }
    }
}
