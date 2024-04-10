using UnityEngine;

namespace Prototype
{
    public interface IRequiredMeleeWeapon
    {
        public MeleeWeaponType RequiredWeapon { get; }
    }

    public class RequiredMeleeWeapon : MonoBehaviour, IRequiredMeleeWeapon
    {
        [field: SerializeField]
        public MeleeWeaponType RequiredWeapon { get; private set; }
    }
}
