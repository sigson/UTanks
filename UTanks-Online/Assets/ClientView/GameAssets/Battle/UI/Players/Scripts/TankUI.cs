using Assets.ClientCore.CoreImpl.ECS.Components.Battle.CharacteristicTransformers;
using SecuredSpace.Battle.Tank;
using SecuredSpace.CameraControl;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.Special;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Health;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.Extensions;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class TankUI : MonoBehaviour
    {
        public GameObject UISwitch;
        public GameObject TankInfoBlock;
        public GameObject UIRank;
        public GameObject UIUsername;
        public GameObject UIHealth;
        public GameObject UIWeaponEnergy;
        public GameObject UIActiveSupplies;
        public GameObject UIArmorSupply;
        public GameObject UIDamageSupply;
        public GameObject UISpeedSupply;
        public GameObject UIRepairSupply;
        public GameObject UIDamageElementExample;
        public TankManager parentTankManager;
        public RectTransform parentCanvas;
        public float positionYOffset = 1.5f;
        [SerializeField]
        private float positionYOffsetClientPlayer = 0.2f;
        [SerializeField]
        private float positionYOffsetPlayer = 1.8f;
        [SerializeField]
        private float maxEnemyDistanceVisibility = 50f;

        private Plane[] cameraPlanes;
        private Collider objCollider;
        private int playerToClientPlayerState = -1;//1 - ally, 0 - enemy

        private Camera mainCamera;
        private CameraController manCameraController;
        public void Update()
        {
            if (parentTankManager != null)
            {
                if (objCollider == null)
                {
                    objCollider = parentTankManager.Hull.GetComponent<Collider>();
                }
                if(playerToClientPlayerState == -1)
                {
                    try
                    {
                        if (parentTankManager.ManagerEntityId != ClientNetworkService.instance.PlayerEntityId && ((parentTankManager.playerCommandEntityId != ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId].GetComponent<EntityManagersComponent>().Get<TankManager>().playerCommandEntityId && parentTankManager.TeamColor != "dm") || parentTankManager.TeamColor == "dm"))
                        {
                            playerToClientPlayerState = 0;
                        }
                        else
                        {
                            playerToClientPlayerState = 1;
                        }
                    }
                    catch(Exception ex)
                    {
                        ULogger.Error("TankUI_Update " + ex.Message);
                    }
                }
                
                
                if (mainCamera == null)
                {
                    mainCamera = ClientInitService.instance.NowMainCamera.GetComponent<Camera>();
                    manCameraController = ClientInitService.instance.NowMainCamera.transform.parent.GetComponent<CameraController>();
                }

                if (playerToClientPlayerState == 0)
                {
                    RaycastHit hitInfo;
                    if (Vector3.Distance(parentTankManager.Turret.transform.position, mainCamera.transform.position) > maxEnemyDistanceVisibility)
                    {
                        this.UISwitch.SetActive(false);
                        return;
                    }
                    if(Physics.Raycast(mainCamera.transform.position, (parentTankManager.Hull.transform.position - mainCamera.transform.position).normalized, out hitInfo, maxEnemyDistanceVisibility, LayerMask.GetMask("Default", "TankBounds", "Friction", "Tank"), QueryTriggerInteraction.Ignore))
                    {
                        if(hitInfo.collider.tag != "Tank")
                        {
                            this.UISwitch.SetActive(false);
                            return;
                        }
                    }
                    this.UISwitch.SetActive(true);
                }
                else if(manCameraController.SpectatorMode)
                {
                    this.UISwitch.SetActive(false);
                    return;
                }
                else
                {
                    if(!this.UISwitch.activeInHierarchy)
                        this.UISwitch.SetActive(true);
                }
                    
                cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
                if (GeometryUtility.TestPlanesAABB(cameraPlanes, objCollider.bounds))
                {
                    this.UISwitch.SetActive(true);
                }
                else
                {
                    this.UISwitch.SetActive(false);
                    return;
                }

                float offsetPosY = parentTankManager.Turret.transform.position.y + positionYOffset;
                if (parentCanvas == null)
                    parentCanvas = UIService.instance.BattleUI.GetComponent<RectTransform>();
                // Final position of marker above GO in world space
                Vector3 offsetPos = new Vector3(parentTankManager.Turret.transform.position.x, offsetPosY, parentTankManager.Turret.transform.position.z);

                // Calculate *screen* position (note, not a canvas/recttransform position)
                Vector2 canvasPos;
                Vector2 screenPoint = mainCamera.WorldToScreenPoint(offsetPos);

                // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas, screenPoint, null, out canvasPos);

                // Set
                this.GetComponent<RectTransform>().localPosition = canvasPos;
            }
        }

        public void UpdateInfo(ECSEntity tankEntity, UserRankComponent userRankComponent)
        {
            UIRank.GetComponent<Image>().sprite = RankService.instance.GetRank(userRankComponent.Rank).miniSprite;
        }

        public void UpdateInfo(ECSEntity tankEntity, UsernameComponent usernameComponent)
        {
            bool lastState = UIUsername.activeInHierarchy;
            if (!ClientInitService.instance.CheckEntityIsPlayer(tankEntity))
            {
                UIUsername.SetActive(true);
                UIRank.SetActive(true);
                positionYOffset = positionYOffsetPlayer;
            }
            else
            {
                positionYOffset = positionYOffsetClientPlayer;
                UIUsername.SetActive(false);
                UIRank.SetActive(false);
            }
            UIUsername.GetComponent<Text>().text = usernameComponent.Username;
            if (lastState != UIUsername.activeInHierarchy)
                TankInfoBlock.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }

        public void UpdateInfo(ECSEntity tankEntity, HealthComponent healthComponent)
        {
            bool lastState = UIHealth.activeInHierarchy;
            UIHealth.SetActive(true);
            UIHealth.GetComponent<Slider>().value = (healthComponent.CurrentHealth / healthComponent.MaxHealth);
            if (lastState != UIHealth.activeInHierarchy)
                TankInfoBlock.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }

        public void UpdateInfo(ECSEntity tankEntity, WeaponEnergyComponent weaponEnergyComponent)
        {
            bool lastState = UIWeaponEnergy.activeInHierarchy;
            UIWeaponEnergy.SetActive(true);
            UIWeaponEnergy.GetComponent<Slider>().value = (weaponEnergyComponent.Energy / weaponEnergyComponent.MaxEnergy);
            if (lastState != UIWeaponEnergy.activeInHierarchy)
                TankInfoBlock.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }

        public void UpdateSupplyInfo(ECSEntity tankEntity)
        {
            bool EnabledBlock = false;
            if (tankEntity.HasComponent(ArmorTransformerComponent.Id))
            { 
                UIArmorSupply.SetActive(true);
                EnabledBlock = true;
            }
            else
            {
                UIArmorSupply.SetActive(false);
            }
            if (tankEntity.HasComponent(DamageTransformerComponent.Id))
            {
                UIDamageSupply.SetActive(true);
                EnabledBlock = true;
            }
            else
            {
                UIDamageSupply.SetActive(false);
            }
            if (tankEntity.HasComponent(NitroTransformerComponent.Id))
            {
                UISpeedSupply.SetActive(true);
                EnabledBlock = true;
            }
            else
            {
                UISpeedSupply.SetActive(false);
            }
            if (tankEntity.HasComponent(RepairTransformerComponent.Id))
            {
                UIRepairSupply.SetActive(true);
                EnabledBlock = true;
            }
            else
            {
                UIRepairSupply.SetActive(false);
            }
            if(EnabledBlock)
            {
                UIActiveSupplies.SetActive(true);
            }
            else
            {
                UIActiveSupplies.SetActive(false);
            }
            UIActiveSupplies.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }

        public void UpdateInfo(Assets.ClientCore.CoreImpl.ECS.Types.Battle.Command command, Assets.ClientCore.CoreImpl.ECS.Types.Battle.CommandPlayers commandPlayer)
        {

        }

        public void UpdateInfo(Assets.ClientCore.CoreImpl.ECS.Types.Battle.CommandPlayers commandPlayer)
        {
            //UIHealth.SetActive(true);
            //UIWeaponEnergy.SetActive(true);
            bool lastState = UIUsername.activeInHierarchy;
            if (!ClientInitService.instance.CheckEntityIsPlayer(commandPlayer.EntityId))
            {
                UIUsername.SetActive(true);
                UIRank.SetActive(true);
                positionYOffset = positionYOffsetPlayer;
            }
            else
            {
                positionYOffset = positionYOffsetClientPlayer;
                UIUsername.SetActive(false);
                UIRank.SetActive(false);
            }
            UIRank.GetComponent<Image>().sprite = RankService.instance.GetRank(commandPlayer.Rank).miniSprite;
            UIUsername.GetComponent<Text>().text = commandPlayer.Username;

            UIUsername.GetComponent<Text>().color = parentTankManager.NicknameColor;
            var counterOffset = TeamColors.elementColorOffsets["healthBackground"];
            UIHealth.transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(parentTankManager.NicknameColor, ColorEx.ToColor(counterOffset.Item1), counterOffset.Item2);
            UIHealth.GetComponent<Slider>().fillRect.GetComponent<Image>().color = parentTankManager.NicknameColor;

            if (lastState != UIUsername.activeInHierarchy)
                TankInfoBlock.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }
        //public void UpdateInfo(ECSEntity tankEntity, WeaponEnergyComponent usernameComponent)
        //{

        //}
        public void ShowDamage(float damage)
        {
            Color damageColor = new Color(255, 0, 0);
            Color healColor = new Color(0, 255, 0);
            var damageElement = Instantiate(UIDamageElementExample, UIDamageElementExample.transform.parent);
            damageElement.GetComponent<Text>().text = Mathf.Abs(damage).ToString();
            damageElement.GetComponent<Text>().color = damage > 0 ? damageColor : healColor;
            damageElement.SetActive(true);
        }
    }
}
