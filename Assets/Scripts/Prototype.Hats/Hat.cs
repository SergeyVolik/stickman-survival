using System;
using UnityEngine;

namespace Prototype
{
    public class Hat : MonoBehaviour
    {
        public Vector3 spawnOffset;
        public Vector3 spawnRotation;
        private Rigidbody m_rb;

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
        }
        public void Setup()
        {
            transform.localPosition = spawnOffset;
            transform.localRotation = Quaternion.Euler(spawnRotation);
        }

        public void Drop()
        {
            m_rb.isKinematic = false;
            transform.parent = null;
        }
    }
}
