using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Prototype
{
    [System.Serializable]
    public class GlobalSave
    {
        public List<ResourceSaveItem> playerResources = new List<ResourceSaveItem>();
    }

    [System.Serializable]
    public class ResourceSaveItem
    {
        public int resourceTypeHash;
        public int count;
    }

    public class GlobalDataSaveManager : PlayerPrefsSaveManager<GlobalSave>
    {
        private const string PLAYER_SAVE_KEY = "PLAYER_SAVE_KEY";
        private PlayerResources m_Resource;
        private IResourceTypes m_gResources;

        [Inject]
        void Construct(PlayerResources pResources, IResourceTypes gResources)
        {
            m_Resource = pResources;
            m_gResources = gResources;
        }

        public override void LoadPass(GlobalSave loadData)
        {
            m_Resource.resources.Clear();

            foreach (var item in loadData.playerResources)
            {
                var resType = m_gResources.Types.FirstOrDefault(e => e.GetId() == item.resourceTypeHash);
                m_Resource.resources.SetResource(resType, item.count);
            }
        }

        public override void SavePass(GlobalSave saveData)
        {
            foreach (var item in m_Resource.resources.ResourceIterator())
            {
                saveData.playerResources.Add(new ResourceSaveItem
                {
                    count = item.Value,
                    resourceTypeHash = item.Key.GetId(),
                });
            }
        }

        private void Awake()
        {
            Load(PLAYER_SAVE_KEY);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Save(PLAYER_SAVE_KEY);
                Debug.Log("OnApplicationPause Global Save");
            }
        }

        private void OnApplicationQuit()
        {
            Save(PLAYER_SAVE_KEY);
            Debug.Log("OnApplicationQuit Global Save");
        }
    }
}