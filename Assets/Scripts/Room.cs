using Prototype;
using UnityEngine;

public class Room : MonoBehaviour
{
    public PhysicsCallbacks rootTrigger;

    public Wall[] doDeactivateAfterEnterRoom;
    public bool activateWithRoomTrigger;
    private void Awake()
    {
        rootTrigger.onTriggerEnter += RootTrigger_onTriggerEnter;
        rootTrigger.onTriggerExit += RootTrigger_onTriggerExit;
    }

    private void RootTrigger_onTriggerExit(Collider obj)
    {
        if (!activateWithRoomTrigger)
            return;

        if (obj.GetComponent<PlayerInput>())
        {
            HideRoom();
        }
    }

    public void ShowRoom()
    {
        foreach (var item in doDeactivateAfterEnterRoom)
        {
            item.Show();
        }
    }

    private void RootTrigger_onTriggerEnter(Collider obj)
    {
        if (!activateWithRoomTrigger)
            return;

        if (obj.GetComponent<PlayerInput>())
        {
            ShowRoom();
        }
    }

    public void HideRoom()
    {
        foreach (var item in doDeactivateAfterEnterRoom)
        {
            item.Hide();
        }
    }
}
