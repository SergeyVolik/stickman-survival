using UnityEngine;
using Zenject;

namespace Prototype
{
    public class HatSetup : MonoBehaviour
    {
        [SerializeField]
        private Transform m_HatSpawnPoint;
        private Hat m_CurrentHat;

        public HatType hatType;
        private HatFactory m_hatFactory;

        [Inject]
        void Construct(HatFactory hatFactory)
        {
            m_hatFactory = hatFactory;
        }

        private void Awake()
        {
            SpawnHat(hatType);
        }

        public Hat CurrentHat => m_CurrentHat;

        private void OnDisable()
        {
            if (m_CurrentHat)
            {
                m_CurrentHat.gameObject.SetActive(false);
            }
        }
        private void OnEnable()
        {
            if (m_CurrentHat)
            {
                m_CurrentHat.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (m_CurrentHat)
            {
                GameObject.Destroy(m_CurrentHat);
            }
        }

        public void SpawnHat(HatType hatType)
        {
            if (m_CurrentHat)
            {
                GameObject.Destroy(m_CurrentHat.gameObject);
            }

            m_CurrentHat = m_hatFactory.SpawnHat(hatType, m_HatSpawnPoint);
        }

        public void ClearHat()
        {
            if(m_CurrentHat)
                GameObject.Destroy(m_CurrentHat.gameObject);
        }
    }
}

