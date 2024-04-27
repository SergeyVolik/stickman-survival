using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class FinishRequiredResourceQuest : BaseQuest
    {
        public RequiredResourcesBehaviour resourcesToFind;
        public Transform highlighObject;

        public override void Setup(Transform questUISpawnPoint)
        {
            base.Setup(questUISpawnPoint);
            resourcesToFind.onFinished += UpdateQuest;
        }

        private void OnDestroy()
        {
            resourcesToFind.onFinished -= UpdateQuest;
        }

        public override void FinishQuest()
        {
            base.FinishQuest();        
        }

        public override Transform GetQuestTargetObject()
        {
            return highlighObject;
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
            yield return GetQuestTargetObject();
        }

        public override bool IsFinished()
        {
            return resourcesToFind.Finished();
        }
    }
}
