using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class SaveSceneHelper
    {
        public static void LoadGameScene(SceneSaveData saveObj)
        {
            var saveableObjects = GameObject.FindObjectsOfType<SaveableObject>(includeInactive: true);
          
            SaveHelper.LoadItem(saveObj.TransformSave, saveableObjects);
            SaveHelper.LoadItem(saveObj.GameObjectSave, saveableObjects);
        }

        public static SceneSaveData SaveGameScene(SceneSaveData save)
        {
            var saveableObjects = GameObject.FindObjectsOfType<SaveableObject>(includeInactive: true);

            SaveHelper.SaveItem(save.TransformSave, saveableObjects);
            SaveHelper.SaveItem(save.GameObjectSave, saveableObjects);

            return save;
        }
    }

    [System.Serializable]
    public class SceneSaveData
    {
        public Dictionary<SerializableGuid, TransformSave> TransformSave = new Dictionary<SerializableGuid, TransformSave>();
        public Dictionary<SerializableGuid, GameObjectSave> GameObjectSave = new Dictionary<SerializableGuid, GameObjectSave>();
    }

    public class LocationSaveManager : PlayerPrefsSaveManager<SceneSaveData>
    {
        public string SaveKey;

        public override void LoadPass(SceneSaveData LoadData)
        {
            Debug.Log("Load Location");

            SaveSceneHelper.LoadGameScene(LoadData);
        }

        public override void SavePass(SceneSaveData saveData)
        {
            Debug.Log("Save Location");

            SaveSceneHelper.SaveGameScene(saveData);
        }

        private void Start()
        {
            Load(SaveKey);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Save(SaveKey);
            }
        }

        private void OnApplicationQuit()
        {
            Save(SaveKey);
        }
    }
}
