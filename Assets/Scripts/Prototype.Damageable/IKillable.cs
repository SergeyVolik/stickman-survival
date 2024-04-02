using System;
using UnityEngine;

namespace Prototype
{
    public interface IKillable
    {
        public event Action onDeath;
        public GameObject KilledBy { get; }
        public void Kill();
        public bool IsDead { get; }
    }
}
