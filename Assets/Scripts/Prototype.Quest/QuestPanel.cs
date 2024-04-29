using DG.Tweening;
using System;
using UnityEngine;

namespace Prototype
{
    public class QuestPanel : MonoBehaviour
    {
        private RectTransform m_RectTransform;
        private Vector2 m_StartAnchor;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_StartAnchor = m_RectTransform.anchoredPosition;
        }
        public void Show(Action onFinished = null)
        {
            var targetAnchor = m_StartAnchor;
            targetAnchor.y += 250;
            m_RectTransform.anchoredPosition = targetAnchor;
            m_RectTransform.DOAnchorPos(m_StartAnchor, 0.5f).OnComplete(() =>
            {
                onFinished?.Invoke();
            });
        }

        public void Hide(Action onFinished = null)
        {
            var targetAnchor = m_StartAnchor;
            targetAnchor.y = 250;
            m_RectTransform.DOAnchorPos(targetAnchor, 0.5f).OnComplete(() =>
            {
                onFinished?.Invoke();
            });
        }
    }
}