using UnityEngine;

namespace Prototype
{
    public class CollectableHealtBox : MonoBehaviour, ICollectable
    {
        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<HealthData>(out var data))
            {
                data.DoHeal(data.maxHealth, gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}

