using UnityEngine;

namespace Prototype
{
    public interface IDamageable
    {
        public bool IsDamageable { get; }
        public void DoDamage(int damage, GameObject source);
    }
}
