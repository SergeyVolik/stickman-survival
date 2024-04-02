using Prototype;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGUnBehaviour : MonoBehaviour
{
    public Gun gun;
    public float shotInterval = 0.5f;
    private CustomCharacterController m_Controller;
    private CharacterAnimatorV2 m_CharacterAnimator;
    float t;
    private void Awake()
    {
        m_Controller = GetComponent<CustomCharacterController>();
        m_CharacterAnimator = GetComponentInChildren<CharacterAnimatorV2>();
        GetComponent<HealthData>().onDeath+=() => { enabled = false; };

        gun.owner = gameObject;
    }

    private void Update()
    {

        if (m_Controller.IsAiming)
        {
            t += Time.deltaTime;

            if (t > shotInterval)
            {
                t = 0;
                gun.Shot();
                m_CharacterAnimator.Shot();
            }
        }
        else {
            t = 0;
        }
    }
}
