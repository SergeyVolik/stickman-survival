using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class DropExecutor : MonoBehaviour
    {
        public ResourceContainer resources;
        private TransferMoveManager m_movManager;
        public Transform dropPoint;
        private Transform RealDroppoint => dropPoint ? dropPoint : transform;

        [Inject]
        void Construct(TransferMoveManager movManager)
        {
            m_movManager = movManager;
        }

        public void ExecuteDrop(GameObject dropTarget)
        {
            DropHelper.TryDrop(dropTarget, RealDroppoint.position, resources, m_movManager);
        }
    }
}