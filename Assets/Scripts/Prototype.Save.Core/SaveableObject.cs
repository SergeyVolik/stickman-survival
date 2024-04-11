using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Prototype
{
    [DisallowMultipleComponent]
    public abstract class SaveableObject : MonoBehaviour, ISaveGuid
    {
        [field: SerializeField]
        public SerializableGuid Id { get; set; } = SerializeableGuidHelper.NewGuid();

        private void OnValidate()
        {
            HasConflicts(Id, gameObject);
        }

        public static bool HasConflicts(SerializableGuid guid, GameObject go)
        {
            var allItems = FindObjectsOfType<SaveableObject>(includeInactive: true);

            int count = 0;
            foreach (var item in allItems)
            {
                if (item.Id == guid)
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
            Id = System.Guid.NewGuid();
        }
    }
}