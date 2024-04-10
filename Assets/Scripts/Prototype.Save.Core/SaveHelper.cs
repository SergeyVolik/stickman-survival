using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Prototype
{
    public static class SaveHelper
    {
        public static void LoadItem<T>(Dictionary<SerializableGuid, T> saveObj, IEnumerable<SaveableObject> saveableObjects)
        {
            foreach (var item in saveableObjects)
            {
                var guid = item.guid;

                if (item.TryGetComponent<ISaveable<T>>(out var recSave))
                {
                    if (saveObj.TryGetValue(guid, out var data))
                        recSave.Load(data);
                }
            }
        }

        public static void SaveItem<T>(Dictionary<SerializableGuid, T> saveObj, IEnumerable<SaveableObject> saveableObjects)
        {
            foreach (var item in saveableObjects)
            {
                var guid = item.guid;

                if (item.TryGetComponent<ISaveable<T>>(out var comp))
                {
                    Assert.IsTrue(saveObj.TryGetValue(guid, out var obj) == false, $"guid conflict name:{item.gameObject.name} id:{item.gameObject.GetInstanceID()}");
                    saveObj.Add(guid, comp.Save());
                }
            }
        }
    }
}