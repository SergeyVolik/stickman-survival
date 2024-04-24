using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public enum HatType
    {
        None,
        Cowboy,
        Miner,
        Magic,
        Police
    }

    [System.Serializable]
    public class HatItem
    {
        public HatType type;
        public GameObject hatPrefab;
    }

    public class HatFactory : MonoBehaviour
    {
        private Dictionary<HatType, HatItem> m_Database;
        [SerializeField]
        private HatItem[] m_HatDatabase;
        public Dictionary<HatType, HatItem> Database
        {
            get
            {
                SetupDatabase();
                return m_Database;
            }
        }

        private void SetupDatabase()
        {
            if (m_Database == null)
            {
                m_Database = new Dictionary<HatType, HatItem>();

                foreach (var item in m_HatDatabase)
                {
                    m_Database.Add(item.type, item);
                }
            }
        }


        public Hat SpawnHat(HatType type, Transform spawnPoint)
        {
            if (Database.TryGetValue(type, out var hatItem))
            {
                var hatInstance = GameObject.Instantiate(hatItem.hatPrefab, spawnPoint).GetComponent<Hat>();
                hatInstance.Setup();
                return hatInstance;
            }

            return null;
        }
    }
}
