using MoreMountains.Feedbacks;
using Prototype;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage;
    public float pushForce;
    public float killPushForce;
    [SerializeField]
    private MMF_Player feedback;
    [SerializeField]
    private Transform projectileSpawnPoint;

    public GameObject owner;
    public void Shot()
    {
        feedback?.PlayFeedbacks();

        var shotVector = owner.transform.forward;

        if (Physics.Raycast(projectileSpawnPoint.position, shotVector, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.DoDamage(damage, gameObject);
            }

            if (hit.collider.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(shotVector * pushForce, mode: ForceMode.Force);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray {  origin = projectileSpawnPoint.position, direction = projectileSpawnPoint.forward });
    }
}
