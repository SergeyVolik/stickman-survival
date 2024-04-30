using MoreMountains.Feedbacks;
using System;
using UnityEngine;

namespace Prototype
{
    public class Hat : MonoBehaviour
    {
        public Vector3 spawnOffset;
        public Vector3 spawnRotation;
        private Rigidbody m_rb;

        private Collider m_collider;

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
            m_collider = GetComponentInChildren<Collider>();

            m_collider.enabled = false;
            m_rb.isKinematic = true;
            m_rb.Sleep();
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
            m_collider.enabled = true;
            m_rb.WakeUp();
            m_rb.isKinematic = false;
            transform.parent = null;
        }
    }
}
