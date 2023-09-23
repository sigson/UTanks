using SecuredSpace.ClientControl.Managers;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Effects.Lighting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.Settings
{
    public class LightAnchor : IAnchor
    {
        public List<ILight> lights = new List<ILight>();
        public bool WeaponLight;
        public bool MapObjectLight;
        public bool GlobalMapLight;
        protected override void StartImpl()
        {
            base.StartImpl();
        }
        protected override IEnumerator UpdateObjectImpl()
        {
            //lights = lights.Where(x => x.gameObject != null).ToList();
            if (WeaponLight)
                lights.ForEach(x => {
                    objectEnabled = ClientInitService.instance.gameSettings.WeaponIllumination;
                    if (x.lightSwitchState != ClientInitService.instance.gameSettings.WeaponIllumination)
                        x.LightSwitch(ClientInitService.instance.gameSettings.WeaponIllumination);
                });
            if (MapObjectLight)
                lights.ForEach(x => {
                    objectEnabled = ClientInitService.instance.gameSettings.MapIllumination;
                    if (x.lightSwitchState != ClientInitService.instance.gameSettings.MapIllumination)
                        x.LightSwitch(ClientInitService.instance.gameSettings.MapIllumination);
                });
            if (GlobalMapLight)
                lights.ForEach(x => {
                    objectEnabled = ClientInitService.instance.gameSettings.GlobalIllumination;
                    if (x.lightSwitchState != ClientInitService.instance.gameSettings.GlobalIllumination)
                        x.LightSwitch(ClientInitService.instance.gameSettings.GlobalIllumination);
                    var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(this.ownerManagerSpace.ManagerEntityId);
                    var mapLoader = battleManager.MapManager;
                    x.transform.rotation = mapLoader.GlobalLightRotation;
                    x.lightColor = mapLoader.GlobalLightColor;
                    x.UpdateLight();
                });
            yield return new WaitForEndOfFrame();
        }
    }
}