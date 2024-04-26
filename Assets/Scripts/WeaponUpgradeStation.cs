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

    public class WeaponUpgradeStation : MonoBehaviour
    {
        public WeaponLevelUpgrade[] upgradesList;
        public int currentUpgradeLevel;
        public UpgradeWeaponUI uiPrefab;
        public Transform uiBindPoint;
        private PlayerResources m_PlayerResources;
        private WorldToScreenUIManager m_wtsManager;
        private IPlayerFactory m_playerFactory;
        private UpgradeWeaponUI m_UIInstance;
        public PhysicsCallbacks playerTrigger;
        private bool m_PlayerInside;
        private WordlToScreenUIItem m_WorldToScreenHandle;

        [Inject]
        public void Construct(
         PlayerResources resources,
         WorldToScreenUIManager wtsManager,
         IPlayerFactory playerFactory)
        {
            m_PlayerResources = resources;
            m_wtsManager = wtsManager;
            m_playerFactory = playerFactory;
        }

        private void Awake()
        {
            m_UIInstance = GameObject.Instantiate(uiPrefab, m_wtsManager.Root);
            m_UIInstance.upgradeButton.onClick.AddListener(() =>
            {
                TryUpgrade();
            });
           
            SetupTrigger();
            m_UIInstance.gameObject.SetActive(false);
            m_UIInstance.Deactivate();
            ShowNextItem();
        }

        public void ShowNextItem()
        {
            foreach (var item in upgradesList)
            {
                item.visualItem.SetActive(false);
            }
            upgradesList[currentUpgradeLevel].visualItem.SetActive(true);
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
                m_UIInstance.Deactivate();
                m_playerFactory.CurrentPlayerUnit.GetComponent<CharacterInventory>().SetupMeleeWeapon(upgradeRes.weaponPrefab);
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
                m_UIInstance.title.text = $"Craft level {currentUpgradeLevel+2} weapon";
                UpdateResourceItem(m_UIInstance.resource1, upgradeRes.item1);
                UpdateResourceItem(m_UIInstance.resource2, upgradeRes.item2);

                m_PlayerInside = true;
                    m_UIInstance.Activate();
            };

            playerTrigger.onTriggerExit += (col) =>
            {
                m_PlayerInside = false;
                m_UIInstance.Deactivate();
            };
        }
    }
}
