using Prototype;
using System;
using UnityEngine;

public class FarmableStackObjects : MonoBehaviour
{
    public HealthData[] stackableObjects;

    private void Awake()
    {
        if (stackableObjects.Length <= 1)
            return;

        for (int i = 0; i < stackableObjects.Length; i++)
        {
            stackableObjects[i].IsDamageable = false;
        }

        stackableObjects[0].IsDamageable = true;

        for (int i = 0; i < stackableObjects.Length - 1; i++)
        {
            int index = i + 1;
            stackableObjects[i].onDeath += () =>
            {
                stackableObjects[index].IsDamageable = true;
            };
        }
    }
}
