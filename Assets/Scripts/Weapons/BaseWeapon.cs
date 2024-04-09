using DG.Tweening;
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

        const float duration = 1f;

        const Ease showEase = Ease.OutCubic;
        const Ease hideEase = Ease.OutCubic;
        public virtual void SetupInHands(Transform hand)
        {
            var trans = transform;
            trans.parent = hand;
            //trans.localPosition = handOffset;
            //trans.localRotation = Quaternion.Euler(handRotation);

            trans.DOLocalMove(handOffset, duration).SetEase(showEase);
            trans.DOLocalRotateQuaternion(Quaternion.Euler(handRotation), duration).SetEase(showEase);
        }

        public void SetupInHidePoint(Transform hidePoint)
        {
            var trans = transform;
            trans.parent = hidePoint;

            trans.DOLocalMove(hideOffset, duration).SetEase(hideEase);
            trans.DOLocalRotateQuaternion(Quaternion.Euler(hideRotation), duration).SetEase(hideEase);
            //trans.localPosition = hideOffset;
            //trans.localRotation = Quaternion.Euler(hideRotation);
        }
    }
}