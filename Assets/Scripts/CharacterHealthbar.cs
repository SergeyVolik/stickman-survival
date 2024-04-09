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
        private MMHealthBar m_HealthBarPrefab;
        private MMHealthBar m_HealthBarInstance;
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
            m_Health.onHealthChanged += CharacterHealthbar_onHealthChanged;

            Assert.IsTrue(m_HealthBarPrefab != null);
            Assert.IsTrue(UIRoot != null);

            m_HealthBarInstance = GameObject.Instantiate(m_HealthBarPrefab, m_worldToScreen.Root);
            m_HealthBarInstance.TargetProgressBar.TextValueMultiplier = m_Health.maxHealth;
            m_HealthBarInstance.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, false);

            m_WTSHandle = m_worldToScreen.Register(new WordlToScreenUIItem { item = m_HealthBarInstance.GetComponent<RectTransform>(), worldPositionTransform = UIRoot });
        }

        private void OnDestroy()
        {
            m_worldToScreen.Unregister(m_WTSHandle);

            if(m_HealthBarInstance)
                GameObject.Destroy(m_HealthBarInstance.gameObject);
        }

        private void CharacterHealthbar_onHealthChanged(HealthChangeData obj)
        {
            bool show = m_Health.currentHealth != 0;
            m_HealthBarInstance.UpdateBar(m_Health.currentHealth, 0, m_Health.maxHealth, show);
        }
    }
}