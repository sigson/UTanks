using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.Effects.Lighting
{
    public class GlobalDirectionalLight : ILight
    {
        private Light directLight = null;

        private Light getDirectLight()
        {
            if (directLight == null)
                directLight = this.GetComponent<Light>();
            return directLight;
        }

        protected override void OnEnableImpl()
        {
            base.OnEnableImpl();
            lightColor = getDirectLight().color;
            //cookie = getDirectLight().cookie;
        }

        public override void UpdateLight()
        {
            base.UpdateLight();
            if (ClientInitService.instance.gameSettings.EnableShadow)
                getDirectLight().shadows = LightShadows.Hard;
            else
                getDirectLight().shadows = LightShadows.None;
            if(ClientInitService.instance.gameSettings.LightShadows)
                getDirectLight().shadows = LightShadows.Hard;
            if (ClientInitService.instance.gameSettings.DeepShadows)
                getDirectLight().shadows = LightShadows.Soft;

            getDirectLight().color = lightColor;

            var battleManager = EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(this.ownerManagerSpace.ManagerEntityId);

            if (!(battleManager.MapManager.WeatherMode == "cloudiness" || battleManager.MapManager.WeatherMode == "cloudinessnowfall" || battleManager.MapManager.WeatherMode == "rain"))
            {
                getDirectLight().cookie = null;
            }
            else
            {
                getDirectLight().cookie = cookie;
            }
            //getDirectLight().range = lightSize;
            //getDirectLight().intensity = lightIntencity;
        }

        protected override void OnDisableImpl()
        {
            base.OnDisableImpl();
        }

        public override void LightSwitch(bool switchValue, bool nestedUpdate = false)
        {
            getDirectLight().enabled = switchValue;
            if (!nestedUpdate)
                base.LightSwitch(switchValue);
        }
    }
}