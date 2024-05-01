using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Prototype
{
    public class ItemPreviewRotator : MonoBehaviour, IDragHandler
    {
        [Inject]
        void Construct(ItemPreviewCamera previewCamera)
        {
            m_PreviewCamera = previewCamera;
        }

        private ItemPreviewCamera m_PreviewCamera;
        public float rotationSpeed;

        public void OnDrag(PointerEventData eventData)
        {
            var delta = eventData.delta;
            m_PreviewCamera.RotateCamera(rotationSpeed * delta.x);
        }
    }
}
