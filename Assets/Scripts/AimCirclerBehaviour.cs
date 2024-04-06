using DG.Tweening;
using Prototype;
using RobinGoodfellow.CircleGenerator;
using UnityEngine;

namespace Prototype
{

    public class AimCirclerBehaviour : MonoBehaviour
    {
        [SerializeField]
        public CircleGenerator m_AimCorclePrefab;
        private bool m_Awaked;
        private CircleGenerator m_AimRangeCircle;

        private Transform m_CircleTransform;
        private MeshRenderer m_Renderer;
        private Color m_StartColor;
        private Transform m_Transform;
        public float rotationSpeed;

        private void Awake()
        {
            if (m_Awaked)
                return;

            m_Awaked = true;

            m_AimRangeCircle = GameObject.Instantiate(m_AimCorclePrefab);
            m_CircleTransform = m_AimRangeCircle.transform;
          
            m_Renderer = m_CircleTransform.GetComponent<MeshRenderer>();
            m_StartColor = m_Renderer.material.color;
            m_Transform = transform;
            GetComponent<CharacterInventory>().onMainWeaponChanged += AimCirclerBehaviour_onGunChanged;
            m_CircleTransform.gameObject.SetActive(false);
        }

        bool m_Showed = false;
        private Sequence m_Hide;
        private Sequence m_Show;

        public void Show()
        {
            Awake();

            if (m_Showed) return;

            m_Hide?.Kill();

            enabled = true;
            m_Showed = true;
            m_CircleTransform.gameObject.SetActive(true);
            m_CircleTransform.localScale = Vector3.zero;

            var initColor = m_StartColor;
            initColor.a = 0;

            m_Renderer.material.color = initColor;

            m_Show = DOTween.Sequence();

            m_Show.Insert(0, m_Renderer.material.DOColor(m_StartColor, 1f).SetEase(Ease.OutSine));
            m_Show.Insert(0, m_CircleTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutSine));
        }

        public void Hide()
        {
            Awake();

            m_Show?.Kill();

            m_Showed = false;
            var targetColor = m_StartColor;
            targetColor.a = 0;

            m_Hide = DOTween.Sequence();

            m_Hide.Insert(0, m_Renderer.material.DOColor(targetColor, 1f).SetEase(Ease.OutSine));
            m_Hide.Insert(0, m_CircleTransform.DOScale(Vector3.zero, 1f).SetEase(Ease.InSine));

            m_Hide.OnComplete(() => {
                enabled = false;
            });
        }

        private void AimCirclerBehaviour_onGunChanged(Gun obj)
        {
            if (obj == null) return;

            var circle = m_AimRangeCircle.CircleData;
            circle.Radius = obj.aimDistance;
            m_AimRangeCircle.CircleData = circle;
            m_AimRangeCircle.Generate();
        }

        private void Update()
        {
            m_CircleTransform.position = m_Transform.position + new Vector3(0, 0.01f, 0);
            m_CircleTransform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
        }
    }
}