using System;
using UnityEngine;

namespace Prototype
{
    public class CollectableWeapon : MonoBehaviour, ICollectable
    {
        public GameObject GunPrefab;

        public event Action onCollected;

        public void Collect(GameObject collecteBy)
        {
            if (collecteBy.TryGetComponent<CharacterInventory>(out var behaviour))
            {
                if (GunPrefab.GetComponent<Gun>())
                {
                    behaviour.SetupGun(GunPrefab);
                }
                else if (GunPrefab.GetComponent<MeleeWeapon>())
                {
                    behaviour.SetupMeleeWeapon(GunPrefab.GetComponent<MeleeWeapon>());
                }
                onCollected?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}