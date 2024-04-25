using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

namespace Prototype
{
    public interface IShowable
    {
        void Show();

        void Hide();
    }

    public interface IFadeScreen : IShowable
    {
        public float TweenDuration { get; set; }
        public Ease ShowEase { get; set; }
        public Ease HideEase { get; set; }
        public void ShowInstant();
        public void HideInstant();
        public void Hide(Action onFinished = null);
        public void Show(Action onFinished = null);

    }


    public class FadeScreenUI : Singleton<FadeScreenUI>, IFadeScreen
    {
        [field:SerializeField]
        public float TweenDuration { get; set; } = 1;

        [field: SerializeField]
        public Ease ShowEase { get; set; }= Ease.Linear;

        [field: SerializeField]
        public Ease HideEase { get; set; }= Ease.Linear;

        [SerializeField]
        CanvasGroup CanvasGroup;
        private TweenerCore<float, float, FloatOptions> lastTween;

        private void Awake()
        {
            gameObject.SetActive(false);
            CanvasGroup.alpha = 0f;
        }

        public void ShowInstant()
        {
            CanvasGroup.alpha = 1f;
        }
        public void HideInstant()
        {
            CanvasGroup.alpha = 0f;
        }

        [Button]
        public void Hide(Action onFinished = null)
        {
            lastTween?.Kill();
            gameObject.SetActive(true);
            lastTween =  CanvasGroup.DOFade(0, TweenDuration).SetEase(HideEase).OnComplete(() => {
                gameObject.SetActive(false);
                onFinished?.Invoke();
            });
        }

        [Button]
        public void Show(Action onFinished = null)
        {
            lastTween?.Kill();
            gameObject.SetActive(true);
            lastTween = CanvasGroup.DOFade(1, TweenDuration).SetEase(ShowEase).OnComplete(() => {
                onFinished?.Invoke();
            });
        }

        public void Show()
        {
            Show(null);
        }

        public void Hide()
        {
            Hide(null);
        }
    }

}
