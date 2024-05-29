using UnityEngine;

namespace Prototype
{
    public class PlayerInputReader : IPlayerInputReader
    {
        private bool m_Enabled;
        public Joystick m_Stick;
        public void Enable()
        {
            m_Enabled = true;
        }
        public void Disable()
        {
            m_Enabled = false;
        }

        public PlayerInputReader(Joystick stick)
        {
            m_Enabled = true;
            m_Stick = stick;
        }

        public Vector2 ReadMoveInput()
        {
            return m_Enabled ? m_Stick.Direction : new Vector2();
        }
    }
}
