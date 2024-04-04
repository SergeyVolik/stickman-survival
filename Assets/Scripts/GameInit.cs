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

        public PlayerResources m_PlayerResources;
        public ResourceView m_PlayerResourcesView;

        public override void InstallBindings()
        {
            var spawnFactory = new EnemySpawnFactory(zombiePrefab, Container);
            var input = new PlayerInputReader(Joystick);
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);

            m_PlayerResourcesView.Bind(m_PlayerResources.resources);

            var go = new GameObject();

            var transfer = go.AddComponent<TransferMoveManager>();

            Container.Bind<TransferMoveManager>().FromInstance(transfer);
            Container.Bind<IPlayerFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<PlayerInputReader>().FromInstance(input);
            Container.Bind<PlayerResources>().FromInstance(m_PlayerResources);
            Container.Bind<EnemySpawnFactory>().FromInstance(spawnFactory);
        }

        private void Awake()
        {
            m_playerSpawnFactory.SpawnAtPosition(new Vector3());
        }
    }
}
