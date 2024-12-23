using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class DropExecutor : MonoBehaviour
    {
        public Vector3 dropVectorRotation = new Vector3(0, 0, 0);
        public ResourceContainer resources;
        private ResourceTransferManager m_movManager;
        public Transform dropPoint;
        private Transform RealDroppoint => dropPoint ? dropPoint : transform;
        public float pushForce = 7f;
        [Inject]
        void Construct(ResourceTransferManager movManager)
        {
            m_movManager = movManager;
        }

        public void ExecuteDrop(GameObject dropTarget)
        {
            DropHelper.TryDrop(dropTarget, RealDroppoint.position, resources, m_movManager, GetDropVector(), pushForce: pushForce);
        }

        public void ExecuteDrop(GameObject dropTarget, ResourceContainer resources)
        {
            if (dropTarget == null)
                return;

            DropHelper.TryDrop(dropTarget, RealDroppoint.position, resources, m_movManager, GetDropVector(), pushForce: pushForce);
        }

        private Vector3 GetDropVector() => Quaternion.Euler(dropVectorRotation.x, dropVectorRotation.y, dropVectorRotation.z) * transform.up;
        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(new Ray {  origin = RealDroppoint.position, direction = GetDropVector() });
        }
    }
}