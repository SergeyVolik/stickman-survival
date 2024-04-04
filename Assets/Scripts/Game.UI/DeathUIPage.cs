using Prototype.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class DeathUIPage : UIPage
    {
        public Button deathButton;
        private IPlayerFactory m_playerSpawner;

        [Inject]
        void Construct(IPlayerFactory playerSpawner)
        {
            m_playerSpawner = playerSpawner;
            m_playerSpawner.onPlayerSpawned += M_playerSpawner_onPlayerSpawned;
        }

        private void M_playerSpawner_onPlayerSpawned(GameObject obj)
        {
            obj.GetComponent<HealthData>().onDeath += () => {
                Navigate();
            };
        }

        protected override void Awake()
        {
            base.Awake();

            deathButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}

