namespace Prototype
{
    public interface ISaveComponentData
    {
        public SerializableGuid Id { get; set; }
    }

    public interface ISceneSaveComponent<T> where T : ISaveComponentData
    {
        public SerializableGuid Id { get; set; }
        public T SaveComponent();
        public void LoadComponent(T data);
    }
}