using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class House : MonoBehaviour
    {
        [System.NonSerialized]
        public Floor[] floors;

        [System.NonSerialized]
        public List<Collider> HouseObjects = new List<Collider>();

        public void EnableRender(bool enable)
        {
            foreach (var item in floors)
            {
                item.EnableRender(enable);
            }
        }

        private void OnEnterHouse(Collider col)
        {
            Debug.Log("Enter House");
        }

        private void OnExitHouse(Collider collider)
        {
            Debug.Log("Exit House");

            if (IsPlayer(collider))
            {
                EnableRender(true);
            }
        }

        private void Awake()
        {
            floors = GetComponentsInChildren<Floor>();

            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].Setup();

                int currentFloorIndex = i;

                floors[i].onFloorExited += (col) =>
                {
                    HouseObjects.Remove(col);

                    if (!HouseObjects.Contains(col))
                    {
                        OnExitHouse(col);
                    }
                };

                floors[i].onFloorEntered += (collider) => {

                    if (!HouseObjects.Contains(collider))
                    {
                        OnEnterHouse(collider);
                    }

                    HouseObjects.Add(collider);

                    if (IsPlayer(collider))
                    {
                        for (int j = 0; j < floors.Length; j++)
                        {
                            if (j > currentFloorIndex)
                            {
                                floors[j].EnableRender(false);
                            }
                            else if (j == currentFloorIndex)
                            {
                                floors[j].EnableTopDownRender(true);
                            }
                            else if (j < currentFloorIndex)
                            {
                                floors[j].EnableRender(true);
                            }
                        }
                    }
                };                   
            }
        }

        private static PlayerInput IsPlayer(Collider collider)
        {
            return collider.GetComponent<PlayerInput>();
        }
    }
}
