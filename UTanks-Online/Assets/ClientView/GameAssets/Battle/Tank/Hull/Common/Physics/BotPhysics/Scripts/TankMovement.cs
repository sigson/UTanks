using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public List<AxleInfo> axleInfos; // the information about each individual axle
        public float maxMotorTorque; // maximum torque the motor can apply to wheel
        public float maxSteeringAngle; // maximum steer angle the wheel can have
        public Rigidbody TankRigidbody;


        public float maxTorque = 40f; // maximum torque the motor can apply to wheel
        public float minTorque = 20f;
        public float SpeedUp = 0.7f;
        public float nowSpeed = 0f;

        public void FixedUpdate()
        {
            float motor = maxMotorTorque * Input.GetAxis("Vertical") * -1;
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

            if (Input.GetAxis("Horizontal") != 0f)
            {
                if (nowSpeed == 0)
                {
                    nowSpeed = minTorque;
                }
                else if (nowSpeed < maxTorque)
                {
                    nowSpeed += SpeedUp;
                }
                transform.Rotate(new Vector3(0f, nowSpeed * (Input.GetAxis("Horizontal")), 0f) * Time.fixedDeltaTime);
                //transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0f, transform.rotation.eulerAngles.y + nowSpeed, 0f), Time.deltaTime);
            }
            else
            {
                nowSpeed = minTorque;
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
            }
        }
    }

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; // is this wheel attached to motor?
        public bool steering; // does this wheel apply steer angle?
    }
}