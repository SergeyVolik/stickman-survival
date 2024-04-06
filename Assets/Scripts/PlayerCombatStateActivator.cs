using DG.Tweening;
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
        private CharacterCombatState m_CombatState;

        [Inject]
        void Construct(IPlayerFactory factory, CameraController camera)
        {
            m_PlayerFactory = factory;
            m_camera = camera;
            factory.onPlayerSpawned += Factory_onPlayerSpawned;
        }

        private void Factory_onPlayerSpawned(GameObject obj)
        {
            m_CombatState = obj.GetComponent<CharacterCombatState>();
            m_CombatState.onCombatState += (value) =>
            {
                UpdateState(value);
            };

           
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
            DOVirtual.DelayedCall(0.1f, () =>
            {
                ActivateIdle();
                UpdateState(m_CombatState.InCombat);
            });         
        }

        private void ActivateCombat()
        {
            if (isInCombat)
                return;

            isInCombat = true;
            Debug.Log("Player Combat");
            combatFeedback?.PlayFeedbacks();
            m_camera.ActivateCombatCamera();
        }

        private void ActivateIdle()
        {
            if (!isInCombat)
                return;
            Debug.Log("Player Idle");

            isInCombat = false;

            idleFeedback?.PlayFeedbacks();

            m_camera.ActivateIdleCamera();
        }
    }
}

