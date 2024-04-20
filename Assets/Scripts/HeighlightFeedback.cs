using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class HeighlightFeedback : MonoBehaviour
    {
        private void Awake()
        {
            Feedback = GetComponent<MMF_Player>();
        }

        public MMF_Player Feedback { get; private set; }
    }
}
