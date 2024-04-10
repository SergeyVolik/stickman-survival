using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Prototype
{
    public class CharacterCombatState : MonoBehaviour
    {
        public event Action<bool> onCombatState = delegate { };

        [SerializeField]
        private bool m_InCombat;
        public LayerMask enemyLayerMask;
        public LayerMask wallLayers;

        public float checkEnemyRadius = 5f;
        private Transform m_Transfrom;
        RaycastHit[] m_RaycastHits;
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
            m_RaycastHits = new RaycastHit[10];
        }

        private void Start()
        {
            m_NoEnemiesT = timeToEndCombat;
            onCombatState.Invoke(m_InCombat);
        }

        public float timeToEndCombat = 2f;

        [SerializeField]
        float m_NoEnemiesT;

        private void FixedUpdate()
        {
            bool seeAnyEnemy = PhysicsHelper.GetAllTargetWithoutWalls(m_Transfrom, m_RaycastHits, checkEnemyRadius, enemyLayerMask, wallLayers, 0.5f) != 0;

            if (seeAnyEnemy)
            {               
                m_NoEnemiesT = 0;
            }
            else
            {
                m_NoEnemiesT += Time.fixedDeltaTime;
            }
      
            bool noEnemiesFinished = m_NoEnemiesT < timeToEndCombat;

            InCombat = noEnemiesFinished;
        }
    }
}