using UnityEngine;
using Zenject;

namespace Prototype
{
    public static class DropHelper
    {
        public static bool TryDrop(GameObject obj, Vector3 spawnPos, ResourceContainer resources, TransferMoveManager transManager)
        {
            float moveDuration = 1f;
            float moveDelay = 2f;

            if (obj.TryGetComponent<IResourceHolder>(out var data))
            {
                spawnPos.y += 0.5f;

                foreach (var item in resources.ResourceIterator())
                {
                    float pushForce = 7f;

                    for (int i = 0; i < item.Value; i++)
                    {
                        var initialVelocity = Vector3.up * pushForce;

                        transManager.TransferResource(spawnPos, initialVelocity, item.Key,
                            moveDuration: moveDuration, moveDelay: moveDelay
                            );
                    }
                }

                return true;
            }

            return false;
        }
    }

    public class UnitDropOnDeath : MonoBehaviour
    {
        public ResourceContainer resources;
        private HealthData m_Health;
        private TransferMoveManager m_TransManager;

        private void Awake()
        {
            m_Health = GetComponent<HealthData>();

            if (m_Health)
            {
                m_Health.onDeath += UnitDrop_onDeath;
            }
        }

        [Inject]
        void Construct(TransferMoveManager moveManager, WorldSpaceMessageFactory factory)
        {
            m_TransManager = moveManager;
        }

        private void UnitDrop_onDeath()
        {
            var pos = transform.position;
            if (!DropHelper.TryDrop(m_Health.KilledBy, pos, resources, m_TransManager))
            {
                if (m_Health.KilledBy.TryGetComponent<IOwnable>(out var owner))
                {
                    DropHelper.TryDrop(owner.Owner, pos, resources, m_TransManager);
                }
            }
        }

      
    }

}