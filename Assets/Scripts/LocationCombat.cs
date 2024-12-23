using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        public bool spawnHat;
        public HatType hatType;
        [System.NonSerialized]
        public GameObject instance;
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
        public GameObject[] externalEnemies;

        public PhysicsCallbacks startCombatCollider;
        public WaveData[] spawnWaves;
        private EnemySpawnFactory m_spawnFactory;
        private IPlayerFactory m_playerFactory;
        private int currentWaveIndex;
        private int currentSpawnIndex;
        public event Action onSpawnFinished = delegate { };
        public event Action onEnemyKilled = delegate { };
        int toKill = 0;

        public UnityEvent OnStartedUE;
        public UnityEvent OnSpawnFinishedUE;
        public UnityEvent OnAllKilledUE;

        public bool prepareEnemies;

        public int AlreadyKilled => alreadyKilled;
        public int TargetKills => toKill;

        List<Transform> aliveUnits = new List<Transform>();
        float currentSpawnT = 0;
        private int alreadyKilled;
        private void Awake()
        {
            enabled = false;

            foreach (var item in spawnWaves)
            {
                foreach (var item1 in item.Spawns)
                {
                    toKill++;

                    if (prepareEnemies)
                    {
                        var instance = ExecuteSpawn(item1);
                        item1.instance = instance;
                        instance.gameObject.SetActive(false);
                    }
                }
            }

            toKill += externalEnemies.Length;

            if (startCombatCollider)
            {
                startCombatCollider.onTriggerEnter += (col) =>
                {
                    startCombatCollider.gameObject.SetActive(false);
                    StartCombat();
                };
            }
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
            onSpawnFinished.Invoke();
            OnSpawnFinishedUE.Invoke();
        }

        public void StartCombat()
        {
            if (enabled)
                return;

            SetupExternalEnemies();

            OnStartedUE.Invoke();
            m_playerFactory.CurrentPlayerUnit.GetComponent<CharacterCombatState>().forceCombat = true;
            enabled = true;
        }

        private void SetupExternalEnemies()
        {
            foreach (var item in externalEnemies)
            {
                var seeker = item.GetComponent<ITargetSeeker>();
                seeker.SetTarget(m_playerFactory.CurrentPlayerUnit.transform);
                item.GetComponent<HealthData>().onDeath += () => {
                    OnEnemyDeath(item.transform);
                };
                aliveUnits.Add(item.transform);
            }
        }

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

            if (currWave.Spawns.Length == 0)
            {
                currentWaveIndex++;
                currentSpawnIndex = 0;
                return;
            }

            var currentSpawn = currWave.Spawns[currentSpawnIndex];

            if (currentSpawn.delay < currentSpawnT)
            {
                currentSpawnT = 0;
                var enemyInstance = currentSpawn.instance ? currentSpawn.instance : ExecuteSpawn(currentSpawn);
                enemyInstance.SetActive(true);
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

        public IEnumerable<Transform> GetAllAliveUnits()
        {
            return aliveUnits;
        }

        public Transform GetAliveUnit()
        {
            return aliveUnits.Count == 0 ? null : aliveUnits[0];
        }

        void OnEnemyDeath(Transform enemyInstance)
        {
            alreadyKilled++;
            onEnemyKilled.Invoke();
            aliveUnits.Remove(enemyInstance);

            if (alreadyKilled == TargetKills)
            {
                m_playerFactory.CurrentPlayerUnit.GetComponent<CharacterCombatState>().forceCombat = false;
                OnAllKilledUE.Invoke();
            }
        }
        private GameObject ExecuteSpawn(SpawnData currentSpawn)
        {
            GameObject instance = null;
            var spawnPoint = currentSpawn.spawnPoint;
            switch (currentSpawn.enemyType)
            {
                case EnemyType.RunnerZombie:
                    instance = m_spawnFactory.SpawnRunnerZombie(spawnPoint.position);
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

            aliveUnits.Add(instance.transform);
            instance.GetComponent<HealthData>().onDeath += () => {
                OnEnemyDeath(instance.transform);              
            };
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

            if (currentSpawn.spawnHat && instance.TryGetComponent<HatSetup>(out var hatSetup))
            {
                hatSetup.SpawnHat(currentSpawn.hatType);
            }

            return instance;
        }
    }
}