using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform[] SpawnPoints;

        public float spawnInterval;
        public float spawnT;

        private EnemySpawnFactory m_spawnFactory;

        [Inject]
        void Construct(EnemySpawnFactory spawnFactory)
        {
            m_spawnFactory = spawnFactory;
        }

        private void Update()
        {
            spawnT += Time.deltaTime;

            if (spawnT > spawnInterval)
            {
                spawnT = 0;
                var spawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
                m_spawnFactory.SpawnZombie(spawnPos);
                
            }
        }
    }
}