using System;
using UnityEngine;

namespace Prototype
{
    public interface IMeleeWeaponValidator
    {
        bool Validate(int level, MeleeWeaponType weaponType);
        public event Action onMeleeWeaponFailed;
    }
    public interface IRequiredMeleeWeapon : IMeleeWeaponValidator
    {
        public MeleeWeaponType RequiredWeapon { get; }
        public int RequiredWeaponLevel { get; }
    }

    public class RequiredMeleeWeapon : MonoBehaviour, IRequiredMeleeWeapon
    {
        [field: SerializeField]
        public MeleeWeaponType RequiredWeapon { get; private set; }

        [field: SerializeField]
        public int RequiredWeaponLevel { get; set; }

        public event Action onMeleeWeaponFailed;

        public bool Validate(int level, MeleeWeaponType weaponType)
        {
            return true;
        }
    }
}
