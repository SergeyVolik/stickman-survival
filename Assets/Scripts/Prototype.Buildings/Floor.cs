using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{   
    public class Floor : MonoBehaviour
    {
        [System.NonSerialized]
        public Room[] rooms;

        public event Action<Collider> onFloorEntered = delegate { };
        public event Action<Collider> onFloorExited = delegate { };

        [System.NonSerialized]
        public List<Collider> FloorItems = new List<Collider>();

        private void Awake()
        {
            rooms = GetComponentsInChildren<Room>();
        }

        public void Setup()
        {
            foreach (Room room in rooms)
            {
                room.onEnterRoom += (col) =>
                {
                    if (!FloorItems.Contains(col))
                        onFloorEntered.Invoke(col);

                    FloorItems.Add(col);
                };

                room.onExitRoom += (col) =>
                {
                    FloorItems.Remove(col);

                    if (!FloorItems.Contains(col))
                        onFloorExited.Invoke(col);
                };
            }
        }

        public void EnableRender(bool enable)
        {
            foreach (var item in rooms)
            {
                item.EnableRender(enable);
            }
        }

        public void EnableTopDownRender(bool enable)
        {
            EnableRender(enable);

            foreach (var item in rooms)
            {
                item.EnableTopDownRender(enable);
            }
        }
    }
}