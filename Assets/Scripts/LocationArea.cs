using UnityEngine;

namespace Prototype
{
    public class LocationArea : MonoBehaviour
    {
        public GameObject[] HideAreaObjects;

        private void Awake()
        {
            foreach (GameObject go in HideAreaObjects)
            {
                go.SetActive(true);
            }

        }
        public void Open()
        {
            foreach (GameObject go in HideAreaObjects)
            {
                go.SetActive(false);
            }
        }
    }
}
