using UnityEngine;
using System.Collections;
using System;
 
namespace SecuredSpace.CameraSpectator
{
    [AddComponentMenu("Camera-Control/Mouse Look")]
    [Serializable]
    public class MouseLook : MonoBehaviour
    {
        public float mouseSensitivity = 100.0f;
        public float clampAngle = 80.0f;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        void Start()
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }

        void Update()
        {
            float mouseX = !ClientInitService.instance.LockInput ? Input.GetAxis("Mouse X") * Input.GetAxis("Mouse Click") : 0f;
            float mouseY = !ClientInitService.instance.LockInput ? -Input.GetAxis("Mouse Y") * Input.GetAxis("Mouse Click") : 0f;

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            transform.rotation = localRotation;
        }
    }
}