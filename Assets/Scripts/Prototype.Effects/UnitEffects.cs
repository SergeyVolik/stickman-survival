using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class UnitEffects : MonoBehaviour
    {
        private List<Effect> effects = new List<Effect>();
        private List<Effect> toRemove= new List<Effect>();
        public Transform effectHolder;
        public void AddEffect(Effect effectPrefab)
        {
            var effect = GameObject.Instantiate(effectPrefab, effectHolder);
            effect.Setup(gameObject);
            effects.Add(effect);
        }

        private void Update()
        {
            var delta = Time.deltaTime;
            foreach (var effect in effects)
            {
                effect.currentTime += delta;

                if (effect.currentTime > effect.duration)
                {
                    toRemove.Add(effect);
                }
            }
            foreach (var effect in toRemove)
            {
                effect.ClearEffect();
                GameObject.Destroy(effect.gameObject);
                effects.Remove(effect);
            }
            toRemove.Clear();
        }
    }
}
