using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    [System.Serializable]
    public class WeaponLevelUpgrade
    {
        public ResourceItem item1;
        public ResourceItem item2;
        public GameObject visualItem;
        public MeleeWeapon weaponPrefab;
    }

    public class WeaponUpgradeStation : MonoBehaviour, IRequiredResourceContainer
    {
        public WeaponLevelUpgrade[] upgradesList;
        public int currentUpgradeLevel;
        public UpgradeWeaponUI uiPrefab;
        public Transform uiBindPoint;
        private PlayerResources m_PlayerResources;
        private WorldToScreenUIManager m_wtsManager;
        private IPlayerFactory m_playerFactory;
        private ItemPreviewUIPage m_ItemPreview;
        private UpgradeWeaponUI m_UIInstance;
        public PhysicsCallbacks playerTrigger;
        private WordlToScreenUIItem m_WorldToScreenHandle;

        private ResourceContainer requiredResources = new ResourceContainer();
        public ResourceContainer RequiredResources => requiredResources;

        public event Action<WeaponLevelUpgrade> onWeaponCrafted = delegate { };

        public WeaponLevelUpgrade GetCurrentCraftItem() => currentUpgradeLevel > upgradesList.Length -1 ? null : upgradesList[currentUpgradeLevel];
        public WeaponLevelUpgrade GetLastCraftedItem() => currentUpgradeLevel == 0 ? null : upgradesList[currentUpgradeLevel - 1];

        [Inject]
        public void Construct(
         PlayerResources resources,
         WorldToScreenUIManager wtsManager,
         IPlayerFactory playerFactory,
         ItemPreviewUIPage itemPreview)
        {
            m_PlayerResources = resources;
            m_wtsManager = wtsManager;
            m_playerFactory = playerFactory;
            m_ItemPreview = itemPreview;
        }

        private void Awake()
        {
            m_UIInstance = GameObject.Instantiate(uiPrefab, m_wtsManager.Root);
            m_UIInstance.upgradeButton.onClick.AddListener(() =>
            {
                TryUpgrade();
            });

            SetupTrigger();
            m_UIInstance.Deactivate();
            m_UIInstance.gameObject.SetActive(false);

            ShowNextItem();
            SetupNextRequiredResources();
        }

        private void SetupNextRequiredResources()
        {
            var currentCraftItem = GetCurrentCraftItem();
          
            requiredResources.Clear();

            if (currentCraftItem == null)
                return;
            
            requiredResources.AddResource(currentCraftItem.item1.resourceType, currentCraftItem.item1.count);
            requiredResources.AddResource(currentCraftItem.item2.resourceType, currentCraftItem.item2.count);
        }

        public void ShowNextItem()
        {
            foreach (var item in upgradesList)
            {
                item.visualItem.SetActive(false);
            }

            if (currentUpgradeLevel < upgradesList.Length)
            {
                upgradesList[currentUpgradeLevel].visualItem.SetActive(true);
            }
        }

        private void TryUpgrade()
        {
            var upgradeRes = upgradesList[currentUpgradeLevel];

            var res1 = upgradeRes.item1.count;
            var res2 = upgradeRes.item2.count;

            var current1 = m_PlayerResources.resources.GetResource(upgradeRes.item1.resourceType);
            var current2 = m_PlayerResources.resources.GetResource(upgradeRes.item2.resourceType);

            if (current1 >= res1 && current2 >= res2)
            {
                m_PlayerResources.resources.RemoveResource(upgradeRes.item1.resourceType, res1);
                m_PlayerResources.resources.RemoveResource(upgradeRes.item2.resourceType, res2);
                currentUpgradeLevel++;
                ShowNextItem();
                SetupNextRequiredResources();
                m_UIInstance.Deactivate();
                m_playerFactory.CurrentPlayerUnit.GetComponent<CharacterInventory>().SetupMeleeWeapon(upgradeRes.weaponPrefab);
                onWeaponCrafted.Invoke(upgradeRes);
            }
        }

        private void OnEnable()
        {
            m_WorldToScreenHandle = m_wtsManager.Register(new WordlToScreenUIItem
            {
                worldPositionTransform = uiBindPoint,
                item = m_UIInstance.GetComponent<RectTransform>()
            });
        }

        private void OnDestroy()
        {
            m_wtsManager.Unregister(m_WorldToScreenHandle);
        }

        private void UpdateResourceItem(ResourceUIItem resourceItem, ResourceItem item1)
        {
            var res1 = m_PlayerResources.resources.GetResource(item1.resourceType);

            resourceItem.SetSprite(item1.resourceType.resourceIcon);
            resourceItem.SetText(item1.count.ToString());
            resourceItem.SetTextColor(res1 >= item1.count ? Color.green : Color.red);
        }

        private void SetupTrigger()
        {
            playerTrigger.onTriggerEnter += (col) =>
            {
                if (currentUpgradeLevel >= upgradesList.Length)
                {
                    return;
                }

                var upgradeRes = upgradesList[currentUpgradeLevel];

                var res1 = m_PlayerResources.resources.GetResource(upgradeRes.item1.resourceType);
                m_UIInstance.title.text = $"Craft level {upgradeRes.weaponPrefab.GetComponent<MeleeWeapon>().level} weapon";
                UpdateResourceItem(m_UIInstance.resource1, upgradeRes.item1);
                UpdateResourceItem(m_UIInstance.resource2, upgradeRes.item2);

                m_UIInstance.Activate();
            };

            playerTrigger.onTriggerExit += (col) =>
            {
                m_UIInstance.Deactivate();
            };
        }
    }
}
