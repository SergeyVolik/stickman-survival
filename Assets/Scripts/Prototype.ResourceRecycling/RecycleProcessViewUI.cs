using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class RecycleProcessViewUI : ActivatableUI
    {
        [SerializeField] public Slider m_Slider;
        private ResourceRecycling m_recicling;
        [SerializeField] private Image m_ResourceImage;
        [SerializeField] private TextMeshProUGUI m_TimerText;
        [SerializeField] private Button m_CalimResourcesButton;

        protected override void Awake()
        {
            base.Awake();

            m_CalimResourcesButton.onClick.AddListener(() =>
            {
                Deactivate();
                m_recicling.ProcessFinish();
            });
        }

        public void Bind(ResourceRecycling recicling)
        {
            m_ResourceImage.sprite = recicling.destinationResource.resourceIcon;
            m_recicling = recicling;
        }

        private void Update()
        {
            if (m_recicling == null)
                return;

            if (!m_recicling.IsTimerFinished())
            {
                ActivateTimerState();
                m_Slider.value = m_recicling.GetProgress01();
                m_TimerText.text = m_recicling.GetTimerText();
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