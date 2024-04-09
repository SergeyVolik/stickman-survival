using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class Wall : MonoBehaviour
    {
        public GameObject[] HideParts;
        private Renderer[] m_WallRenderers;
        public List<Renderer> topDownRender = new List<Renderer>();
        private void Awake()
        {
           m_WallRenderers = GetComponentsInChildren<Renderer>();

            foreach (var item in HideParts)
            {
                topDownRender.AddRange(item.GetComponentsInChildren<Renderer>());
            }
        }

        public void EnableRender(bool enable)
        {
            foreach (Renderer r in m_WallRenderers)
            {
                r.enabled = enable;
            }
        }

        public void EnableTopDownRender(bool enable)
        {
            foreach (Renderer part in topDownRender)
            {
                part.enabled = !enable;
            }
        }
    }
}
