using MoreMountains.Tools;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public class Ragdoll : MMRagdoller
    {
        public Rigidbody[] RagdollBodies { get; private set; }

        public Rigidbody[] UpperParts { get; private set; }

        [field: SerializeField]
        public Rigidbody Head { get; private set; }
        [field: SerializeField]
        public Rigidbody LeftLeg { get; private set; }
        [field: SerializeField]
        public Rigidbody RightLeg { get; private set; }
        [field: SerializeField]
        public Rigidbody Chest { get; private set; }
        [field: SerializeField]
        public Rigidbody LeftArt { get; private set; }
        [field: SerializeField]
        public Rigidbody RightArt { get; private set; }

        public void ForceKinematic()
        {
            foreach (var item in RagdollBodies)
            {
                item.isKinematic = true;
            }
        }

        public Rigidbody GetRandomUpper()
        {
            return UpperParts[Random.Range(0, UpperParts.Length)];
        }

        protected override void Initialization()
        {
            base.Initialization();

            UpperParts = new Rigidbody[4];

            UpperParts[0] = Head;
            UpperParts[1] = Chest;
            UpperParts[2] = LeftArt;
            UpperParts[3] = RightArt;

            RagdollBodies = _rigidbodies.OfType<Rigidbody>().ToArray();
        }
    }
}
