using System;
using UnityEngine;

namespace Prototype
{
    public class CollectableHealtBox : MonoBehaviour, ICollectable
    {
        public event Action onCollected;

        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<HealthData>(out var data))
            {
                onCollected?.Invoke();
                data.DoHeal(data.maxHealth, gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}

