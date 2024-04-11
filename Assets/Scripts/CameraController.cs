using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

namespace Prototype
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform m_CameraTarget;
        public float lookAheadSpeed;
        public float lookAheadOffset;

        public float lookAheadSpeedIdle;
        public float lookAheadOffsetIdle;

        private float m_CurrentlookAheadSpeed;
        private float m_CurrentlookAheadOffset;

        public GameObject combatVCamera;
        public GameObject idleVCamera;

        private IPlayerFactory m_PlayerFactory;
        private GameObject m_CurrentCamera;
        private TweenerCore<Vector3, Vector3, VectorOptions> m_SetOffsetTween;

        [Inject]
        public void Construct(IPlayerFactory factory)
        {
            m_PlayerFactory = factory;
        }

        private void Awake()
        {
            ActivateIdleCamera();
        }

        private void Update()
        {
            if (m_PlayerFactory.CurrentPlayerUnit)
            {
                var uniTrans = m_PlayerFactory.CurrentPlayerUnit.transform;
                var unitPos = uniTrans.position;
                var unitForward = uniTrans.forward;
                var offset = unitForward * m_CurrentlookAheadOffset;

                m_CameraTarget.position = Vector3.Lerp(m_CameraTarget.position, unitPos + offset, Time.deltaTime * m_CurrentlookAheadSpeed);
            }
        }

        [Button]
        public void ActivateCombatCamera()
        {
            m_CurrentlookAheadSpeed = lookAheadSpeed;
            m_CurrentlookAheadOffset = lookAheadOffset;

            combatVCamera.SetActive(true);
            idleVCamera.SetActive(false);

            m_CurrentCamera = combatVCamera;
        }

        [Button]
        public void ActivateIdleCamera()
        {
            m_CurrentlookAheadSpeed = lookAheadSpeedIdle;
            m_CurrentlookAheadOffset = lookAheadOffsetIdle;

            combatVCamera.SetActive(false);
            idleVCamera.SetActive(true);
            m_CurrentCamera = idleVCamera;
        }

        public class ForceOffsetData
        {
            public GameObject source;
            public float offset;
        }
        public List<ForceOffsetData> forcedOffsets = new List<ForceOffsetData>();
        public void FourceOffset(GameObject source, float value)
        {
            forcedOffsets.Add(new ForceOffsetData { source = source, offset = value });

            SetXOffset(idleVCamera, value);
            SetXOffset(combatVCamera, value);
        }

        private void SetXOffset(GameObject cam, float value)
        {
            if (cam == null)
                return;
            var follow = cam.GetComponent<CinemachineFollow>();
            var followOffset = follow.FollowOffset;
            followOffset.x = value;

            m_SetOffsetTween = DOTween.To(() => follow.FollowOffset, (v) => follow.FollowOffset = v, followOffset, 1f).SetEase(Ease.OutSine);
        }

        internal void RemoveOffset(GameObject source)
        {
            forcedOffsets.RemoveAll((data) => data.source == source);

            float offset = 0;

            if (forcedOffsets.Count != 0)
            {
                offset = forcedOffsets[forcedOffsets.Count-1].offset;
            }

            SetXOffset(idleVCamera, offset);
            SetXOffset(combatVCamera, offset);
        }
    }
}