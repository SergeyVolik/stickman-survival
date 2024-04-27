using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class FindResourcesQuest : BaseQuest
    {
        public ResourceTypeSO resourcesToFind;
        public int toFind;
        private PlayerResources m_resoruces;

        [Inject]
        void Construct(PlayerResources resoruces)
        {
            m_resoruces = resoruces;      
        }

        public override void Setup(Transform questUISpawnPoint)
        {
            base.Setup(questUISpawnPoint);
            m_resoruces.resources.onResourceChanged += Resources_onResourceChanged;
            var findResUI = m_CurrentQuestUI.GetComponent<FindResourceQuestUI>();
            findResUI.Construct(m_resoruces);
        }

        private void OnDestroy()
        {
            if(m_resoruces != null)
            m_resoruces.resources.onResourceChanged -= Resources_onResourceChanged;
        }

        public override void FinishQuest()
        {
            base.FinishQuest();
            m_resoruces.resources.onResourceChanged -= Resources_onResourceChanged;          
        }

        private void Resources_onResourceChanged(ResourceTypeSO arg1, int arg2)
        {
            UpdateQuest();
        }

        public override bool IsFinished()
        {
            return m_resoruces.resources.GetResource(resourcesToFind) >= toFind;
        }
    }
}
