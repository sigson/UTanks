using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Extensions;

namespace SecuredSpace.Effects
{
    public class CameraBillboard : MonoBehaviour
    {
        public bool freezeX = false;
        public bool freezeY = false;
        public bool freezeZ = false;

        public Vector3 eulerRotation;

        //public void Start()
        //{
        //    if(eulerRotation == Vector3.zero)
        //        eulerRotation = this.transform.eulerAngles;
        //}
        private void LateUpdate()
        {
            if (Lambda.TryExecute(() => ClientInitService.instance.LockInput = ClientInitService.instance.LockInput) && ClientInitService.instance != null)
            {
                transform.forward = -ClientInitService.instance.NowMainCamera.transform.forward;
            }
            else
            {
                transform.forward = -Camera.main.transform.forward;
            }
            if (freezeX)
                transform.rotation = Quaternion.Euler(eulerRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
            if (freezeY)
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eulerRotation.y, transform.eulerAngles.z);
            if (freezeZ)
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, eulerRotation.z);
        }
    }

}