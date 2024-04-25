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

    public class GameInit : MonoInstaller
    {
        public GameObject PlayerPrefab;
        private PlayerSpawnFactory m_playerSpawnFactory;
        public Joystick Joystick;

        public GameObject zombiePrefab;
        public GameObject bigZombiePrefab;
        public GameObject slowZombiePrefab;

        public PlayerResourcesView m_PlayerResourcesView;
        public WorldSpaceMessage m_WSMPrefab;
        public ResourceTransferManager m_Transfer;
        public CameraController CameraController;
        public WorldToScreenUIManager WorldToScreenUIManager;
        public ActivateByDistanceToPlayerManager ActivateByDistanceToPlayerManager;
        public AdsManager AdsManager;
        public HatFactory HatFactory;
        public EnemySpawnFactory EnemySpawnFactory;

        public override void InstallBindings()
        {
            var input = new PlayerInputReader(Joystick);
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);
            var wsm = new WorldSpaceMessageFactory(m_WSMPrefab);

            Container.Bind<HatFactory>().FromInstance(HatFactory);
            Container.Bind<IAdsPlayer>().FromInstance(AdsManager);
            Container.Bind<ActivateByDistanceToPlayerManager>().FromInstance(ActivateByDistanceToPlayerManager);
            Container.Bind<WorldToScreenUIManager>().FromInstance(WorldToScreenUIManager);
            Container.Bind<CameraController>().FromInstance(CameraController);
            Container.Bind<PlayerResourcesView>().FromInstance(m_PlayerResourcesView);
            Container.Bind<WorldSpaceMessageFactory>().FromInstance(wsm);
            Container.Bind<ResourceTransferManager>().FromInstance(m_Transfer);
            Container.Bind<IPlayerFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<IPlayerInputReader>().FromInstance(input);
            Container.Bind<EnemySpawnFactory>().FromInstance(EnemySpawnFactory);
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
