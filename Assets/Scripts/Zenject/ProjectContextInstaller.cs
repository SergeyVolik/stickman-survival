using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public PlayerResources m_PlayerResources;
        public ResourcesTypesSO m_ResourceTypesSO;
        public override void InstallBindings()
        {
            Container.Bind<PlayerResources>().FromInstance(m_PlayerResources);
            Container.Bind<IResourceTypes>().FromInstance(m_ResourceTypesSO);
        }
    }
}
