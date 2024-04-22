using UnityEngine;

namespace Prototype
{
    public abstract class MonoState : MonoBehaviour, IState
    {
        public bool IsActive { get => enabled; set => enabled = value; }
    }
}