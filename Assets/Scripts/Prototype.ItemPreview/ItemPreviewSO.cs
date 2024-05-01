using UnityEngine;

namespace Prototype
{
    [CreateAssetMenu]
    public class ItemPreviewSO : ScriptableObject
    {
        public string title;
        public Vector3 previewOffset;
        public Vector3 previewRotation;
        public GameObject previewPrefab;
    }
}
