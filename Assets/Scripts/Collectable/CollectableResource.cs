using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CollectableResource : MonoBehaviour, ICollectable
    {
        public ResourceTypeSO resource;
        public int count = 1;
        private PlayerResources m_pRes;

        [Inject]
        void Construct(PlayerResources pRes)
        {
            m_pRes = pRes;
        }

        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<PlayerResourceHolder>(out var behaviour))
            {
                m_pRes.resources.AddResource(resource, count);
                gameObject.SetActive(false);
            }
        }
    }
}