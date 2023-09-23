using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using System;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.CameraControl
{
    public class CameraController : IManagable
    {
        // The position that that camera will be following.
        public Transform target;
        public Transform CameraObject;
        public GameObject RainGenerator;
        // The speed with which the camera will be following.           
        public float smoothing = 5f;
        public float rotSmoothing = 3f;
        public float MoveSpeed = 3f;
        public float maxDistance = 9f;
        public GameObject defaultPosition;
        int GroundLayer;
        public bool Follow;
        public bool SpectatorMode;
        private bool spectatorEnabled;

        // The initial offset from the target.
        Vector3 offset;

        void Start()
        {
            // Calculate the initial offset.
            var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(this.ownerManagerSpace.ManagerEntityId);

            offset = transform.position - target.position;
            GroundLayer = LayerMask.GetMask("Default");
            if(battleManager != null)
            {
                if (battleManager.MapManager.WeatherMode == "snowfall" || battleManager.MapManager.WeatherMode == "cloudinessnowfall")
                {
                    this.CameraObject.GetComponent<GlobalSnowEffect.GlobalSnow>().enabled = true;
                }
                if (battleManager.MapManager.WeatherMode == "rain")
                {
                    RainGenerator.SetActive(true);
                }
            }
        }

        void FixedUpdate()
        {
            if (Follow)
            {
                Vector3 targetCamPos = target.position + offset;
                //transform.position = target.position;



                transform.position = Vector3.Lerp(transform.position, target.position, smoothing * Time.deltaTime);
                //transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, target.rotation.eulerAngles, smoothing * Time.deltaTime));

                transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.LerpAngle(transform.rotation.eulerAngles.y, target.rotation.eulerAngles.y, rotSmoothing * Time.deltaTime), 0));
                //CameraObject.rotation = Quaternion.Euler(new Vector3(22, -180, 0));
                CameraObject.LookAt(target.transform);
                float LeftTurn = !ClientInitService.instance.LockInput ? Input.GetAxis("CameraPositionUp") : 0f;
                float RightTurn = !ClientInitService.instance.LockInput ? Input.GetAxis("CameraPositionDown") : 0f;


                float distanceOffset = 0.01f;
                RaycastHit hit;
                if (Physics.Linecast(target.position, defaultPosition.transform.position, out hit, GroundLayer, QueryTriggerInteraction.Ignore))
                {
                    float tempDistance = Vector3.Distance(target.position, hit.point);
                    float cameraDistance = Vector3.Distance(target.position, CameraObject.position);
                    Vector3 position = target.position - (CameraObject.rotation * Vector3.forward * (tempDistance - distanceOffset));
                    CameraObject.position = Vector3.Lerp(CameraObject.position, new Vector3(hit.point.x, hit.point.y + MoveSpeed * (LeftTurn * -1 + RightTurn), hit.point.z), smoothing * Time.deltaTime);
                }
                else
                {
                    CameraObject.position = Vector3.Lerp(CameraObject.position, new Vector3(defaultPosition.transform.position.x, defaultPosition.transform.position.y + MoveSpeed * (LeftTurn * -1 + RightTurn), defaultPosition.transform.position.z), smoothing * Time.deltaTime);
                }
                defaultPosition.transform.position = Vector3.Lerp(defaultPosition.transform.position, new Vector3(defaultPosition.transform.position.x, defaultPosition.transform.position.y + MoveSpeed * (LeftTurn * -1 + RightTurn), defaultPosition.transform.position.z), smoothing * Time.deltaTime);
                //if(Vector3.Distance(CameraObject.position, targetCamPos) > maxDistance)
                //{
                //    CameraObject.position = Vector3.Lerp(CameraObject.position, new Vector3(CameraObject.position.x, CameraObject.position.y + MoveSpeed * (LeftTurn * -1 + RightTurn) * Time.deltaTime, CameraObject.position.z), smoothing*4);
                //}
                //var Radius = Math.Sqrt(Math.Pow(transform.position.x - target.position.x, 2) + Math.Pow(transform.position.y - target.position.y, 2) + Math.Pow(transform.position.z - target.position.z, 2));
                //var CircleLenght = 2f * Mathf.PI * (float)Radius;
                //float k = (float)CircleLenght / 360;
                //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + (target.rotation.y + 180f) * k, transform.position.y, transform.position.z + (target.rotation.y + 180f) * k), smoothing * Time.deltaTime);s
            }
            if(SpectatorMode)
            {
                if(!spectatorEnabled)
                {
                    CameraObject.gameObject.GetComponent<CameraSpectator.Spectator>().enabled = true;
                    CameraObject.gameObject.GetComponent<CameraSpectator.MouseLook>().enabled = true;
                    spectatorEnabled = true;
                }
            }
            else
            {
                if (spectatorEnabled)
                {
                    CameraObject.gameObject.GetComponent<CameraSpectator.Spectator>().enabled = false;
                    CameraObject.gameObject.GetComponent<CameraSpectator.MouseLook>().enabled = false;
                    spectatorEnabled = false;
                }
            }
        }
    }
}