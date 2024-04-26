using Prototype;
using System;
using UnityEngine;

namespace Prototype
{
    public class CarControl : MonoBehaviour
    {
        public float motorTorque = 2000;
        public float brakeTorque = 2000;
        public float maxSpeed = 20;
        public float steeringRange = 30;
        public float steeringRangeAtMaxSpeed = 10;
        public float centreOfGravityOffset = -1f;

        [Range(0, 1f)]
        public float vInput = 0;
        [Range(0, 1f)]
        public float hInput = 0;

        public bool blockWheels;
        WheelControl[] wheels;
        Rigidbody rigidBody;

        [SerializeField]
        private Collider m_FreezeCarCollider;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            // Find all child GameObjects that have the WheelControl script attached       
            wheels = GetComponentsInChildren<WheelControl>();
            // Adjust center of mass vertically, to help prevent the car from rolling
            rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

            Freeze(false);
        }

        public void Freeze(bool freeze)
        {
            rigidBody.isKinematic = freeze;
            if(m_FreezeCarCollider)
            m_FreezeCarCollider.enabled = freeze;
        }

        public float CurrentMotorTorque => currentMotorTorque;

        float currentMotorTorque;

        void Update()
        {
            // Calculate current speed in relation to the forward direction of the car
            // (this returns a negative number when traveling backwards)
            float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

            // Calculate how close the car is to top speed
            // as a number from zero to one
            float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

            // Use that to calculate how much torque is available 
            // (zero torque at top speed)
            currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

            // …and to calculate how much to steer 
            // (the car steers more gently at top speed)
            float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

            // Check whether the user input is in the same direction 
            // as the car's velocity
            bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

            foreach (var wheel in wheels)
            {
                // Apply steering to Wheel colliders that have "Steerable" enabled
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
                }

                if (blockWheels)
                {
                    // If the user is trying to go in the opposite direction
                    // apply brakes to all wheels
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(1f) * brakeTorque;
                    wheel.WheelCollider.motorTorque = 0;
                }
                else if (isAccelerating)
                {
                    // Apply torque to Wheel colliders that have "Motorized" enabled
                    if (wheel.motorized)
                    {
                        wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                    }
                    wheel.WheelCollider.brakeTorque = 0;
                }
                else
                {
                    // If the user is trying to go in the opposite direction
                    // apply brakes to all wheels
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                    wheel.WheelCollider.motorTorque = 0;
                }
            }
        }
    }
}