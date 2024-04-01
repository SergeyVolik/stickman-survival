using UnityEngine;
using Zenject;

namespace Prototype
{
    public interface IInputReader
    {
        public Vector2 ReadMoveInput();
    }

    public class PlayerInputReader : IInputReader
    {
        public Joystick m_Stick;

        public PlayerInputReader(Joystick stick)
        {
            m_Stick = stick;
        }

        public Vector2 ReadMoveInput()
        {
            return m_Stick.Direction;
        }
    }

    public class GameInit : MonoInstaller
    {
        public GameObject PlayerPrefab;
        private PlayerSpawnFactory m_playerSpawnFactory;
        public Joystick Joystick;

        public override void InstallBindings()
        {
            var input = new PlayerInputReader(Joystick);
            m_playerSpawnFactory = new PlayerSpawnFactory(PlayerPrefab, Container);
            Container.Bind<PlayerSpawnFactory>().FromInstance(m_playerSpawnFactory);
            Container.Bind<PlayerInputReader>().FromInstance(input);
        }

        private void Awake()
        {
            m_playerSpawnFactory.SpawnAtPosition(new Vector3());
        }
    }
}
