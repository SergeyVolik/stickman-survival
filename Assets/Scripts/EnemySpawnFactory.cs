using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemySpawnFactory : MonoBehaviour
    {
        [SerializeField] GameObject m_ZombieRunner;
        [SerializeField] GameObject m_BigEnemy;
        [SerializeField] GameObject m_SlowZombieEnemy;

        private DiContainer m_Conteiner;

        [Inject]
        void Construct(DiContainer container)
        {
            m_Conteiner = container;
        }

        public GameObject SpawnRunnerZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(m_ZombieRunner, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }

        public GameObject SpawnBigZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(m_BigEnemy, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }

        public GameObject SpawnSlowZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(m_SlowZombieEnemy, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }  
    }
}