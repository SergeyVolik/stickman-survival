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
            onCombatState.Invoke(m_InCombat);
        }

        private void FixedUpdate()
        {          
            var count = Physics.SphereCastNonAlloc(m_Transfrom.position, checkEnemyRadius, Vector3.up, m_RaycastHit, float.MaxValue, enemyLayerMask);
            InCombat = count > 0;
        }
    }
}