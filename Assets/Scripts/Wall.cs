using UnityEngine;

namespace Prototype
{
    public class Wall : MonoBehaviour
    {
        public GameObject[] HideParts;

        public void Hide()
        {
            foreach (GameObject part in HideParts)
            {
                part.SetActive(false);
            }
        }

        public void Show()
        {
            foreach (GameObject part in HideParts)
            {
                part.SetActive(true);
            }
        }
    }
}
