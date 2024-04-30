using UnityEngine;

namespace Prototype
{
    public class DisableRegenInCombat : MonoBehaviour
    {
        private void Awake()
        {
            var regen = GetComponent<HealthRegen>();
            GetComponent<CharacterCombatState>().onCombatState += (value) => {
                regen.enabled = !value;
            };
        }
    }
}

