using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Prototype;
using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Prototype
{
    public class RequiredResourcesBehaviour : MonoBehaviour
    {
        public ResourceContainer requiredResources;
        private ResourceContainer m_RequiredCurrentResources;


        public PhysicsCallbacks openUiTrigger;
        public RequiredResourceView requiredResourceUIViewPrefab;

        private RequiredResourceView m_requiredResourceUIViewInstance;
        public Transform uiBindPoint;
        private WorldToScreenUIManager m_wts;
        private PlayerResources m_playerResources;
        private WordlToScreenUIItem m_BindHandle;

        public event Action onFinished = delegate { };
        public UnityEvent onFinishedUI;
        private TweenerCore<Vector3, Vector3, VectorOptions> m_HideUiTween;

        [Inject]
        void Construct(WorldToScreenUIManager wts, PlayerResources playerResources)
        {
            m_wts = wts;
            m_playerResources = playerResources;
        }

        private void Awake()
        {
            openUiTrigger.onTriggerEnter += OpenUiTrigger_onTriggerEnter;
            openUiTrigger.onTriggerExit += OpenUiTrigger_onTriggerExit;

          
            m_RequiredCurrentResources = new ResourceContainer();
            m_RequiredCurrentResources.onResourceChanged += RequiredResources_onResourceChanged;

            m_requiredResourceUIViewInstance = GameObject.Instantiate(requiredResourceUIViewPrefab, m_wts.Root);
            m_requiredResourceUIViewInstance.transform.localScale = Vector3.zero;

            m_requiredResourceUIViewInstance.onItemAdded += M_requiredResourceUIViewInstance_onItemAdded;
            m_requiredResourceUIViewInstance.Bind(requiredResources, m_RequiredCurrentResources);
        }

        private void M_requiredResourceUIViewInstance_onItemAdded(ResourceTypeSO resourceType, RquiredResourceUIItem arg2)
        {
            arg2.addResource.onClick.AddListener(() =>
            {
                var current = m_RequiredCurrentResources.GetResource(resourceType);
                var required = requiredResources.GetResource(resourceType);
                var playerResource = m_playerResources.resources.GetResource(resourceType);
                var toAdd = required - current;

                toAdd = Math.Clamp(toAdd, 0, playerResource);

                m_playerResources.resources.RemoveResource(resourceType, toAdd);
                m_RequiredCurrentResources.AddResource(resourceType, toAdd);
            });
        }

        private void RequiredResources_onResourceChanged(ResourceTypeSO arg1, int arg2)
        {
            if (Finished())
            {
                DeactuvateUI(() => { gameObject.SetActive(false); });
                onFinished.Invoke();
                onFinishedUI.Invoke();                
            }
        }

        private bool Finished()
        {
            return requiredResources.Equals(m_RequiredCurrentResources);
        }

        private void OpenUiTrigger_onTriggerExit(Collider obj)
        {
            if (Finished())
            {
                return;
            }

            DeactuvateUI();
        }

        private void DeactuvateUI(Action callback = null)
        {
            m_HideUiTween = m_requiredResourceUIViewInstance.transform.DOScale(0, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    m_wts.Unregister(m_BindHandle);
                    m_BindHandle = null;
                    callback?.Invoke();
                });
        }

        private void ActuvateUI()
        {
            m_HideUiTween?.Kill();
            if (m_BindHandle == null)
            {
                m_BindHandle = m_wts.Register(new WordlToScreenUIItem
                {
                    item = m_requiredResourceUIViewInstance.GetComponent<RectTransform>(),
                    worldPositionTransform = uiBindPoint
                });
            }

            m_requiredResourceUIViewInstance.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        private void OpenUiTrigger_onTriggerEnter(Collider obj)
        {
            if (Finished())
            {
                return;
            }

            ActuvateUI();
        }
    }
}