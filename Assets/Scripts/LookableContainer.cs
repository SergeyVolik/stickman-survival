using MoreMountains.Feedbacks;
using UnityEngine;

namespace Prototype
{
    public class LookableContainer : MonoBehaviour
    {
        PhysicsCallbacks trigger;
        public MMF_Player lookFeedback;

        private void Awake()
        {
            trigger.onTriggerEnter += Trigger_onTriggerEnter;
            trigger.onTriggerExit += Trigger_onTriggerEnter;
        }

        private void Trigger_onTriggerEnter(Collider obj)
        {
            throw new System.NotImplementedException();
        }
    }

}
