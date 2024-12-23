using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public interface IFinishObjectsQuest
    {
        public int CurrentValue();
        public int TargetValue();
    }

    public class DestroyObjectsQuest : BaseQuest, IFinishObjectsQuest
    {
        public HealthData[] objectsToDestory;
        private int killed;

        protected override void Awake()
        {
            base.Awake();

            foreach (var obj in objectsToDestory)
            {
                obj.onDeath += Obj_onDeath;
            }
        }

        private void Obj_onDeath()
        {
            killed++;
            UpdateQuest();
        }

        public int CurrentValue() => killed;
        public int TargetValue() => objectsToDestory.Length;

        public override Transform GetQuestTargetObject()
        {
            foreach (var obj in objectsToDestory)
            {
                if (!obj.IsDead)
                {
                    return obj.transform;
                }
            }
            return null;
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
            List<Transform> targets = new List<Transform>();

            foreach (var obj in objectsToDestory)
            {
                if (!obj.IsDead)
                {
                    targets.Add(obj.transform);
                }
            }

            return targets;
        }
        public override bool IsFinished()
        {
            return killed == objectsToDestory.Length;
        }
    }
}
