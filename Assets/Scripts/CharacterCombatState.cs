using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Prototype
{
    public static class PhysicsHelper
    {
        public static int GetAllTargetWithoutWalls(Transform castFrom, RaycastHit[] sphereCastHits, float castRadius, LayerMask sphereCastLayer, LayerMask wallLayer, float yOffset = 0)
        {
            var selfPos = castFrom.position;
            selfPos.y += yOffset;

            NativeList<RaycastHit> noWallHits = new NativeList<RaycastHit>(Allocator.Temp);

            var count = Physics.SphereCastNonAlloc(selfPos, castRadius, Vector3.up, sphereCastHits, float.MaxValue, sphereCastLayer);

            for (int i = 0; i < count; i++)
            {
                var targetPos = sphereCastHits[i].transform.position;
                targetPos.y += yOffset;
                var vector = targetPos - selfPos;

                if (!Physics.Raycast(selfPos, vector.normalized, castRadius, wallLayer))
                {
                    noWallHits.Add(sphereCastHits[i]);
                }
            }

            for (int i = 0; i < noWallHits.Length; i++)
            {
                sphereCastHits[i] = noWallHits[i];
            }

            return noWallHits.Length;
        }
    }

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