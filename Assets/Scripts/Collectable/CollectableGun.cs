using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGun : MonoBehaviour, ICollectable
{
    public GameObject GunPrefab;

    public void Collect(GameObject collecteBy)
    {
        if (collecteBy.TryGetComponent<CharacterGunBehaviourV2>(out var behaviour))
        {
            behaviour.SpawnGun(GunPrefab);
            GameObject.Destroy(gameObject);
        }
    }
}
