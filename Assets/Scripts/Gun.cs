using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage;

    [SerializeField]
    private MMF_Player feedback;
    [SerializeField]
    private Transform projectileSpawnPoint;

    public GameObject owner;
    public void Shot()
    {
        feedback?.PlayFeedbacks();

        if (Physics.Raycast(projectileSpawnPoint.position, owner.transform.forward, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.DoDamage(damage, owner);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray {  origin = projectileSpawnPoint.position, direction = projectileSpawnPoint.forward });
    }
}
