using Prototype;
using Prototype.Ads;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class BustButton : MonoBehaviour
    {
        public Effect effectPrefab;
        private IPlayerFactory m_factory;
        private IAdsPlayer m_adsManager;
        public Button button;
        public Image timerImage;
        public RectTransform timerHolder;
        private Effect m_CurrentEffect;

        float noBuffTimerT;
        public float activateButtonTimer;
        [Inject]
        void Construct(IPlayerFactory factory, IAdsPlayer adsManager)
        {
            m_factory = factory;
            m_adsManager = adsManager;
        }

        private void Awake()
        {
            button.gameObject.SetActive(false);
            timerHolder.gameObject.SetActive(false);
            button.onClick.AddListener(() =>
            {
                if (!m_factory.CurrentPlayerUnit)
                    return;

                m_adsManager.ShowRewardAd(null);
                m_CurrentEffect = m_factory.CurrentPlayerUnit.GetComponent<UnitEffects>().AddEffect(effectPrefab);
                noBuffTimerT = 0;
                button.gameObject.SetActive(false);
            });
        }

        private void Update()
        {

            if (m_CurrentEffect != null)
            {
                timerHolder.gameObject.SetActive(true);
                button.gameObject.SetActive(false);
                timerImage.fillAmount = 1f - m_CurrentEffect.currentTime / m_CurrentEffect.duration;
            }
            else {
                noBuffTimerT += Time.deltaTime;

                if (noBuffTimerT > activateButtonTimer)
                {
                    button.gameObject.SetActive(true);
                }
            }
        }
    }
}