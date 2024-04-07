using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public PhysicsCallbacks physicsCallbacks;
    public bool isOpened;
    private void Awake()
    {
        physicsCallbacks.onTriggerEnter += PhysicsCallbacks_onTriggerEnter;
    }
    public MMF_Player doorOpenedFeedback;
    public void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
        doorOpenedFeedback?.PlayFeedbacks();
    }

    private void PhysicsCallbacks_onTriggerEnter(Collider obj)
    {
        if (isOpened)
            return;

        isOpened = true;

        OpenDoor();
    }
}
