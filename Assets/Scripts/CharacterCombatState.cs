using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterCombatState : MonoBehaviour
    {
        public event Action<bool> onCombatState = delegate { };

        [SerializeField]
        private bool m_InCombat;
        public LayerMask enemyLayerMask;
        public float checkEnemyRadius = 5f;
        private Transform m_Transfrom;
        RaycastHit[] m_RaycastHit;

        public bool InCombat
        {
            get
            {
                return m_InCombat;
            }
            private set
            {
                bool prev = m_InCombat;

                m_InCombat = value;

                if (prev != value)
                    onCombatState.Invoke(m_InCombat);
            }
        }

        private void Awake()
        {
            m_Transfrom = transform;
            m_RaycastHit = new RaycastHit[3];
        }

        private void Start()
        {
            m_NoEnemiesT = timeToEndCombat;
            onCombatState.Invoke(m_InCombat);          
        }

        public float timeToEndCombat = 2f;
        float m_NoEnemiesT;

        private void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(m_Transfrom.position, checkEnemyRadius, Vector3.up, m_RaycastHit, float.MaxValue, enemyLayerMask);
            bool hasEnemies = count > 0;

            if (hasEnemies)
            {
                m_NoEnemiesT = 0;
            }

            m_NoEnemiesT += Time.deltaTime;

            InCombat = hasEnemies || m_NoEnemiesT < timeToEndCombat;
        }
    }
}