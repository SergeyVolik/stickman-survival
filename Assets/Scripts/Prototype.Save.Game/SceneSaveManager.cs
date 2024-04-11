using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class SceneSaveData
    {
        public List<TransformSave> TransSave = new List<TransformSave>();
        public List<GameObjectSave> GoSave = new List<GameObjectSave>();
    }

    public class SceneSaveManager : PlayerPrefsSaveManager<SceneSaveData>
    {
        public string SaveKey;

        public override void LoadPass(SceneSaveData sceneSaveData)
        {
            Debug.Log($"Scene Load {SaveKey}");

            SaveHelper.LoadComponents<TransformSave, SaveTransform>(sceneSaveData.TransSave);
            SaveHelper.LoadComponents<GameObjectSave, SaveGameObjectState>(sceneSaveData.GoSave);
        }

        public override void SavePass(SceneSaveData sceneSaveData)
        {
            Debug.Log($"Scene Save {SaveKey}");

            sceneSaveData.TransSave = SaveHelper.SaveComponents<TransformSave, SaveTransform>();
            sceneSaveData.GoSave = SaveHelper.SaveComponents<GameObjectSave, SaveGameObjectState>();
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
