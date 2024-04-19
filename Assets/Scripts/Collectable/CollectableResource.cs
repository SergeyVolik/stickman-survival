using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CollectableResource : MonoBehaviour, ICollectable
    {
        public ResourceTypeSO resource;
        public int count = 1;
        private PlayerResources m_pRes;

        public event Action onCollected;

        [Inject]
        void Construct(PlayerResources pRes)
        {
            m_pRes = pRes;
        }

        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<PlayerResourceHolder>(out var behaviour))
            {
                onCollected?.Invoke();
                m_pRes.resources.AddResource(resource, count);
                gameObject.SetActive(false);
            }
        }
    }
}