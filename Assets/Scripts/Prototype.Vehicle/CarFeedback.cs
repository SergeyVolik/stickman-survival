using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    public class CarFeedback : MonoBehaviour
    {
        public AudioSource engineSound;
        private CarControl m_Car;

        public float minPitch = 1;
        public float maxPitch = 2;

        public float engineTransitionSpeed = 1f;
        private void Awake()
        {
            m_Car = GetComponent<CarControl>();
        }
        private void Update()
        {
            var input = math.clamp(m_Car.vInput, 0, 1);
            engineSound.pitch = math.lerp(engineSound.pitch, math.remap(0, 1, minPitch, maxPitch, input), Time.deltaTime * engineTransitionSpeed);
        }
    }
}
