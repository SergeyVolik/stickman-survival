using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ResourceTransferManager : MonoBehaviour
    {
        private Camera m_Camera;
        private PlayerResourcesView m_resourceUi;
        private PlayerResources m_playerResources;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        [Inject]
        void Construct(PlayerResourcesView resourceUi, PlayerResources playerResources)
        {
            m_resourceUi = resourceUi;
            m_playerResources = playerResources;
        }


        public void TransferResource(
           Vector3 startPosition,
           Vector3 initialVelocity,
           ResourceTypeSO resourceType,
           float maxVectorAngleOffset = 10,
           float moveDelay = 1f,
           float moveDuration = 1f,
            int resourceNumber = 1
           )
        {
            var resourceObjectInstance = GameObject.Instantiate(resourceType.Resource3dItem, startPosition, Quaternion.identity);
            resourceObjectInstance.SetActive(true);
            var rb = resourceObjectInstance.GetComponent<Rigidbody>();

            //rotate RigidBody vector by angles
            var angle1 = quaternion.AxisAngle(math.forward(), UnityEngine.Random.Range(math.radians(-maxVectorAngleOffset), math.radians(maxVectorAngleOffset)));
            var angle2 = quaternion.AxisAngle(math.right(), UnityEngine.Random.Range(math.radians(-maxVectorAngleOffset), math.radians(maxVectorAngleOffset)));
            var velocityWithOffset = math.mul(angle1, math.mul(angle2, initialVelocity));

            rb.velocity = velocityWithOffset;

            var uiTarget = m_resourceUi.GetResourceItemsByType(resourceType);

            DOVirtual.DelayedCall(moveDelay, () =>
            {
                var uiTarget = m_resourceUi.GetResourceItemsByType(resourceType);
                var uiItem = GameObject.Instantiate(resourceType.Resource2dItem, m_resourceUi.transform.parent);

                SetScreenPositionFromWorldPosition(uiItem.GetComponent<RectTransform>(), rb.transform.position, m_Camera);
                GameObject.Destroy(rb.gameObject);

                var targetPos = uiTarget.spriteImage.transform.position;

                var seq = DOTween.Sequence();
                var duration = moveDuration * UnityEngine.Random.Range(0.9f, 1f);
                seq.Insert(0, uiItem.transform.DOMoveX(targetPos.x, duration).SetEase(Ease.InCubic));
                seq.Insert(0, uiItem.transform.DOMoveY(targetPos.y, duration).SetEase(Ease.Linear));
                //seq.Insert(duration, uiItem.transform.DOScale(0, 0.3f).SetEase(Ease.OutCubic));
                seq.OnComplete(() =>
                {
                    m_playerResources.resources.AddResource(resourceType, resourceNumber);

                    GameObject.Destroy(uiItem);
                });               
            });
        }

        void SetScreenPositionFromWorldPosition(RectTransform item, Vector3 worldPos, Camera camera)
        {
            var pos = worldPos;
            var speenPos = camera.WorldToScreenPoint(pos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)item.parent, speenPos, null, out var anchorPos);
            item.anchoredPosition = anchorPos;
        }
    }
}
