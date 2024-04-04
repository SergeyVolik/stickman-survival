namespace Prototype
{
    [System.Serializable]
    public class PlayerResources
    {
        public PlayerResources()
        { }

        public PlayerResources(ResourceContainer _resources)
        {
            this.resources = _resources;
        }

        public ResourceContainer resources;
    }

    [System.Serializable]
    public class GameResources
    {
        public ResourceTypeSO[] Value;
    }
}
