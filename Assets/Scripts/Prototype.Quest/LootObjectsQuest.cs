using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class LootObjectsQuest : BaseQuest, IFinishObjectsQuest
    {
        public LootableObjectBehaviour[] objectsToLoot;
        private int looted;

        protected override void Awake()
        {
            base.Awake();

            foreach (var obj in objectsToLoot)
            {
                obj.onLooted.AddListener(OnLooted);
            }
        }

        private void OnLooted()
        {
            looted++;
            UpdateQuest();
        }

        public int CurrentValue() => looted;
        public int TargetValue() => objectsToLoot.Length;

        public override Transform GetQuestTargetObject()
        {
            foreach (var obj in objectsToLoot)
            {
                if (!obj.IsOpened)
                {
                    return obj.transform;
                }
            }
            return null;
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
            List<Transform> targets = new List<Transform>();

            foreach (var obj in objectsToLoot)
            {
                if (!obj.IsOpened)
                {
                    targets.Add(obj.transform);
                }
            }

            return targets;
        }
        public override bool IsFinished()
        {
            return looted == objectsToLoot.Length;
        }
    }
}
