using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Prototype
{
    public class CollectItemQuest : BaseQuest
    {
        public GameObject CollectableObjectInstance;

        bool finished = false;
        protected override void Awake()
        {
            base.Awake();
            CollectableObjectInstance.GetComponent<ICollectable>().onCollected += CollectItemQuest_onCollected;
        }
        public override Transform GetQuestTargetObject()
        {
            if(CollectableObjectInstance == null)
                return null;

            return CollectableObjectInstance.transform;
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
           yield return CollectableObjectInstance.transform;
        }
        private void CollectItemQuest_onCollected()
        {
            finished = true;
            UpdateQuest();
        }

        public override bool IsFinished()
        {
            return finished;
        }
    }
}
