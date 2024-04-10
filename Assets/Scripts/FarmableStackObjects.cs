using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    [System.Serializable]
    public class FarmableStackItem
    {
        public HealthData[] damageable;
        private int deadTargets;
        public event Action onAllDead = delegate { };

        public void Setup()
        {
            foreach (var item in damageable)
            {
                item.onDeath += () =>
                {
                    deadTargets++;

                    if (deadTargets == damageable.Length)
                        onAllDead.Invoke();
                };
            }
        }

        public void SetAllDamageable(bool enable)
        {
            foreach (var item in damageable) {
                item.IsDamageable = enable;
            }
        }
    }

    public class FarmableStackObjects : MonoBehaviour
    {
        public FarmableStackItem[] stackableObjects;

        public UnityEvent onAllKilled;
        private int killed;

        private void Awake()
        {
            if (stackableObjects.Length <= 1)
                return;

            for (int i = 0; i < stackableObjects.Length; i++)
            {
                stackableObjects[i].Setup();
                stackableObjects[i].onAllDead += () => { 

                    killed++;
                    if (killed == stackableObjects.Length)
                        onAllKilled.Invoke();
                };

                stackableObjects[i].SetAllDamageable(false);
            }

            stackableObjects[0].SetAllDamageable(true);

            for (int i = 0; i < stackableObjects.Length - 1; i++)
            {
                int index = i + 1;
                stackableObjects[i].onAllDead += () =>
                {
                    stackableObjects[index].SetAllDamageable(true);       
                };
            }
        }
    }
}