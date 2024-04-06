using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class PlayerCombatStateActivator : MonoBehaviour
    {      
        private IPlayerFactory m_PlayerFactory;
        private CameraController m_camera;
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
            var state = obj.GetComponent<CharacterCombatState>();
            state.onCombatState += (value) =>
            {
                UpdateState(value);
            };

            UpdateState(state.InCombat);
        }

        private void UpdateState(bool value)
        {
            if (value)
            {
                ActivateCombat();
            }
            else
            {
                ActivateIdle();
            }
        }

        private void OnEnable()
        {
            ActivateIdle();
        }

        private void ActivateCombat()
        {
            if (isInCombat)
                return;

            isInCombat = true;

            combatFeedback?.PlayFeedbacks();
            m_camera.ActivateCombatCamera();
        }

        private void ActivateIdle()
        {
            if (!isInCombat)
                return;

            isInCombat = false;

            idleFeedback?.PlayFeedbacks();

            m_camera.ActivateIdleCamera();
        }
    }
}

