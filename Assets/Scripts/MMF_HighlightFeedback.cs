using UnityEngine;
using MoreMountains.Tools;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("You can add a description for your feedback here.")]
    [FeedbackPath("Custom/HighlightFeedback")]
    public class MMF_HighlightFeedback : MMF_Feedback
    {
        /// use this override to specify the duration of your feedback (don't hesitate to look at other feedbacks for reference)
        public override float FeedbackDuration { get { return Duration; } }

        [MMFInspectorGroup("HighlightFeedback", true, 0, false, true)]
        public Renderer Renderer;
        public Renderer[] AdditionalRenderers;
        public float Duration = 1f;
        public Color highlightColor = new Color(1f, 1f, 1f, 1f);

        public Ease EaseType;
        public bool useAnimationCurve;
        public AnimationCurve curve;
        private Coroutine m_Coroutine;

        float AnimCurveFunction(float time, float duration, float overshootOrAmplitude, float period) => curve.Evaluate(time / duration);

        private static Material matPrefab;
        private static Material GetAnimationMaterial() {
            if (matPrefab == null)
            {
                matPrefab = Resources.Load<Material>("WhiteHit");
            }

            return matPrefab;
        }
        private class AnimationData
        {
            public Renderer Renderer;
            public Material AnimatedMaterial;
        }

        List<Material> toChangeList = new List<Material>();
        private List<AnimationData> allAnimatedRenderers = new List<AnimationData>();
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);

            if(Renderer != null)
                allAnimatedRenderers.Add(SetupRenderer(Renderer));

            if (AdditionalRenderers != null)
            {
                for (int i = 0; i < AdditionalRenderers.Length; i++)
                {
                    if (AdditionalRenderers[i] != null)
                    allAnimatedRenderers.Add(SetupRenderer(AdditionalRenderers[i]));
                }
            }
        }

        private AnimationData SetupRenderer(Renderer renderer)
        {
            var materialInstance = GameObject.Instantiate(GetAnimationMaterial());
            materialInstance.name = "MMF_ColorFade (Instance)";
          
            return new AnimationData
            {
                Renderer = renderer,
                AnimatedMaterial = materialInstance
            };
        }

        private void SetupMaterials()
        {          
            foreach (var item in allAnimatedRenderers)
            {
                var materials = item.Renderer.sharedMaterials.ToList();
                materials.Add(item.AnimatedMaterial);
                item.Renderer.sharedMaterials = materials.ToArray();
            }
        }

        private void ClearMaterials()
        {         
            foreach (var item in allAnimatedRenderers)
            {
                
                var materials = item.Renderer.sharedMaterials.ToList();
                int count = materials.Count;
                materials.Remove(item.AnimatedMaterial);

                if(materials.Count != count)
                    item.Renderer.sharedMaterials = materials.ToArray();
            }
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

            m_Coroutine = Owner.StartCoroutine(Flicker(highlightColor, new Color(), Duration));
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (Renderer == null)
                return;
        }

        public virtual IEnumerator Flicker(Color source, Color destination, float duration)
        {
            IsPlaying = true;
            SetupMaterials();
            float currentTime = 0;
            var easeFunction = useAnimationCurve ? AnimCurveFunction : EaseManager.ToEaseFunction(EaseType);

            while (currentTime < duration)
            {
                float t = easeFunction(currentTime, duration, 0, 0);
                var color = Color.Lerp(source, destination, t);

                foreach (var item in allAnimatedRenderers)
                {
                    item.AnimatedMaterial.color = color;
                }

                currentTime += Time.deltaTime;

                yield return null;   
            }

            ClearMaterials();
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

            ClearMaterials();
        }
    }
}