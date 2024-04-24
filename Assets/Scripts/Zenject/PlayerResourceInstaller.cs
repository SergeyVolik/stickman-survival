using Prototype;
using Zenject;

public class PlayerResourceInstaller : MonoInstaller
{
    public PlayerResources m_PlayerResources;
    public override void InstallBindings()
    {
        Container.Bind<PlayerResources>().FromInstance(m_PlayerResources);
    }
}