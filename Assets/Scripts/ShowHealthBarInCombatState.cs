using UnityEngine;

namespace Prototype
{
    public class ShowHealthBarInCombatState : MonoBehaviour
    {
        private void Awake()
        {
            var hb = GetComponent<CharacterHealthbar>();

            GetComponent<CharacterCombatState>().onCombatState += (value) => {
                hb.AlwaysVisible(value);
                hb.Show(value);
            };
        }
    }
}