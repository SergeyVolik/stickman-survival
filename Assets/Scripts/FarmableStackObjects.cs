using Prototype;
using System;
using UnityEngine;

public class FarmableStackObjects : MonoBehaviour
{
    public FarmableObject[] farmableStack;

    private void Awake()
    {
        if (farmableStack.Length <= 1)
            return;

        for (int i = 0; i < farmableStack.Length; i++)
        {
            farmableStack[i].enabled = false;
        }

        farmableStack[0].enabled = true;

        for (int i = 0; i < farmableStack.Length - 1; i++)
        {
            int index = i + 1;
            farmableStack[i].GetComponent<HealthData>().onDeath += () =>
            {
                farmableStack[index].enabled = true;
            };
        }
    }
}
