namespace Prototype
{
    public interface ISaveable<T>
    {
        public T Save();
        public void Load(T data);
    }

    public interface ISceneSaveComponent
    {
        public object SaveObject();
        public void LoadObject(object data);
    }
}