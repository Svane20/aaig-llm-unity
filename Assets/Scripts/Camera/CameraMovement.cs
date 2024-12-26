using System;
using UnityEngine;

namespace camera
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform target;
        public float smoothing;
        public Vector2 maxPosition;
        public Vector2 minPosition;

        private void LateUpdate()
        {
            if (transform.position != target.position)
            {
                var targetPosition = target.position;
                var transformPosition = transform.position;
                var targetVectorPosition = new Vector3(
                    x: targetPosition.x, 
                    y: targetPosition.y,
                    z: transformPosition.z);

                if (targetVectorPosition.y < -4 && targetVectorPosition.y > -15)
                {
                    targetVectorPosition.x = Mathf.Clamp(
                        value: targetPosition.x,
                        min: minPosition.x,
                        max: 69);
                }
                else
                {
                    targetVectorPosition.x = Mathf.Clamp(
                        value: targetPosition.x,
                        min: minPosition.x,
                        max: maxPosition.x);
                
                    targetVectorPosition.y = Mathf.Clamp(
                        value: targetPosition.y,
                        min: minPosition.y,
                        max: maxPosition.y);
                }
                
                

                transformPosition = Vector3.Lerp(
                    a: transformPosition,
                    b: targetVectorPosition,
                    t: smoothing * Time.deltaTime);
                transform.position = transformPosition;
            }
        }
    }
}