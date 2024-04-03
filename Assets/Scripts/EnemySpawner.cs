using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject EnemyPrefab;

        public Transform[] SpawnPoints;

        public float spawnInterval;
        public float spawnT;
        private DiContainer m_Conteiner;

        [Inject]
        void Construct(DiContainer container)
        {
            m_Conteiner = container;
        }

        private void Update()
        {
            spawnT += Time.deltaTime;

            if (spawnT > spawnInterval)
            {
                spawnT = 0;
                var spawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
                m_Conteiner.InstantiatePrefab(EnemyPrefab, spawnPos, Quaternion.identity, null);
            }
        }
    }
}