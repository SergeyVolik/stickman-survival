using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class GameObjectSave
    {
        public bool activeSelf;
    }

    public class SaveGameObjectState : MonoBehaviour, ISaveable<GameObjectSave>, ISceneSaveComponent
    {
        public bool saveActiveState = true;

        public void Load(GameObjectSave data)
        {
            if (data == null)
                return;

            gameObject.SetActive(data.activeSelf);
        }

        public void LoadObject(object data)
        {
            if (data is GameObjectSave saveData)
            {
                Load(saveData);
            }
        }

        public GameObjectSave Save()
        {
            if (saveActiveState == false)
                return null;

            return new GameObjectSave
            {
                activeSelf = gameObject.activeSelf,
            };
        }

        public object SaveObject()
        {
            return Save();
        }
    }
}
