using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class CompleteCombatQuest : BaseQuest, IFinishObjectsQuest
    {
        public LocationCombat combat;
 
        protected override void Awake()
        {
            base.Awake();

            combat.onEnemyKilled += OnEnemyKilled;
        }

        private void OnEnemyKilled()
        {
            UpdateQuest();
        }

        public override bool IsFinished()
        {
            return combat.TargetKills == combat.AlreadyKilled;
        }

        public int CurrentValue()
        {
            return combat.AlreadyKilled;
        }

        public int TargetValue()
        {
            return combat.TargetKills;
        }

        public override Transform GetQuestTargetObject()
        {
            return combat.GetAliveUnit();
        }

        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
            return combat.GetAllAliveUnits();
        }
    }
}
