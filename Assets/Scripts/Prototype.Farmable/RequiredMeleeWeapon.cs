using UnityEngine;

namespace Prototype
{
    public interface IRequiredMeleeWeapon
    {
        public WeaponType RequiredWeapon { get; }
    }

    public class RequiredMeleeWeapon : MonoBehaviour, IRequiredMeleeWeapon
    {
        [field: SerializeField]
        public WeaponType RequiredWeapon { get; private set; }
    }
}
