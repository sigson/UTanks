using SecuredSpace.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class TurretRotaion : MonoBehaviour
    {
        public float maxTorque = 40f; // maximum torque the motor can apply to wheel
        public float minTorque = 20f;
        public float SpeedUp = 0.7f;
        public float nowSpeed = 0f;
        public bool Movable = false;
        public TankManager parentTankManager;
        public AudioAnchor turretBaseSound;
        public float LeftTurn = 0f;
        public float RightTurn = 0f;

        private void FixedUpdate()
        {
            if (Movable && (ClientNetworkService.instance == null || ClientNetworkService.instance.PlayerEntityId == parentTankManager.ManagerEntityId))
            {
                LeftTurn = !ClientInitService.instance.LockInput ? Input.GetAxis("LeftTurret") : 0f;
                RightTurn = !ClientInitService.instance.LockInput ? Input.GetAxis("RightTurret") : 0f;
            }
            if (LeftTurn > 0f || RightTurn > 0f)
            {
                if (nowSpeed == 0)
                {
                    nowSpeed = minTorque;
                }
                else if (nowSpeed < maxTorque)
                {
                    nowSpeed += SpeedUp;
                }
                transform.Rotate(new Vector3(0f, nowSpeed * (LeftTurn * -1 + RightTurn), 0f) * Time.fixedDeltaTime);
                //transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0f, transform.rotation.eulerAngles.y + nowSpeed, 0f), Time.deltaTime);
            }
            else
            {
                nowSpeed = minTorque;
            }
            if(LeftTurn > 0f || RightTurn > 0f)
            {
                var nowPlayed = turretBaseSound.audioManager.GetNowPlayingAudioName();
                if (nowPlayed.Length == 0)
                {
                    turretBaseSound.audioManager.StopAll();
                    turretBaseSound.audioManager.ResetAll();
                    turretBaseSound.audioManager.PlayBlock(new List<string>() { "turret" });
                }
            }
            else
            {
                turretBaseSound.audioManager.FadeAll();
            }
        }

        public void ExternalManagement(float ExternalNowSpeed, float turnSum)
        {
            if(ClientNetworkService.instance.PlayerEntityId != parentTankManager.ManagerEntityId)
            {
                //transform.Rotate(new Vector3(0f, nowSpeed * turnSum, 0f) * Time.fixedDeltaTime);
                LeftTurn = (turnSum < 0 ? turnSum * -1 : 0);
                RightTurn = (turnSum > 0 ? turnSum : 0);
            }
        }
    }

}