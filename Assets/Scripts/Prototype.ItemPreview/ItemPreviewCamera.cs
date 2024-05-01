using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Prototype
{
    public class ItemPreviewCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera m_PreviewCamera;
        public Transform cameraTarget;
        private GameObject m_PreviewInstance;
        private bool m_NeedReset;
        private float m_ResetT;
        public float resetSpeed = 5f;
        public float timeToReset = 1f;

        private void Awake()
        {
            Dectivate();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Dectivate()
        {
            gameObject.SetActive(false);
        }

        public void SetupPreview(ItemPreviewSO previewSetting, Action onShowed = null)
        {
            if (m_PreviewInstance)
            {
                GameObject.Destroy(m_PreviewInstance);
            }

            cameraTarget.localScale = Vector3.zero;
            cameraTarget.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
            {
                onShowed?.Invoke();
            });

            m_PreviewInstance = GameObject.Instantiate(previewSetting.previewPrefab, cameraTarget);
            m_PreviewInstance.transform.localPosition = previewSetting.previewOffset;
            m_PreviewInstance.transform.localRotation = Quaternion.Euler(previewSetting.previewRotation);
        }

        private void Update()
        {
            if (m_NeedReset)
            {
                m_ResetT += Time.unscaledDeltaTime;
                if (m_ResetT > timeToReset)
                {
                    cameraTarget.rotation = Quaternion.Slerp(cameraTarget.rotation, Quaternion.identity, Time.unscaledDeltaTime * resetSpeed);
                }
            }
        }

        public void RotateCamera(float rotateAngle)
        {
            m_NeedReset = true;
            m_ResetT = 0;
            cameraTarget.Rotate(Vector3.up, rotateAngle);
        }
    }
}