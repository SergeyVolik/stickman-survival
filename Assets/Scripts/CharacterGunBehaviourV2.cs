using System;
using UnityEngine;

namespace Prototype
{
    public class CharacterGunBehaviourV2 : MonoBehaviour
    {
        public Gun currentGun;
        private CustomCharacterController m_Controller;
        private CharacterWithGunAnimator m_CharacterAnimator;
        private float m_ShotT;
        public Transform rightHand;
        public event Action<Gun> onGunChanged = delegate { };
        public void SpawnGun(GameObject gunPrefab)
        {
            if (currentGun)
            {
                GameObject.Destroy(currentGun.gameObject);
            }
            currentGun = GameObject.Instantiate(gunPrefab, rightHand).GetComponent<Gun>();
            currentGun.owner = gameObject;
            enabled = true;
            onGunChanged.Invoke(currentGun);
        }

        private void Awake()
        {
            m_Controller = GetComponent<CustomCharacterController>();
            m_CharacterAnimator = GetComponentInChildren<CharacterWithGunAnimator>();
            GetComponent<HealthData>().onDeath += () => { enabled = false; };

            if (currentGun == null)
                enabled = false; 
        }


        private void Update()
        {
            if (m_Controller.HasTarget)
            {
                m_ShotT += Time.deltaTime;

                if (m_ShotT > currentGun.shotInterval)
                {
                    m_ShotT = 0;
                    currentGun.Shot();
                    m_CharacterAnimator.Shot();
                }
            }
            else
            {
                m_ShotT = 0;
            }
        }
    }
}
