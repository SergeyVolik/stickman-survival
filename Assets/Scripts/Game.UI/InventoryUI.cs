using Prototype.UI;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class InventoryUI : UIPage
    {
        private IPlayerFactory m_factory;

        [Inject]
        void Construct(IPlayerFactory factory)
        {
            m_factory = factory;
        }

        public override void Show()
        {
            base.Show();

            if (m_factory.CurrentPlayerUnit)
            {
                m_factory.CurrentPlayerUnit.GetComponent<PlayerPreviewCamera>().Activate();
               
            }

            Time.timeScale = 0;
        }

        public override void Hide(bool onlyDosableInput = false)
        {
            base.Hide(onlyDosableInput);

            if (m_factory.CurrentPlayerUnit)
            {
                m_factory.CurrentPlayerUnit.GetComponent<PlayerPreviewCamera>().Dectivate();
            }

            Time.timeScale = 1;
        }
    }
}
