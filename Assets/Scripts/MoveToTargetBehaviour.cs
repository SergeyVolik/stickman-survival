using System;
using UnityEngine;

namespace Prototype
{
    public class MoveToTargetBehaviour : MonoBehaviour
    {
        private Transform m_Transform;
        private CustomCharacterController m_CharContr;

        public float distanceToEnd = 1f;

        public Transform target;
        public bool targetReached = false;
        public float currentDistance;
        private void Awake()
        {
            m_Transform = transform;
            m_CharContr = GetComponent<CustomCharacterController>();
        }

        private void Update()
        {
            if (target == null)
            {
                targetReached = true;
                enabled = false;
                return;
            }

            var targetPos = target.position;
            targetPos.y = 0;

            var selfPos = m_Transform.position;
            selfPos.y = 0;

            var vector = (targetPos - selfPos).normalized;
            m_CharContr.MoveInput = new Vector2(vector.x, vector.z);

            currentDistance = Vector3.Distance(targetPos, selfPos);

            if (currentDistance < distanceToEnd)
            {
                enabled = false;
                targetReached = true;
            }
        }

        internal void SetTarget(Transform newTarget)
        {
            targetReached = false;
            target = newTarget;
        }
    }
}