using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prototype
{
    [DisallowMultipleComponent]
    public class SaveableObject : MonoBehaviour
    {
        public SerializableGuid guid;

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