using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ragdoll : MMRagdoller
{
    public Rigidbody[] RagdollBodies { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();

        RagdollBodies = _rigidbodies.OfType<Rigidbody>().ToArray();
    }
}
