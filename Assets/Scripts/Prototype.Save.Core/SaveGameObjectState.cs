using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class GameObjectSave : ISaveComponentData
    {
        public bool activeSelf;
        public SerializableGuid Id { get; set; }
    }

    public class SaveGameObjectState : MonoBehaviour, ISceneSaveComponent<GameObjectSave>
    {
        public bool saveActiveState = true;

        [field:SerializeField]
        public SerializableGuid Id { get; set; } = SerializeableGuidHelper.NewGuid();

        public void LoadComponent(GameObjectSave data)
        {
            if (data == null)
                return;

            gameObject.SetActive(data.activeSelf);
        }

        public GameObjectSave SaveComponent()
        {
            if (saveActiveState == false)
                return null;

            return new GameObjectSave
            {
                activeSelf = gameObject.activeSelf,
            };
        }
    }
}
