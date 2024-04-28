using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class FindResourcesQuest : BaseQuest
    {
        public GameObject requiredResourcesObject;
        private PlayerResources m_resoruces;
        private IRequiredResourceContainer m_requiredResources;

        [Inject]
        void Construct(PlayerResources resoruces)
        {
            m_resoruces = resoruces;      
        }

        public override void Setup(Transform questUISpawnPoint)
        {
            base.Setup(questUISpawnPoint);

            m_requiredResources = requiredResourcesObject.GetComponent<IRequiredResourceContainer>();
            m_resoruces.resources.onResourceChanged += Resources_onResourceChanged;
            m_requiredResources.RequiredResources.onResourceChanged += RequiredResources_onResourceChanged;

            var findResUI = m_CurrentQuestUI.GetComponent<FindResourceQuestUI>();
            findResUI.Construct(m_resoruces, m_requiredResources);
        }

        private void RequiredResources_onResourceChanged(ResourceTypeSO arg1, int arg2)
        {
            UpdateQuest();
        }

        private void OnDestroy()
        {
            if(m_resoruces != null)
            m_resoruces.resources.onResourceChanged -= Resources_onResourceChanged;
        }

        public override void FinishQuest()
        {
            base.FinishQuest();
            m_requiredResources.RequiredResources.onResourceChanged -= RequiredResources_onResourceChanged;
            m_resoruces.resources.onResourceChanged -= Resources_onResourceChanged;          
        }

        private void Resources_onResourceChanged(ResourceTypeSO arg1, int arg2)
        {
            UpdateQuest();
        }


        public override bool IsFinished()
        {
            if (m_requiredResources == null)
                return false;
         
            foreach (var item in m_requiredResources.RequiredResources.ResourceIterator())
            {
                var playerResource = m_resoruces.resources.GetResource(item.Key);
                if (playerResource < item.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
