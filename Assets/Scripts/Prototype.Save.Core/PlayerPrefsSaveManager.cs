using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    public abstract class PlayerPrefsSaveManager<T> : MonoBehaviour, ISaveManager<T> where T : class, new()
    {
        public void SerializedData(T data, string key)
        {
            var str = JsonConvert.SerializeObject(data);
            Debug.Log(str);
            PlayerPrefs.SetString(key, str);
        }

        public T DerializedData(string key)
        {
            var data = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));
            return data;
        }

        public void Save(string key)
        {
            var saveData = new T();

            SavePass(saveData);

            SerializedData(saveData, key);
        }

        public void Load(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                return;

            var loadData = DerializedData(key);

            LoadPass(loadData);
        }

        public abstract void SavePass(T saveData);
        public abstract void LoadPass(T LoadData);
    }
}