using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    [System.Serializable]
    public class WaveData
    {
        public SpawnData[] Spawns;
    }

    [System.Serializable]
    public class SpawnData
    {
        public float delay;
        public EnemyType enemyType;
        public Transform spawnPoint;
        public EnemyDropMode dropMode;
        public ResourceContainer dropOverride;
    }

    public enum EnemyDropMode
    {
        None,
        Add,
        Set        
    }
    
    public enum EnemyType
    {
        RunnerZombie,
        BigZombie,
        SlowZombie
    }

    public class LocationCombat : MonoBehaviour
    {
        public WaveData[] spawnWaves;
        private EnemySpawnFactory m_spawnFactory;
        private IPlayerFactory m_playerFactory;
        private int currentWaveIndex;
        private int currentSpawnIndex;

        private void Awake()
        {
            enabled = false;
        }

        [Inject]
        void Construct(EnemySpawnFactory spawnFactory, IPlayerFactory playerFactory)
        {
            m_spawnFactory = spawnFactory;
            m_playerFactory = playerFactory;
        }

        public bool Finished() => !enabled;
        private void Finish()
        {
            enabled = false;
        }

        public void StartCombat()
        {
            if (enabled)
                return;

            enabled = true;
        }

        float currentSpawnT = 0;
        private void Update()
        {
            var deltaTime = Time.deltaTime;

            if (currentWaveIndex >= spawnWaves.Length)
            {
                Finish();
                return;
            }

            currentSpawnT += deltaTime;

            var currWave = spawnWaves[currentWaveIndex];
            var currentSpawn = currWave.Spawns[currentSpawnIndex];

            if (currentSpawn.delay < currentSpawnT)
            {
                currentSpawnT = 0;
                var enemyInstance = ExecuteSpawn(currentSpawn);
                var seeker = enemyInstance.GetComponent<ITargetSeeker>();
                seeker.SetTarget(m_playerFactory.CurrentPlayerUnit.transform);
                currentSpawnIndex++;

                if (currWave.Spawns.Length <= currentSpawnIndex)
                {
                    currentWaveIndex++;
                    currentSpawnIndex = 0;
                }
            }
        }

        private GameObject ExecuteSpawn(SpawnData currentSpawn)
        {
            GameObject instance = null;
            var spawnPoint = currentSpawn.spawnPoint;
            switch (currentSpawn.enemyType)
            {
                case EnemyType.RunnerZombie:
                    instance = m_spawnFactory.SpawnDefaultZombie(spawnPoint.position);
                    break;
                case EnemyType.BigZombie:
                    instance = m_spawnFactory.SpawnBigZombie(spawnPoint.position);
                    break;
                case EnemyType.SlowZombie:
                    instance = m_spawnFactory.SpawnSlowZombie(spawnPoint.position);
                    break;
                default:
                    break;
            }
            instance.transform.forward = spawnPoint.forward;

            switch (currentSpawn.dropMode)
            {
                case EnemyDropMode.None:
                    break;
                case EnemyDropMode.Add:
                    var drop = instance.GetComponent<UnitDropOnDeath>();
                    foreach (var item in currentSpawn.dropOverride.ResourceIterator())
                    {
                        drop.resources.AddResource(item.Key, item.Value);
                    }         
                    
                    break;
                case EnemyDropMode.Set:
                     drop = instance.GetComponent<UnitDropOnDeath>();
                    drop.resources = currentSpawn.dropOverride;

                    break;
                default:
                    break;
            }

            return instance;
        }
    }
}