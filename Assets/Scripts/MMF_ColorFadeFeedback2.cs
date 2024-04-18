using UnityEngine;
using MoreMountains.Tools;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("You can add a description for your feedback here.")]
    [FeedbackPath("Custom/Flicker3")]
    public class MMF_ColorFadeFeedback2 : MMF_Flicker
    {
        public Ease EaseType;
        public bool useAnimationCurve;
        public AnimationCurve curve;

        float AnimCurveFunction(float time, float duration, float overshootOrAmplitude, float period) => curve.Evaluate(time/duration);
        public override IEnumerator Flicker(Renderer renderer, int materialIndex, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            if (!_propertiesFound[materialIndex])
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            flickerDuration =  Mathf.Clamp(flickerDuration, 0, flickerDuration);
            float flickerStop = FeedbackTime + flickerDuration;
            IsPlaying = true;

            var easeFunction = useAnimationCurve ? AnimCurveFunction : EaseManager.ToEaseFunction(EaseType);
            StoreSpriteRendererTexture();
            float currentTime = 0;

            float t = easeFunction(currentTime, flickerDuration, 0, 0);
            var color = Color.Lerp(flickerColor, initialColor, t);
            SetColor(materialIndex, color);
            
            while (FeedbackTime < flickerStop)
            {
                currentTime += Time.deltaTime;
                t = easeFunction(currentTime, flickerDuration, 0, 0);
                color = Color.Lerp(flickerColor, initialColor, t);
                SetColor(materialIndex, color);
                yield return null;
            }

            SetColor(materialIndex, initialColor);
            IsPlaying = false;
        }
    }
}