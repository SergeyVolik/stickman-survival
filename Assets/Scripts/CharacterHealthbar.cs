using MoreMountains.Tools;
using Prototype;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Prototype
{
    public class CharacterHealthbar : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_HealthBarPrefab;
        private CustomHealthbar m_HealthBarInstance;

        [SerializeField]
        private Transform UIRoot;

        private HealthData m_Health;
        private WorldToScreenUIManager m_worldToScreen;
        private WordlToScreenUIItem m_WTSHandle;

        [Inject]
        void Construct(WorldToScreenUIManager worldToScreen)
        {
            m_worldToScreen = worldToScreen;
        }

        private void Awake()
        {
            m_Health = GetComponent<HealthData>();
            m_Health.onHealthChanged += (evt) => UpdateHB();
            m_Health.onDeath += M_Health_onDeath;
            Assert.IsTrue(m_HealthBarPrefab != null);
            Assert.IsTrue(UIRoot != null);

            SetupUI();
            
        }

        private void SetupUI()
        {
            m_HealthBarInstance = GameObject.Instantiate(m_HealthBarPrefab, m_worldToScreen.Root).GetComponent<CustomHealthbar>();
            m_HealthBarInstance.TargetProgressBar.TextValueMultiplier = m_Health.maxHealth;
            m_HealthBarInstance.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, false);
            m_HealthBarInstance.onEnabled += M_HealthBarInstance_onEnabled;
            m_HealthBarInstance.HideHealthbar();
        }

        private void M_HealthBarInstance_onEnabled(bool obj)
        {
            if (obj)
            {
                RegisterCanvas();
            }
            else {
                UnregisterCanvas();
            }
        }

        void RegisterCanvas()
        {
            m_WTSHandle = m_worldToScreen.Register(new WordlToScreenUIItem {
                item = m_HealthBarInstance.GetComponent<RectTransform>(),
                worldPositionTransform = UIRoot 
            });
        }

        private void M_Health_onDeath()
        {
            m_HealthBarInstance.gameObject.SetActive(false);
            m_worldToScreen.Unregister(m_WTSHandle);
        }

        private void OnDisable()
        {
           
        }
       
        private void Start()
        {
            UpdateHB();
        }

        private void OnDestroy()
        {
            UnregisterCanvas();

            if (m_HealthBarInstance)
                GameObject.Destroy(m_HealthBarInstance.gameObject);
        }

        private void UnregisterCanvas()
        {
            m_worldToScreen.Unregister(m_WTSHandle);
        }

        private void UpdateHB()
        {
            bool show = m_Health.currentHealth != 0;
            m_HealthBarInstance.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, show);
        }

        public void AlwaysVisible(bool alwaysVisible)
        {
            m_HealthBarInstance.AlwaysVisible = alwaysVisible;
        }

        public void Show(bool show)
        {          
            m_HealthBarInstance.ShowBar(show);

            if (show)
            {
                UpdateHB();
            }
        }
    }
}