using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class TransformSave
    {
        public Vector3S position;
        public QuaternionS rotation;
        public Vector3S localScale;
    }

    [DisallowMultipleComponent]
    public class SaveTransform : MonoBehaviour, ISaveable<TransformSave>, ISceneSaveComponent
    {
        public bool savePosition = true;
        public bool saveRotation = true;
        public bool saveScale = true;

        public void Load(TransformSave data)
        {
            if (data == null)
                return;

            var tran = transform;

            if (savePosition)
            {
                tran.position = data.position;
            }

            if (saveRotation)
            {
                tran.rotation = data.rotation;
            }

            if (saveScale)
            {
                tran.localScale = data.localScale;
            }
        }

        public void LoadObject(object data)
        {
            if (data is TransformSave transSave)
            {
                Load(transSave);
            }
        }

        public object SaveObject()
        {
            return Save();
        }

        public TransformSave Save()
        {
            if (savePosition == false)
                return null;

            var tran = transform;

            return new TransformSave
            {
                position = tran.position,
                rotation = tran.rotation,
                localScale = tran.localScale,
            };
        }
    }
}
