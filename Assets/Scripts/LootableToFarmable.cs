using DG.Tweening;
using UnityEngine;

namespace Prototype
{
    public class LootableToFarmable : MonoBehaviour
    {
        public float swithDelay = 1f;

        [SerializeField]
        private FarmableObject m_Farmable;
        [SerializeField]
        private LootableObjectBehaviour m_Lootable;

        private void Awake()
        {
            m_Lootable.onLooted.AddListener(() => {
                DOVirtual.DelayedCall(swithDelay, () =>
                {
                    if (m_Lootable)
                    {
                        m_Lootable.GetComponent<Collider>().enabled = false;
                        m_Farmable.gameObject.SetActive(true);
                    }
                });               
            });
        }
    }
}
