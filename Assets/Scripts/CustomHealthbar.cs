using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Prototype
{
    public class CustomHealthbar : MMHealthBar
    {
        private CanvasGroup m_FadeGroup;
        private Canvas m_Canvas;
        public float showDuration = 0.5f;
        public float hideDuration = 0.5f;

        public Ease hideEase;
        public Ease showEase;

        public event Action<bool> onEnabled = delegate { };

        protected override void Awake()
        {
            base.Awake();
            m_FadeGroup = GetComponentInChildren<CanvasGroup>();
            m_Canvas = GetComponent<Canvas>();
            m_FadeGroup.alpha = 0f;
        }

        bool showed = true;
        private TweenerCore<float, float, FloatOptions> m_Tween;

        public void HideHealthbar()
        {
            showed = false;
            m_Canvas.enabled = false;
            onEnabled.Invoke(false);
        }


        [Button]
        public override void ShowBar(bool state)
        {
            if (state)
            {
                m_Canvas.enabled = true;
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
                    onEnabled.Invoke(true);
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
                        m_Canvas.enabled = false;
                        onEnabled.Invoke(false);
                    });
                }
            }
        }
    }

}