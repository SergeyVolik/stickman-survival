using MoreMountains.Feedbacks;
using Prototype.Ads;
using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public interface IPlayerInputReader
    {
        public Vector2 ReadMoveInput();
        public void Enable();
        public void Disable();

    }

    public class PlayerInputReader : IPlayerInputReader
    {
        private bool m_Enabled;
        public Joystick m_Stick;
        public void Enable()
        {
            m_Enabled = true;
        }
        public void Disable()
        {
            m_Enabled = false;
        }

        public PlayerInputReader(Joystick stick)
        {
            m_Enabled = true;
            m_Stick = stick;
        }

        public Vector2 ReadMoveInput()
        {
            return m_Enabled ? m_Stick.Direction : new Vector2();
        }
    }

    public class EnemySpawnFactory
    {
        GameObject _zombieEnemy;
        GameObject _bigEnemy;
        GameObject _slowZombieEnemy;

        private DiContainer m_Conteiner;

        public GameObject SpawnDefaultZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(_zombieEnemy, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }

        public GameObject SpawnBigZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(_bigEnemy, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }

        public GameObject SpawnSlowZombie(Vector3 spawnPos)
        {
            var zombieInstance = m_Conteiner.InstantiatePrefab(_slowZombieEnemy, spawnPos, Quaternion.identity, null);
            return zombieInstance;
        }

        public EnemySpawnFactory(GameObject zombieEnemy, GameObject bigZombie, GameObject slowZombieEnemy, DiContainer container) {
            m_Conteiner = container;
            _slowZombieEnemy = slowZombieEnemy;
            _zombieEnemy = zombieEnemy;
            _bigEnemy = bigZombie;
        }
    }

    public class GameInit : MonoInstaller
    {
        public GameObject PlayerPrefab;
        private PlayerSpawnFactory m_playerSpawnFactory;
        public Joystick Joystick;

        public GameObject zombiePrefab;
        public GameObject bigZombiePrefab;
        public GameObject slowZombiePrefab;

        public Transform playerSpawnPoint;
        public PlayerResourcesView m_PlayerResourcesView;
        public WorldSpaceMessage m_WSMPrefab;
        public ResourceTransferManager m_Transfer;
        public CameraController CameraController;
        public WorldToScreenUIManager WorldToScreenUIManager;
        public ActivateByDistanceToPlayerManager ActivateByDistanceToPlayerManager;
        public AdsManager AdsManager;
        public override void InstallBindings()
        {
            var spawnFactory = new EnemySpawnFactory(zombiePrefab, bigZombiePrefab, slowZombiePrefab, Container);
            var input = new PlayerInputReader(Joystick);
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);
            var wsm = new WorldSpaceMessageFactory(m_WSMPrefab);

            Container.Bind<IAdsPlayer>().FromInstance(AdsManager);
            Container.Bind<ActivateByDistanceToPlayerManager>().FromInstance(ActivateByDistanceToPlayerManager);
            Container.Bind<WorldToScreenUIManager>().FromInstance(WorldToScreenUIManager);
            Container.Bind<CameraController>().FromInstance(CameraController);
            Container.Bind<PlayerResourcesView>().FromInstance(m_PlayerResourcesView);
            Container.Bind<WorldSpaceMessageFactory>().FromInstance(wsm);
            Container.Bind<ResourceTransferManager>().FromInstance(m_Transfer);
            Container.Bind<IPlayerFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<IPlayerInputReader>().FromInstance(input);
            Container.Bind<EnemySpawnFactory>().FromInstance(spawnFactory);
        }

        public override void Start()
        {
            base.Start();
        }

        private void OnEnable()
        {
            m_playerSpawnFactory.SpawnAtPosition(playerSpawnPoint.position);
        }
    }
}
