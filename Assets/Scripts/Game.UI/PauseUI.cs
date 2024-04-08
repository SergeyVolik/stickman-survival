using Prototype.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Prototype
{
    public class PauseUI : UIPage
    {
        [SerializeField]
        private Button continueButton;
        [SerializeField]
        private Button restartButton;

        public AudioMixer mixer;
        protected override void Awake()
        {
            base.Awake();

            continueButton.onClick.AddListener(() => {
                PopPage();
            });

            restartButton.onClick.AddListener(() => {
                SceneManager.LoadScene(0);
            });
        }
        public override void Show()
        {
            base.Show();

            mixer.SetFloat("MasterPitch", 0.9f);
            Time.timeScale = 0.0f;
        }

        public override void Hide(bool onlyDosableInput = false)
        {
            base.Hide(onlyDosableInput);
            mixer.SetFloat("MasterPitch", 1f);
            Time.timeScale = 1.0f;
        }
    }
}
