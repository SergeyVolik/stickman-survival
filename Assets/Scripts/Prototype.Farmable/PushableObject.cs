using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public interface IPushable
    {
        public Rigidbody Rigidbody { get; }

        public void Push(Vector3 force);
    }

    public class PushableObject : MonoBehaviour, IPushable
    {
        private Rigidbody m_Rb;

        private void Awake()
        {
            m_Rb = GetComponent<Rigidbody>();
        }

        public void Push(Vector3 force)
        {
            m_Rb.AddForce(force, ForceMode.Impulse);
        }

        public Rigidbody Rigidbody => m_Rb;
    }
}

