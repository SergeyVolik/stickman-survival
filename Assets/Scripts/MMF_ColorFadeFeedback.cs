using UnityEngine;
using MoreMountains.Tools;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("You can add a description for your feedback here.")]
    [FeedbackPath("Custom/Flicker2")]
    public class MMF_ColorFadeFeedback : MMF_Feedback
    {
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return Duration; } }
        /// pick a color here for your feedback's inspector
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.DebugColor; } }
#endif
        [MMFInspectorGroup("Flicker", true, 0, false, true)]
        public Color FadeColor = new Color(1f, 1f, 1f, 1f);
        public Renderer Renderer;
        public float Duration = 1f;
        private EaseFunction easeFunction;
        private Color initCollor;
        public Ease EaseType;
        private Coroutine m_Coroutine;

        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            if (Renderer == null)
                return;

            easeFunction = EaseManager.ToEaseFunction(EaseType);
            initCollor = Renderer.material.color;
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (Renderer == null)
                return;

            if (!Active)
            {
                return;
            }

            if (m_Coroutine != null)
                Owner.StopCoroutine(m_Coroutine);

            m_Coroutine = Owner.StartCoroutine(Flicker(Renderer, initCollor, FadeColor, 1f));

            // your play code goes here
            Renderer.material.color = FadeColor;
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (Renderer == null)
                return;

            Renderer.material.color = initCollor;
            // your stop code goes here
        }

        public virtual IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerDuration)
        {
            IsPlaying = true;

            float currentTime = 0;


            while (currentTime < flickerDuration)
            {
                float t = easeFunction(currentTime, flickerDuration, 0, 0);
                renderer.material.color = Color.Lerp(flickerColor, initialColor, t);
                currentTime += Time.deltaTime;
                yield return null;   
            }

            IsPlaying = false;
        }

        protected override void CustomReset()
        {
            base.CustomReset();

            if (Renderer == null)
                return;

            if (InCooldown)
            {
                return;
            }


            Renderer.material.color = initCollor;
        }
    }
}