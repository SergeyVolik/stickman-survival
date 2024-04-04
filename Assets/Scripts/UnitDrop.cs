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
    void Construct(TransferMoveManager moveManager)
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
        const int maxItems = 10;

        if (obj.TryGetComponent<IResourceHolder>(out var data))
        {
            var spawnPos = transform.position;
            spawnPos.y += 0.5f;

            foreach (var item in resources.ResourceIterator())
            {
                data.Resources.AddResource(item.Key, item.Value);

                int transferVisual = Mathf.Clamp(item.Value, 0, maxItems);

                float pushForce = 7f;
                for (int i = 0; i < transferVisual; i++)
                {
                    var resourceObjectInstance = GameObjectPool.GetPoolObject(item.Key.Resource3dItem);

                    resourceObjectInstance.transform.position = spawnPos;

                    resourceObjectInstance.SetActive(true);

                    var rb = resourceObjectInstance.GetComponent<Rigidbody>();
                    var initialVelocity = Vector3.up * pushForce;

                    m_TransManager.Transfer3dObject(rb, spawnPos, initialVelocity, data.CenterPoint, moveDuration: 0.5f,
                        onComplete: () =>
                        {
                            rb.GetComponent<PoolObject>().Release();
                        });
                }
            }

            return true;
        }

        return false;
    }
}
