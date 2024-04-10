#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Prototype
{
    public static class SaveLoadEdiorHelper
    {

        [MenuItem("Prototype/GeneteteSaveableGuids")]
        public static void GeneteteSaveableGuids()
        {
            var objs = GameObject.FindObjectsByType<SaveableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var obj in objs)
            {
                EditorUtility.SetDirty(obj);
                obj.GenGuid();
            }

            Debug.Log("New guids Generated");
        }

        [MenuItem("Prototype/CheckGuidConflicts")]
        public static void CheckGuidConflicts()
        {
            var objs = GameObject.FindObjectsByType<SaveableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            bool hasConflicts = false;
            foreach (var obj in objs)
            {
                if (SaveableObject.HasConflicts(obj.guid, obj.gameObject))
                {
                    hasConflicts = true;
                }
            }

            if (!hasConflicts)
            {
                Debug.Log("No Guid conflicts finded");
            }
        }
    }
}

#endif