using Prototype;
using UnityEngine;

public class Room : MonoBehaviour
{
    public PhysicsCallbacks rootTrigger;

    public Wall[] doDeactivateAfterEnterRoom;

    private void Awake()
    {
        rootTrigger.onTriggerEnter += RootTrigger_onTriggerEnter;
        rootTrigger.onTriggerExit += RootTrigger_onTriggerExit;
    }

    private void RootTrigger_onTriggerExit(Collider obj)
    {
        if (obj.GetComponent<PlayerInput>())
        {
            foreach (var item in doDeactivateAfterEnterRoom)
            {
                item.Show();
            }
        }
    }

    private void RootTrigger_onTriggerEnter(Collider obj)
    {
        if (obj.GetComponent<PlayerInput>())
        {
            foreach (var item in doDeactivateAfterEnterRoom)
            {
                item.Hide();
            }
        }
    }
}
