using UnityEngine;

namespace Prototype
{
    public class DestroyObjectsQuest : BaseQuest
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

        public int AlreadyKiller() => killed;
        public int TargetKills() => objectsToDestory.Length;

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

        public override bool IsFinished()
        {
            return killed == objectsToDestory.Length;
        }
    }
}
