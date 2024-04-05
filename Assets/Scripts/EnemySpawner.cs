using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform[] SpawnPoints;

        public float spawnInterval;
        public float spawnT;
        public int perSpawn = 10;
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

                for (int i = 0; i < perSpawn; i++)
                {
                    var spawnPos = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
                    m_spawnFactory.SpawnZombie(spawnPos);
                }
                enabled = false;

            }
        }
    }
}