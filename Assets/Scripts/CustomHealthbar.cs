using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Prototype
{
    public class CustomHealthbar : MMHealthBar
    {
        private CanvasGroup m_FadeGroup;
        public float showDuration = 0.5f;
        public float hideDuration = 0.5f;

        public Ease hideEase;
        public Ease showEase;

        protected override void Awake()
        {
            base.Awake();
            m_FadeGroup = GetComponentInChildren<CanvasGroup>();
            m_FadeGroup.alpha = 0f;
        }

        bool showed = false;
        private TweenerCore<float, float, FloatOptions> m_Tween;

        [Button]
        public override void ShowBar(bool state)
        {
            if (state)
            {
                _progressBar.gameObject.SetActive(true);
                if (!showed)
                {
                    //Debug.Log("Show HB", gameObject);
                    showed = true;
                    _progressBar.gameObject.SetActive(true);
                    m_Tween?.Kill();
                    m_Tween = m_FadeGroup.DOFade(1f, showDuration).SetEase(showEase);

                    var currentVarTarget = _progressBar.BarTarget;
                    _progressBar.BarTarget = currentVarTarget - 0.01f;
                    _progressBar.UpdateBar01(currentVarTarget);
                }
            }
            else
            {
                if (showed)
                {
                    //Debug.Log("Hide HB", gameObject);
                    showed = false;
                    m_Tween?.Kill();
                    m_Tween = m_FadeGroup.DOFade(0, hideDuration).SetEase(hideEase).OnComplete(() => {
                        _progressBar.gameObject.SetActive(false);
                    });
                }
            }
        }
    }

}