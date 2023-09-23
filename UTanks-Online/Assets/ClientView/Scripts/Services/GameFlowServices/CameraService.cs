using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.Services
{
    public class CameraService : IService
    {
        public static CameraService instance => IService.getInstance<CameraService>();
        public Camera nowMainCamera;

        public override void PostInitializeProcess()
        {
            //ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.BattleCamera);
            //ResourcesService.instance.GetPrefab(PrefabID.PrefabsID.LobbyCamera);
        }

        public override void InitializeProcess()
        {

        }

        public override void OnDestroyReaction()
        {

        }
    }
}