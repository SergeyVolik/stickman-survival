using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class CraftWeaponQuest : BaseQuest
    {
        public WeaponUpgradeStation upgradeStation;
        private bool finished;
        public int requiredWeaponLevel;

        protected override void Awake()
        {
            base.Awake();           
        }

        public override Transform GetQuestTargetObject()
        {
            return upgradeStation.transform;
        }

        public override void Setup(Transform questUISpawnPoint)
        {
            base.Setup(questUISpawnPoint);
            upgradeStation.onWeaponCrafted += UpgradeStation_onWeaponCrafted1;
            UpgradeStation_onWeaponCrafted1(upgradeStation.GetLastCraftedItem());
        }

        private void UpgradeStation_onWeaponCrafted1(WeaponLevelUpgrade obj)
        {
            if (obj != null && obj.weaponPrefab.GetComponent<MeleeWeapon>().level >= requiredWeaponLevel)
            {
                finished = true;
            }

            UpdateQuest();
        }

        public override void Clear()
        {
            upgradeStation.onWeaponCrafted -= UpgradeStation_onWeaponCrafted1;
            base.Clear();
        }
        public override IEnumerable<Transform> GetQuestTargetObjects()
        {
           yield return upgradeStation.transform;
        }


        public override bool IsFinished()
        {
            return finished;
        }
    }
}
