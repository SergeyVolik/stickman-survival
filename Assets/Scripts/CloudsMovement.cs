using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [Range(0, 1f)]
    public float speedRation;

    public Transform[] items;
    public Vector3 moveVector;
    public float teleportOffset;
}

namespace Prototype
{
    public class CloudsMovement : MonoBehaviour
    {
        public float speed;

        public ParallaxLayer[] layers;

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            var moveSpeed = speed;
            foreach (var layer in layers)
            {
                var speedRation = layer.speedRation;
                var moveVector = layer.moveVector;

                foreach (var item in layer.items)
                {
                    item.transform.position += moveVector * layer.speedRation * moveSpeed * deltaTime;

                    if (item.transform.position.x > layer.teleportOffset)
                    {
                        item.transform.position -= new Vector3(layer.teleportOffset, 0, 0);
                    }
                }
            }
        }
    }
}
