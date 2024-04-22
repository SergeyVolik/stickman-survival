using UnityEngine;

namespace Prototype
{
    public class SelectionTarget : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_Target;
        private void Awake()
        {
            m_Target.gameObject.SetActive(false);
        }
        public void Activate()
        {
            m_Target.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            m_Target.gameObject.SetActive(false);
        }

        public void SetColor(Color color)
        {
            m_Target.color = color;
        }
    }
}