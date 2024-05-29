using UnityEngine;

namespace Prototype
{
    public interface IPlayerInputReader
    {
        public Vector2 ReadMoveInput();
        public void Enable();
        public void Disable();
    }
}