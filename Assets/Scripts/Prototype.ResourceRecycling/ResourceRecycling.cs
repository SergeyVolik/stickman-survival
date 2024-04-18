using System;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Prototype
{
    [System.Serializable]
    public class ResourceRecyclingSave : ISaveComponentData
    {
        public int itemToRecycle;
        public DateTime startTime;
        public DateTime endTime;
        public SerializableGuid Id { get; set; }

        public override string ToString()
        {
            return $"starttime: {startTime} endtime: {endTime} itemToRecycle: {itemToRecycle}";
        }
    }

    public class ResourceRecycling : SaveableObject, ISceneSaveComponent<ResourceRecyclingSave>
    {
        public ResourceTypeSO sourceResource;
        public ResourceTypeSO destinationResource;

        [Min(1)]
        public int itemsToDestResource = 1;

        private PlayerResources m_PlayerResources;
        private WorldToScreenUIManager m_wtsManager;

        [SerializeField] private RecycleUI m_UIPrefab;
        [SerializeField] private RecycleProcessViewUI m_UIRecycleViewUiPrefab;
        private RecycleUI m_UIInstance;
        private bool m_Awaked;
        private RecycleProcessViewUI m_UIRecycleViewInstance;
        public PhysicsCallbacks playerTrigger;
        private WordlToScreenUIItem m_WorldToScreenHandle;
        [SerializeField] private Transform m_UiBindPoint;
        public float distanceToActivateUI = 2f;
        [Min(0.01f)]
        public float recycleItemDuration = 1f;
        public int itemToRecycle;
        private WordlToScreenUIItem m_WorldToScreenHandle2;

        public bool IsTimerFinished() => m_EndRecyclingTime <= DateTime.Now;

        private bool IsClaimed() => itemToRecycle == 0 && IsTimerFinished();

        private bool NeedActivateTrigger() => IsClaimed();

        private DateTime m_StartRecyclingTime;
        private DateTime m_EndRecyclingTime;
        private bool m_PlayerInside;

        public string GetTimerText()
        {
            var timeToEnd = m_EndRecyclingTime - DateTime.Now;

            if (timeToEnd.Hours != 0)
            {
                return $"{timeToEnd.Hours}h {timeToEnd.Minutes}m ";
            }
            else if (timeToEnd.Minutes != 0)
            {
                return $"{timeToEnd.Minutes}m {timeToEnd.Seconds}s ";
            }
            else if (timeToEnd.Seconds != 0)
            {
                return $"{timeToEnd.Seconds}s ";
            }
            
            return "0s";
        }

        public float GetProgress01()
        {
            var totalDuration = m_EndRecyclingTime - m_StartRecyclingTime;
            var timeToEnd = m_EndRecyclingTime - DateTime.Now;

            return (float) (1 -(timeToEnd.TotalSeconds / totalDuration.TotalSeconds));
        }

        [Inject]
        public void Construct(
             PlayerResources resources,
             WorldToScreenUIManager wtsManager)
        {
            m_PlayerResources = resources;
            m_wtsManager = wtsManager;
        }

        private void Awake()
        {
            if (m_Awaked)
                return;

            m_Awaked = true;
            m_UIRecycleViewInstance = GameObject.Instantiate(m_UIRecycleViewUiPrefab, m_wtsManager.Root);
            m_UIInstance = GameObject.Instantiate(m_UIPrefab, m_wtsManager.Root);
            m_UIRecycleViewInstance.Deactivate();
            m_UIInstance.Deactivate();

            m_UIRecycleViewInstance.Bind(this);

            SetupTrigger();

            m_UIInstance.m_TransferButton.onClick.AddListener(() =>
            {
                int nextRecicle = (int)(m_UIInstance.m_Slider.value);
                m_PlayerResources.resources.RemoveResource(sourceResource, nextRecicle * itemsToDestResource);
                itemToRecycle = nextRecicle;
                m_StartRecyclingTime = DateTime.Now;
                m_EndRecyclingTime = m_StartRecyclingTime + TimeSpan.FromSeconds(itemToRecycle * recycleItemDuration);
                m_UIInstance.Deactivate();
                m_UIRecycleViewInstance.Activate();
            });

            m_UIInstance.m_Slider.onValueChanged.AddListener((v) =>
            {
                Resources_onResourceChanged(null, 0);
            });           
        }

        public void ProcessFinish()
        {
            m_PlayerResources.resources.AddResource(destinationResource, itemToRecycle);
            itemToRecycle = 0;

            m_UIRecycleViewInstance.Deactivate();

            if (m_PlayerInside)
            {
                m_UIInstance.Activate();
            }
        }

        private void SetupTrigger()
        {
            playerTrigger.onTriggerEnter += (col) =>
            {
                m_PlayerInside = true;
                if(NeedActivateTrigger())
                    m_UIInstance.Activate();
            };

            playerTrigger.onTriggerExit += (col) =>
            {
                m_PlayerInside = false;
                if (NeedActivateTrigger())
                    m_UIInstance.Deactivate();
            };
        }

        private void OnEnable()
        {
            m_PlayerResources.resources.onResourceChanged += Resources_onResourceChanged;
            Resources_onResourceChanged(null, 0);

            m_WorldToScreenHandle2 = m_wtsManager.Register(new WordlToScreenUIItem
            {
                worldPositionTransform = m_UiBindPoint,
                item = m_UIRecycleViewInstance.GetComponent<RectTransform>()
            });

            m_WorldToScreenHandle = m_wtsManager.Register(new WordlToScreenUIItem
            {
                worldPositionTransform = m_UiBindPoint,
                item = m_UIInstance.GetComponent<RectTransform>()
            });
        }

        private void OnDisable()
        {
            if (m_UIRecycleViewInstance)
                m_UIRecycleViewInstance.gameObject.SetActive(false);

            if (m_UIInstance)
                m_UIInstance.gameObject.SetActive(false);

            m_wtsManager.Unregister(m_WorldToScreenHandle2);
            m_wtsManager.Unregister(m_WorldToScreenHandle);

            m_PlayerResources.resources.onResourceChanged -= Resources_onResourceChanged;
        }

        private void Resources_onResourceChanged(ResourceTypeSO arg1, int arg2)
        {
            var playerResourceNumber = m_PlayerResources.resources.GetResource(sourceResource);

            m_UIInstance.m_TransferButton.enabled = playerResourceNumber >= itemsToDestResource;

            int canBeRecicled = playerResourceNumber / itemsToDestResource;

            m_UIInstance.m_Slider.minValue = 0;
            m_UIInstance.m_Slider.maxValue = canBeRecicled;

            int nextRecicle = (int)(m_UIInstance.m_Slider.value);

            m_UIInstance.m_SliderView.minValue = 0;
            m_UIInstance.m_SliderView.maxValue = 1f;
            float offsetteValue = math.remap(0, 1f, 0, 1 - m_UIInstance.m_ViewInitValue, canBeRecicled != 0 ? nextRecicle / (float)canBeRecicled : 0f);
            m_UIInstance.m_SliderView.value = m_UIInstance.m_ViewInitValue + offsetteValue;

            m_UIInstance.sourceResourceUI.SetSprite(sourceResource.resourceIcon);
            m_UIInstance.sourceResourceUI.SetText(TextUtils.IntToText(nextRecicle * itemsToDestResource));

            m_UIInstance.destionationResourceUI.SetSprite(destinationResource.resourceIcon);
            m_UIInstance.destionationResourceUI.SetText(TextUtils.IntToText(nextRecicle));
        }

        public ResourceRecyclingSave SaveComponent()
        {
            var item = new ResourceRecyclingSave
            {
                itemToRecycle = itemToRecycle,
                startTime = m_StartRecyclingTime,
                endTime = m_EndRecyclingTime,
            };

            return item;
        }

        public void LoadComponent(ResourceRecyclingSave data)
        {
            if (data == null)
                return;

            itemToRecycle = data.itemToRecycle;
            m_StartRecyclingTime = data.startTime;
            m_EndRecyclingTime = data.endTime;

            Awake();

            if (!IsTimerFinished())
            {
                m_UIInstance.Deactivate();
                m_UIRecycleViewInstance.Activate();
            }
        }
    }
}