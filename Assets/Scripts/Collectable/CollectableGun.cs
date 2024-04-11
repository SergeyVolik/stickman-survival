using UnityEngine;

namespace Prototype
{
    public class CollectableGun : MonoBehaviour, ICollectable
    {
        public GameObject GunPrefab;

        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<CharacterInventory>(out var behaviour))
            {
                behaviour.SetupWeapon(GunPrefab);
                gameObject.SetActive(false);
            }
        }
    }
}