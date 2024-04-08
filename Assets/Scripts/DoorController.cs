using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class DoorController : MonoBehaviour
    {
        public Animator doorAnimator;
        public bool isOpened;
        public MMF_Player doorOpenedFeedback;

        public void OpenDoor()
        {
            if (isOpened)
                return;

            isOpened = true;

            doorAnimator.SetTrigger("Open");
            doorOpenedFeedback?.PlayFeedbacks();
        }
    }
}
