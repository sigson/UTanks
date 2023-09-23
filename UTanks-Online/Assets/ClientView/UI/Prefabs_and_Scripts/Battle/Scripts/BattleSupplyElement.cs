using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Tank;
using SecuredSpace.UI.Special;
using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace SecuredSpace.UI.GameUI
{
    public class BattleSupplyElement : TimerElement
    {
        public Text supplyCount;
        public string SupplyConfigPath;
        public Image supplyEffectFadeRadialIndicator;
        public Image supplyUsingDelayFadeRadialIndicator;
        public int keyCode = 49;//1

        protected override void StartImpl()
        {
            base.StartImpl();
            supplyEffectFadeRadialIndicator.type = Image.Type.Filled;
            supplyEffectFadeRadialIndicator.fillClockwise = false;
            supplyEffectFadeRadialIndicator.fillMethod = Image.FillMethod.Radial360;
            //supplyEffectFadeRadialIndicator.sprite = SpriteExtensions.Duplicate(supplyEffectFadeRadialIndicator.sprite, SpriteExtensions.TransformType.Rotate180);
            //supplyEffectFadeRadialIndicator.rectTransform.rotation = Quaternion.Euler(0, 0, -180);
            supplyEffectFadeRadialIndicator.fillAmount = 1f;

            supplyUsingDelayFadeRadialIndicator.type = Image.Type.Filled;
            supplyUsingDelayFadeRadialIndicator.fillClockwise = false;
            supplyUsingDelayFadeRadialIndicator.fillMethod = Image.FillMethod.Radial360;
            //supplyUsingDelayFadeRadialIndicator.sprite = SpriteExtensions.Duplicate(supplyEffectFadeRadialIndicator.sprite, SpriteExtensions.TransformType.Rotate180);
            //supplyUsingDelayFadeRadialIndicator.rectTransform.rotation = Quaternion.Euler(0, 0, -180);
            supplyUsingDelayFadeRadialIndicator.fillAmount = 1f;

            onTimerElapsed = (timer) =>
            {
                supplyEffectFadeRadialIndicator.fillAmount = 1f;
                running = false;
            };
        }

        private int EnterKeyDown = 0;
        private int EnterKeyUp = 0;
        private long nextKeyResetTime;
        protected override void UpdateImpl()
        {
            base.UpdateImpl();
            if(running)
                supplyEffectFadeRadialIndicator.fillAmount = timeRemaining / timerTime;

            if (Input.GetKey((KeyCode)keyCode) && EnterKeyDown == 0)
            {
                var playerEntity = ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId];
                if (!EntityGroupManagersStorageService.instance.GetGroupManagerCacheClientFirst<BattleManager, ECSEntity>(playerEntity.instanceId).ContainsKey(ClientNetworkService.instance.PlayerEntityId))
                {
                    ULogger.Error("Error using supply");
                    return;
                }
                var tankManager = playerEntity.GetComponent<EntityManagersComponent>().Get<TankManager>();
                WorldPoint UsingPoint = new WorldPoint() { Position = new Vector3S(tankManager.Hull.transform.position), Rotation = new Vector3S(tankManager.Hull.transform.rotation.eulerAngles) };
                if(SupplyConfigPath == @"garage\supplies\mine")
                {
                    RaycastHit raycastHit;
                    if (Physics.Raycast(tankManager.Hull.transform.position, Vector3.down, out raycastHit, Const.MaxUnityFloatValue, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                    {
                        UsingPoint = new WorldPoint() { Position = new Vector3S(raycastHit.point), Rotation = new Vector3S(tankManager.Hull.transform.rotation.eulerAngles) };
                    }
                }
                var supplyUseEvent = new SupplyUsedEvent()
                {
                    supplyPath = SupplyConfigPath,
                    targetEntities = new List<long> { ClientNetworkService.instance.PlayerEntityId },
                    usingPoint = UsingPoint
                };
                ClientNetworkService.instance.Socket.emit(supplyUseEvent.PackToNetworkPacket());
            }

            if (Input.GetKeyDown((KeyCode)keyCode))//KeyCode.Alpha1//49
                EnterKeyDown = 1;
            if (Input.GetKeyUp((KeyCode)keyCode) && EnterKeyDown == 1)
                EnterKeyUp = 1;

            if ((EnterKeyDown == 1 && EnterKeyUp == 1) || DateTime.Now.Ticks >= nextKeyResetTime)
            {
                EnterKeyDown = 0;
                EnterKeyUp = 0;
                nextKeyResetTime = DateTime.Now.AddSeconds(0.25).Ticks;
            }
        }

        public void SetupTimer(float time)
        {
            timerTime = time;
            ResetTimer();
        }
        public void StopSupplyTimer()
        {
            onTimerElapsed(this);
        }
    }

}