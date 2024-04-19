using System;
using UnityEngine;

namespace Prototype
{
    public class FindResourcesQuest : BaseQuest
    {
        public ResourceTypeSO resourcesToFind;
        public int toFind;

        public override void FinishQuest()
        {
            
        }

        public override bool IsFinished()
        {
            throw new NotImplementedException();
        }
    }
}
