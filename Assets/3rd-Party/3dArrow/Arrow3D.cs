using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class Arrow3D : MonoBehaviour
    {
        [SerializeField]
        MMF_Player m_ShowFeedback;
        [SerializeField]
        MMF_Player m_HideFeedback;

        public void Show()
        {
            m_ShowFeedback?.PlayFeedbacks();
        }

        public void Hide()
        {
            m_HideFeedback?.PlayFeedbacks();
        }
    }
}