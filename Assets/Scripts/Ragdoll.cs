using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
