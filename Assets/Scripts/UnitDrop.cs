using DG.Tweening;
using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UnitDrop : MonoBehaviour
{
    public ResourceContainer resources;
    private HealthData m_Health;
    private TransferMoveManager m_TransManager;

    private void Awake()
    {
        m_Health = GetComponent<HealthData>();
        m_Health.onDeath += UnitDrop_onDeath;
    }

    [Inject]
    void Construct(TransferMoveManager moveManager, WorldSpaceMessageFactory factory)
    {
        m_TransManager = moveManager;
    }

    private void UnitDrop_onDeath()
    {
        if (!TryAddResource(m_Health.KilledBy))
        {
            if (m_Health.KilledBy.TryGetComponent<IOwnable>(out var owner))
            {
                TryAddResource(owner.Owner);
            }
        }
    }

    private bool TryAddResource(GameObject obj)
    {
        float moveDuration = 1f;
        float moveDelay = 2f;

        if (obj.TryGetComponent<IResourceHolder>(out var data))
        {
            var spawnPos = transform.position;
            spawnPos.y += 0.5f;

            foreach (var item in resources.ResourceIterator())
            {
                float pushForce = 7f;

                for (int i = 0; i < item.Value; i++)
                {
                    var initialVelocity = Vector3.up * pushForce;

                    m_TransManager.TransferResource(spawnPos, initialVelocity, item.Key,
                        moveDuration: moveDuration, moveDelay: moveDelay
                        );
                }
            }

            return true;
        }

        return false;
    }
}
