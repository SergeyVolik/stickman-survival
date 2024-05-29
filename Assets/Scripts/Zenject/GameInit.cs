using MoreMountains.Feedbacks;
using Prototype.Ads;
using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class GameInit : MonoInstaller
    {
        public GameObject PlayerPrefab;
        private PlayerSpawnFactory m_playerSpawnFactory;

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
        public AudioListenerController AudioListenerController;
        public ItemPreviewUIPage ItemPreviewUIPage;
        public ItemPreviewCamera ItemPreviewCamera;
        public override void InstallBindings()
        {
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);
            var wsm = new WorldSpaceMessageFactory(m_WSMPrefab);
            Container.Bind<AudioListenerController>().FromInstance(AudioListenerController);
            Container.Bind<ItemPreviewUIPage>().FromInstance(ItemPreviewUIPage);
            Container.Bind<ItemPreviewCamera>().FromInstance(ItemPreviewCamera);
            Container.Bind<HatFactory>().FromInstance(HatFactory);
            Container.Bind<IAdsPlayer>().FromInstance(AdsManager);
            Container.Bind<ActivateByDistanceToPlayerManager>().FromInstance(ActivateByDistanceToPlayerManager);
            Container.Bind<WorldToScreenUIManager>().FromInstance(WorldToScreenUIManager);
            Container.Bind<CameraController>().FromInstance(CameraController);
            Container.Bind<PlayerResourcesView>().FromInstance(m_PlayerResourcesView);
            Container.Bind<WorldSpaceMessageFactory>().FromInstance(wsm);
            Container.Bind<ResourceTransferManager>().FromInstance(m_Transfer);
            Container.Bind<IPlayerFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<EnemySpawnFactory>().FromInstance(EnemySpawnFactory);
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
