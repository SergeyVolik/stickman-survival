using DG.Tweening;
using Prototype.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class ItemPreviewUIPage : UIPage
    {       
        public TextMeshProUGUI title;
        public Button returnButton;
        private ItemPreviewCamera m_previewCamera;

        [Inject]
        void Construct(ItemPreviewCamera previewCamera)
        {
            m_previewCamera = previewCamera;
        }

        public override void Show()
        {
            base.Show();
            Time.timeScale = 0;
            m_previewCamera.Activate();
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
            Time.timeScale = 1;
            m_previewCamera.Dectivate();
        }

        internal void ShowPreview(ItemPreviewSO previewSetting)
        {
            title.text = previewSetting.title;
            returnButton.transform.localScale = Vector3.zero;
            m_previewCamera.SetupPreview(previewSetting, () => {
                returnButton.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            });

            Navigate();
        }
    }
}
