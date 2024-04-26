using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class Room : MonoBehaviour
    {
        public PhysicsCallbacks rootTrigger;

        public Wall[] doDeactivateAfterEnterRoom;
        public bool activateWithRoomTrigger;
        private Renderer[] m_RoomRender;
        public GameObject[] hideRoomObjects;
        public event Action<Collider> onEnterRoom = delegate { };
        public event Action<Collider> onExitRoom = delegate { };

        [System.NonSerialized]
        public HashSet<Collider> RoomItems = new HashSet<Collider>();

        private void Awake()
        {
            rootTrigger.onTriggerEnter += OnEnterRoom;
            rootTrigger.onTriggerExit += OnExitRoom;
            m_RoomRender = GetComponentsInChildren<Renderer>();
        }

        public void EnableRender(bool enable)
        {
            foreach (var item in m_RoomRender)
            {
                item.enabled = enable;
            }
        }

        private void OnExitRoom(Collider obj)
        {
            RoomItems.Remove(obj);
            onExitRoom.Invoke(obj);

            if (!activateWithRoomTrigger)
                return;

            if (obj.GetComponent<PlayerInput>())
            {
                EnableTopDownRender(false);
            }
        }

        private void OnEnterRoom(Collider obj)
        {
            foreach (var item in hideRoomObjects)
            {
                item.gameObject.SetActive(false);
            }

            RoomItems.Add(obj);
            onEnterRoom.Invoke(obj);

            if (!activateWithRoomTrigger)
                return;

            if (obj.GetComponent<PlayerInput>())
            {
                EnableTopDownRender(true);
            }
        }

        public void EnableTopDownRender(bool enable)
        {
            foreach (var item in doDeactivateAfterEnterRoom)
            {
                item.EnableTopDownRender(enable);
            }
        }
    }
}
