using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class TransferMoveManager : MonoBehaviour
    {
        private Camera m_Camera;
        private PlayerResourceUI m_resourceUi;
        private PlayerResources m_playerResources;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        [Inject]
        void Construct(PlayerResourceUI resourceUi, PlayerResources playerResources)
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
            var resourceObjectInstance = GameObject.Instantiate(resourceType.Resource3dItem);

            resourceObjectInstance.transform.position = startPosition;

            resourceObjectInstance.SetActive(true);

            var rb = resourceObjectInstance.GetComponent<Rigidbody>();

            rb.transform.position = startPosition;

            //rotate RigidBody vector by angles
            var angle1 = quaternion.AxisAngle(math.forward(), UnityEngine.Random.Range(math.radians(-maxVectorAngleOffset), math.radians(maxVectorAngleOffset)));
            var angle2 = quaternion.AxisAngle(math.right(), UnityEngine.Random.Range(math.radians(-maxVectorAngleOffset), math.radians(maxVectorAngleOffset)));
            var velocityWithOffset = math.mul(angle1, math.mul(angle2, initialVelocity));

            rb.velocity = velocityWithOffset;

            var uiTarget = m_resourceUi.UI.GetResourceItemsByType(resourceType);

            DOVirtual.DelayedCall(moveDelay, () =>
            {
                var uiTarget = m_resourceUi.UI.GetResourceItemsByType(resourceType);
                var uiItem = GameObject.Instantiate(resourceType.Resource2dItem, m_resourceUi.UI.transform.parent);

                SetScreenPositionFromWorldPosition(uiItem.GetComponent<RectTransform>(), rb.transform.position, m_Camera);
                GameObject.Destroy(rb.gameObject);

                var targetPos = uiTarget.spriteImage.transform.position;

                var seq = DOTween.Sequence();
                var duration = moveDuration * UnityEngine.Random.Range(0.8f, 1f);
                seq.Insert(0, uiItem.transform.DOMoveX(targetPos.x, duration).SetEase(Ease.InCubic));
                seq.Insert(0, uiItem.transform.DOMoveY(targetPos.y, duration).SetEase(Ease.Linear));
                seq.Insert(duration*0.9f, uiItem.transform.DOScale(0, duration * 0.1f).SetEase(Ease.Linear));
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
