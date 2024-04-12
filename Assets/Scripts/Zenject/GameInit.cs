using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public interface IInputReader
    {
        public Vector2 ReadMoveInput();
    }

    public class PlayerInputReader : IInputReader
    {
        public Joystick m_Stick;

        public PlayerInputReader(Joystick stick)
        {
            m_Stick = stick;
        }

        public Vector2 ReadMoveInput()
        {
            return m_Stick.Direction;
        }
    }

    public class EnemySpawnFactory
    {
        GameObject _zombieEnemy;
        private DiContainer m_Conteiner;

        public void SpawnZombie(Vector3 spawnPos)
        {
            m_Conteiner.InstantiatePrefab(_zombieEnemy, spawnPos, Quaternion.identity, null);
        }

        public EnemySpawnFactory(GameObject zombieEnemy, DiContainer container) {
            m_Conteiner = container;
            _zombieEnemy = zombieEnemy;
        }
    }

    public class GameInit : MonoInstaller
    {
        public GameObject PlayerPrefab;
        private PlayerSpawnFactory m_playerSpawnFactory;
        public Joystick Joystick;

        public GameObject zombiePrefab;

        public Transform playerSpawnPoint;
        public PlayerResourcesView m_PlayerResourcesView;
        public WorldSpaceMessage m_WSMPrefab;
        public ResourceTransferManager m_Transfer;
        public CameraController CameraController;
        public WorldToScreenUIManager WorldToScreenUIManager;
        public ActivateByDistanceToPlayerManager ActivateByDistanceToPlayerManager;
        public override void InstallBindings()
        {
            var spawnFactory = new EnemySpawnFactory(zombiePrefab, Container);
            var input = new PlayerInputReader(Joystick);
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);
            var wsm = new WorldSpaceMessageFactory(m_WSMPrefab);

            Container.Bind<ActivateByDistanceToPlayerManager>().FromInstance(ActivateByDistanceToPlayerManager);
            Container.Bind<WorldToScreenUIManager>().FromInstance(WorldToScreenUIManager);
            Container.Bind<CameraController>().FromInstance(CameraController);
            Container.Bind<PlayerResourcesView>().FromInstance(m_PlayerResourcesView);
            Container.Bind<WorldSpaceMessageFactory>().FromInstance(wsm);
            Container.Bind<ResourceTransferManager>().FromInstance(m_Transfer);
            Container.Bind<IPlayerFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<PlayerInputReader>().FromInstance(input);
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
