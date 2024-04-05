using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class CombatStateActivator : MonoBehaviour
    {      
        private IPlayerFactory m_PlayerFactory;
        private CameraController m_camera;
        private CharacterState m_CurrentState;
        public float timeToIdle;
        public float noCombatT;
        private bool isInCombat = true;

        [SerializeField]
        private MMF_Player combatFeedback;
        [SerializeField]
        private MMF_Player idleFeedback;

        [Inject]
        void Construct(IPlayerFactory factory, CameraController camera)
        {
            m_PlayerFactory = factory;
            m_camera = camera;
            factory.onPlayerSpawned += Factory_onPlayerSpawned;
        }

        private void Factory_onPlayerSpawned(GameObject obj)
        {
            obj.GetComponent<CustomCharacterController>().onCharacterStateChanged += (state) =>
            {
                m_CurrentState = state;
            };
        }

        private void Awake()
        {
            ActivateIdle();
        }

        private void Update()
        {
            if(m_CurrentState == CharacterState.Aiming)
            {
                ActivateCombat();
                noCombatT = 0;
                return;
            }

            noCombatT += Time.deltaTime;

            if (isInCombat && noCombatT > timeToIdle)
            {
                ActivateIdle();
            }
        }

        private void ActivateCombat()
        {
            if (isInCombat)
                return;

            isInCombat = true;

            if (m_PlayerFactory.CurrentPlayerUnit)
            {
                m_PlayerFactory.CurrentPlayerUnit.GetComponent<AimCirclerBehaviour>().Show();
                m_PlayerFactory.CurrentPlayerUnit.GetComponent<CharacterInventory>().ActiveLastWeapon();
            }

            combatFeedback?.PlayFeedbacks();
            m_camera.ActivateCombatCamera();
        }

        private void ActivateIdle()
        {
            if (!isInCombat)
                return;

            isInCombat = false;

            if (m_PlayerFactory.CurrentPlayerUnit)
            {
                m_PlayerFactory.CurrentPlayerUnit.GetComponent<AimCirclerBehaviour>().Hide();
                m_PlayerFactory.CurrentPlayerUnit.GetComponent<CharacterInventory>().HideCurrentWeapon();
            }
            idleFeedback?.PlayFeedbacks();

            m_camera.ActivateIdleCamera();
        }
    }
}

