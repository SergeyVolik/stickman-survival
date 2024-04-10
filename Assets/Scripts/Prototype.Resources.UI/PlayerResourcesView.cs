using Zenject;

namespace Prototype
{
    public class PlayerResourcesView : ResourceView
    {
        [Inject]
        void Construct(PlayerResources pResources)
        {
            Bind(pResources.resources);
        }
    }
}
