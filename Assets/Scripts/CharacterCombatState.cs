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
        public LayerMask wallLayers;

        public float checkEnemyRadius = 5f;
        private Transform m_Transfrom;
        RaycastHit[] m_RaycastHits;
        public float seeEnemyTimeToStart;

        [SerializeField]
        private float m_SeeEnemyTime;
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
            var selfPos = m_Transfrom.position;
            selfPos.y += 0.5f;

            var count = Physics.SphereCastNonAlloc(selfPos, checkEnemyRadius, Vector3.up, m_RaycastHits, float.MaxValue, enemyLayerMask);
            bool seeAnyEnemy = false;

            for (int i = 0; i < count; i++)
            {
                var targetPos = m_RaycastHits[i].transform.position;
                targetPos.y += 0.5f;
                var vector = targetPos - selfPos;

                if (!Physics.Raycast(selfPos, vector.normalized, checkEnemyRadius, wallLayers))
                {
                    Debug.DrawLine(selfPos, targetPos, Color.green);
                    seeAnyEnemy = true;
                    break;
                }
            }

            bool hasEnemiesFInished = seeEnemyTimeToStart < m_SeeEnemyTime;
         
            if (seeAnyEnemy)
            {
                if (hasEnemiesFInished)
                {
                    m_NoEnemiesT = 0;
                }

                m_SeeEnemyTime += Time.fixedDeltaTime;
            }
            else
            {
                m_SeeEnemyTime = 0;
                m_NoEnemiesT += Time.fixedDeltaTime;
            }

            bool noEnemiesFinished = m_NoEnemiesT < timeToEndCombat;
            InCombat = noEnemiesFinished || hasEnemiesFInished;
        }
    }
}