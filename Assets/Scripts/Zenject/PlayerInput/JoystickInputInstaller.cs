using Zenject;

namespace Prototype
{
    public class JoystickInputInstaller : MonoInstaller
    {
       
        public Joystick Joystick;

        public override void InstallBindings()
        {
            var input = new PlayerInputReader(Joystick);
            Container.Bind<IPlayerInputReader>().FromInstance(input);
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
