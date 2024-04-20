using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class PlayerTakeDamageFeedback : MonoBehaviour
    {
        public float duration = 0.5f;
        public Ease ease = Ease.InSine;
        [SerializeField]
        private MMF_Player m_feedback;
        public Image redSprite;
        public Color damageColor;
        private void Awake()
        {
            m_feedback = GetComponent<MMF_Player>();
        }

        [Inject]
        void Construct(IPlayerFactory factory)
        {
            factory.onPlayerSpawned += Factory_onPlayerSpawned;
            redSprite.color = new Color();
        }

        private void Factory_onPlayerSpawned(GameObject obj)
        {
            obj.GetComponent<HealthData>().onHealthChanged += PlayerTakeDamageFeedback_onHealthChanged;
        }

        private void PlayerTakeDamageFeedback_onHealthChanged(HealthChangeData obj)
        {
            if (obj.IsDamage)
            {
                m_feedback?.PlayFeedbacks();

                var target = damageColor;
                target.a = 0;
                redSprite.color = damageColor;
                redSprite.DOColor(target, duration).SetEase(ease);
            }
        }
    }
}