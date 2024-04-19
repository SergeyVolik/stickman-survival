using System;
using UnityEngine;

namespace Prototype
{
    public interface ICollectable
    {
        public void Collect(GameObject collecteBy);
        public event Action onCollected;
    }

    public class ItemCollector : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICollectable>(out var item))
            {
                item.Collect(gameObject);
            }
        }
    }
}
