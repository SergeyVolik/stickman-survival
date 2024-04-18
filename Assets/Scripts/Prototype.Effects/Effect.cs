using UnityEngine;

namespace Prototype
{
    public interface IEffectExtention
    {
        public void Setup(GameObject target);
        public void Clear();
    }

    public class Effect : MonoBehaviour
    {
        public float duration;
        public float currentTime;
        private IEffectExtention[] m_Extentions;

        private void Awake()
        {
            m_Extentions = GetComponents<IEffectExtention>();
        }

        public void Setup(GameObject owner)
        {
            foreach (var item in m_Extentions)
            {
                item.Setup(owner);
            }
        }

        public void ClearEffect()
        {
            foreach (var item in m_Extentions)
            {
                item.Clear();
            }
        }
    }
}
