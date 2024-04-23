using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public struct StatData
    {
        public float currentValue;
        public float baseValue;
    }

    public class CharacterStats : MonoBehaviour
    {
       public float attackSpeedMult = 1f;
       public float meleeWeaponDamageMult = 1f;
       public float rangeWeaponDamageMult = 1f;
       public float critChance = 0.1f;
       public float critMult = 1.3f;
       public float moveSpeedMult = 1f;
    }
}
