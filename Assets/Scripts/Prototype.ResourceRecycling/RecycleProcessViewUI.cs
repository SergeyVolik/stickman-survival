using Prototype.Ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class RecycleProcessViewUI : ActivatableUI
    {
        [SerializeField] public Slider m_Slider;
        private ResourceRecycling m_Recicling;
        [SerializeField] private Image m_ResourceImage;
        [SerializeField] private TextMeshProUGUI m_TimerText;
        [SerializeField] private Button m_CalimResourcesButton;
        [SerializeField] private Button m_SkipButton;
        private IAdsService m_adsPlayer;

        protected override void Awake()
        {
            base.Awake();

            m_CalimResourcesButton.onClick.AddListener(() =>
            {
                Deactivate();
                m_Recicling.ProcessFinish();
            });

            m_SkipButton.onClick.AddListener(() =>
            {
                m_adsPlayer.ShowRewardAd(() => {
                    m_Recicling.FinishTimer();                  
                });         
            });
        }

        public void Bind(ResourceRecycling recicling, IAdsService adsPlayer)
        {
            m_adsPlayer = adsPlayer;
            m_ResourceImage.sprite = recicling.destinationResource.resourceIcon;
            m_Recicling = recicling;
        }

        private void Update()
        {
            if (m_Recicling == null)
                return;

            if (!m_Recicling.IsTimerFinished())
            {
                ActivateTimerState();
                m_Slider.value = m_Recicling.GetProgress01();
                m_TimerText.text = m_Recicling.GetTimerText();
            }
            else
            {
                ActivateClaimButtonState();
            }
        }

        private void ActivateTimerState()
        {
            m_Slider.gameObject.SetActive(true);
            m_TimerText.gameObject.SetActive(true);
            m_CalimResourcesButton.gameObject.SetActive(false);
        }

        private void ActivateClaimButtonState()
        {
            m_Slider.gameObject.SetActive(false);
            m_TimerText.gameObject.SetActive(false);
            m_CalimResourcesButton.gameObject.SetActive(true);
        }
    }
}