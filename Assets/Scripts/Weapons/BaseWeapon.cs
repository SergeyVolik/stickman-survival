using UnityEngine;

namespace Prototype
{
    public abstract class BaseWeapon : MonoBehaviour, IOwnable
    {
        public GameObject Owner => owner;
        public GameObject owner;

        public Vector3 handOffset;
        public Vector3 handRotation;

        public Vector3 hideOffset;
        public Vector3 hideRotation;

        public virtual void SetupInHands(Transform hand)
        {
            var trans = transform;
            trans.parent = hand;
            trans.localPosition = handOffset;
            trans.localRotation = Quaternion.Euler(handRotation);           
        }

        public void SetupInHidePoint(Transform hidePoint)
        {
            var trans = transform;
            trans.parent = hidePoint;
            trans.localPosition = hideOffset;
            trans.localRotation = Quaternion.Euler(hideRotation);
        }
    }
}