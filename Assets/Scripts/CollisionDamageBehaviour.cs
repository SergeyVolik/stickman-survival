using Prototype;
using UnityEngine;

public class CollisionDamageBehaviour : MonoBehaviour
{
    public float damageInterval = 0.3f;
    private float prevAttackTime;
    public float pushForce = 200;
    public int damage = 1;
    public bool onlyPlayerAttack;
    public GameObject owner;
    private Collider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
    }

    private void OnEnable()
    { }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        Collider collider = collision.collider;

        var time = Time.time;
        DoAttack(collider, time);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!enabled)
            return;

        Collider collider = collision.collider;

        var time = Time.time;
        DoAttack(collider, time);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        var time = Time.time;
        DoAttack(other, time);
    }

    private void DoAttack(Collider collider, float time)
    {
        if (prevAttackTime < time - damageInterval)
        {
            bool isDamageable = collider.TryGetComponent<IDamageable>(out var damageable);

            if (isDamageable == false)
                return;

            if (onlyPlayerAttack && !collider.GetComponent<PlayerInput>())
            {
                return;
            }

            prevAttackTime = time;

            damageable.DoDamage(damage, gameObject);

            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(owner.transform.forward * pushForce, mode: ForceMode.Force);
            }
        }
    }
}
