using UnityEngine;

namespace Prototype
{
    public class Hat : MonoBehaviour
    {
        public Vector3 spawnOffset;
        public Vector3 spawnRotation;
        private Rigidbody m_RB;
        private Collider m_Collider;
        public bool IsDropped => !m_RB.isKinematic;

        private void Awake()
        {
            m_RB = GetComponent<Rigidbody>();
            m_Collider = GetComponentInChildren<Collider>();

            m_Collider.enabled = false;
            m_RB.isKinematic = true;
            m_RB.Sleep();
        }

        private void Start()
        {
            transform.localPosition = spawnOffset;
            transform.localRotation = Quaternion.Euler(spawnRotation);
        }

        public void Setup()
        {
            transform.localPosition = spawnOffset;
            transform.localRotation = Quaternion.Euler(spawnRotation);
        }

        public void Drop()
        {
            m_Collider.enabled = true;
            m_RB.WakeUp();
            m_RB.isKinematic = false;
            transform.parent = null;
        }

        public void DropWithVelocity(Vector3 velocity)
        {
            Drop();
            m_RB.velocity = velocity;         
        }

        public void DropWithImpulse(Vector3 impulsVector)
        {
            Drop();
            m_RB.AddForce(impulsVector, ForceMode.Impulse);
        }
    }
}
