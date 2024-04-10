using System;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class GameObjectSave
    {
        public bool activeSelf;
    }

    [System.Serializable]
    public class TransformSave
    {
        public Vector3S position;
        public QuaternionS rotation;
    }

    [DisallowMultipleComponent]
    public class SaveableObject : MonoBehaviour, ISaveable<GameObjectSave>, ISaveable<TransformSave>
    {
        public SerializableGuid guid;

        public bool savePosition = true;
        public bool saveActiveState = true;

        public void Load(GameObjectSave data)
        {
            if (data == null)
                return;

            Debug.Log($"Load activeSelf {data.activeSelf}");
            gameObject.SetActive(data.activeSelf);
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

        public void Load(TransformSave data)
        {
            if (data == null)
                return;

            if (savePosition)
            {
                transform.position = data.position;
                transform.rotation = data.rotation;
            }
        }

        TransformSave ISaveable<TransformSave>.Save()
        {
            if (savePosition == false)
                return null;

            return new TransformSave
            {
                position = transform.position,
                rotation = transform.rotation
            };
        }

        private void OnValidate()
        {
            HasConflicts(guid, gameObject);
        }

        public static bool HasConflicts(SerializableGuid guid, GameObject go)
        {
            var allItems = FindObjectsOfType<SaveableObject>(includeInactive: true);

            int count = 0;
            foreach (var item in allItems)
            {
                if (item.guid == guid)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                Debug.LogError($"guid conflict {guid}. Please generete new guid", go);
                return true;
            }

            return false;
        }

        public void GenGuid()
        {
            guid = System.Guid.NewGuid();
        }
    }
}