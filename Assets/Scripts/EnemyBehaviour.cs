using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemyBehaviour : MonoBehaviour
    {
        private PlayerSpawnFactory m_factory;
        private Rigidbody m_RB;
        private Transform m_Transform;

        public float maxSpeed;
        public float damageInterval = 0.3f;
        private float t;
        [Inject]
        void Construct(PlayerSpawnFactory factory)
        {
            m_factory = factory;
        }

        private void Awake()
        {
            m_RB = GetComponent<Rigidbody>();
            m_Transform = transform;
        }

        private void Update()
        {
            t += Time.deltaTime;

            if (m_factory.CurrentPlayerUnit == null)
                return;

            var playerPos = m_factory.CurrentPlayerUnit.transform.position;
            playerPos.y = 0;
            var selfPos = m_RB.position;
            selfPos.y = 0;

            var vector = (playerPos - selfPos).normalized;

            m_RB.velocity = vector * maxSpeed;
            m_Transform.LookAt(playerPos);
        }

        
        private void OnCollisionStay(Collision collision)
        {
            if (t > damageInterval)
            {
              
                if (collision.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    t = 0;
                    damageable.DoDamage(1, gameObject);
                }
            }
        }
    }
}