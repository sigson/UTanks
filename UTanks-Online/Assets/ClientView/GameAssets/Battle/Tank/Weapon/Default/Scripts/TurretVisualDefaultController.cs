using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Important.Aim;
using SecuredSpace.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;
using UTanksClient.Services;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class TurretVisualDefaultController : ITurretVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            TurretVisualDefaultController.BuildVisualShared<TurretVisualDefaultController>(visualizableEquipment, eraItemCard, this);
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if(eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            TurretVisualDefaultController.BuildVisualShared<TurretVisualDefaultController>(visualizableEquipment, eraItemCard, this);
        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {

#if AggressiveLog
            List<(string, object)> variableLogger = new List<(string, object)>();
            //try
#endif
            {
                var selectedEquip = visualizableEquipment;
                var turretVisualConntroller = tankVisualController as ITurretVisualController;

                ////var materials = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin").GetChildFSObject("card").GetContent<ItemCard>();
                

                turretVisualConntroller.TurretResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].PathName + "\\" + selectedEquip.Turrets[0].Grade.ToString()).GetChildFSObject("card").GetContent<ItemCard>();

                turretVisualConntroller.ColormapResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Colormaps[0].PathName).FillChildContentToItem();

                turretVisualConntroller.skinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Turrets[0].Skins[0].SkinPathName);
                turretVisualConntroller.colormapSkinConfig = ConstantService.instance.GetByConfigPath(selectedEquip.Colormaps[0].PathName);

                if (!turretVisualConntroller.skinConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
                {
                    turretVisualConntroller.TurretSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].Skins[0].SkinPathName + "\\" + selectedEquip.Turrets[0].Grade.ToString()).FillChildContentToItem();
                }
                else
                {
                    turretVisualConntroller.TurretSkinResources = ResourcesService.instance.GameAssets.GetDirectory(selectedEquip.Turrets[0].Skins[0].SkinPathName).FillChildContentToItem();
                }
                turretVisualConntroller.TurretSkinResources += eraItemCard;


                for (int i = 1; i < turretVisualConntroller.transform.childCount; i++)
                    Destroy(turretVisualConntroller.transform.GetChild(i).gameObject);

                turretVisualConntroller.MuzzlePoints.Clear();
                #region buildVisualTank

                var playerTurretFullModel = turretVisualConntroller.TurretSkinResources.GetElement<GameObject>("model");
                var playerTurretModel = playerTurretFullModel.GetComponent<MeshFilter>().sharedMesh;

                turretVisualConntroller.GetOrAddComponent<MeshFilter>().mesh = Instantiate(playerTurretModel);


                turretVisualConntroller.normalMaterials.Add(MainTurretMaterial, Instantiate(turretVisualConntroller.TurretSkinResources.GetElement<Material>("Material")));

                turretVisualConntroller.transparentMaterials.Add(MainTurretTransparentMaterial, Instantiate(turretVisualConntroller.TurretSkinResources.GetElement<Material>("TransparentMaterial")));


                turretVisualConntroller.normalMaterials.Values.ForEach((Material material) => {
                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
                    material.SetTexture("_Details", turretVisualConntroller.TurretSkinResources.GetElement<Texture2D>("details"));
                    material.SetTexture("_Lightmap", turretVisualConntroller.TurretSkinResources.GetElement<Texture2D>("lightmap"));
                });
                turretVisualConntroller.transparentMaterials.Values.ForEach((Material material) => {
                    //material.SetTexture("_Colormap", ColormapResources.GetElement<Texture2D>("image"));
                    material.SetTexture("_Details", turretVisualConntroller.TurretSkinResources.GetElement<Texture2D>("details"));
                    material.SetTexture("_Lightmap", turretVisualConntroller.TurretSkinResources.GetElement<Texture2D>("lightmap"));
                });

                turretVisualConntroller.GetOrAddComponent<MeshRenderer>().material = turretVisualConntroller.normalMaterials[MainTurretMaterial];

                turretVisualConntroller.GetOrAddComponent<ColormapScript>().Setup(turretVisualConntroller.colormapSkinConfig, turretVisualConntroller.ColormapResources, true);

                if(!turretVisualConntroller.Preview)
                {
                    var turretResPrefab = playerTurretFullModel;
                    
                    //turretVisualConntroller.MuzzlePointExample = 
                    for (int i = 0; i < turretResPrefab.transform.childCount; i++)
                    {
                        if (turretResPrefab.transform.GetChild(i).name.Contains("muzzle"))
                        {
                            var muzzlePoint = Instantiate(turretVisualConntroller.MuzzlePointExample, turretVisualConntroller.MuzzlePointExample.transform.parent);
                            turretVisualConntroller.MuzzlePoints.Add(muzzlePoint);
                            turretVisualConntroller.MuzzlePoint = muzzlePoint;
                            turretVisualConntroller.MuzzlePoint.SetActive(true);
                            turretVisualConntroller.selectedMuzzlePoint = turretVisualConntroller.MuzzlePoints.Count - 1;
                            turretVisualConntroller.MuzzleCheckPoint = turretVisualConntroller.MuzzlePoint.transform.GetChild(0).gameObject;
                            turretVisualConntroller.MuzzlePoint.transform.localPosition = turretResPrefab.transform.GetChild(i).transform.localPosition;
                            turretVisualConntroller.closeDistanceAIM = turretVisualConntroller.MuzzlePoint.GetComponent<CloseDistanceAIM>();
                        }
                    }
                    turretVisualConntroller.turretManager = (turretVisualConntroller.TurretResources.GetElement<GameObject>("script").GetComponent(typeof(ITurret)) as ITurret).AppendManagerToObject(turretVisualConntroller.gameObject);
                    turretVisualConntroller.turretManager.parentTankManager = turretVisualConntroller.parentTankManager;
                    turretVisualConntroller.turretManager.weaponAudio = turretVisualConntroller.gameObject.AddComponent<AudioAnchor>();
                    turretVisualConntroller.turretManager.weaponAudio.GameplaySound = true;
                    turretVisualConntroller.ChildTemp.Add(turretVisualConntroller.turretManager.weaponAudio);
                    turretVisualConntroller.turretManager.weaponAudio.volume = 0f;
                    turretVisualConntroller.parentTankManager.turretVisualController = turretVisualConntroller;//i do not want fix all weapon managers links to tankmanager and i decided it was a good shit solution
                    turretVisualConntroller.turretManager.Initialize(turretVisualConntroller.parentTankManager, turretVisualConntroller.parentTankManager.ManagerEntity);
                    turretVisualConntroller.turretManager.weaponAudio.DirectRegisterObject();
                    turretVisualConntroller.turretManager.weaponAudio.UpdateObject();
                    //////                hullManager.Initialize(this, playerEntity);
                }


                #endregion
                //var turretScripts = Instantiate(turretVisualConntroller.TurretResources.GetElement<GameObject>("script"), turretVisualConntroller.transform);
            }
#if AggressiveLog
            //catch (Exception ex)
            //{
            //    string variableChecker = "";
            //    variableLogger.ForEach(x => variableChecker += x.Item1 + " = " + x.Item2 + "\n");
            //    ULogger.Error("RebuildTank\n" + ex.Message + "\n" + ex.StackTrace + "\n" + variableChecker);
            //}
#endif


            return (T)tankVisualController;
        }

        public override void SetupColormap(ItemCard colormapResource)
        {
            TurretVisualDefaultController.SetupColormapShared(this, colormapResource);
        }

        public static new void SetupColormapShared(ITurretVisualController turretVisualController, ItemCard colormapResource)
        {
            if (colormapResource == turretVisualController.DeadColormapResources)
            {
                var turretMaterials = turretVisualController.GetComponent<MeshRenderer>().materials;
                turretMaterials.ForEach((Material material) => {
                    material.SetTexture("_Colormap", colormapResource.GetElement<Texture2D>("image"));
                    material.SetTextureScale("_Colormap", turretVisualController.normalMaterials[MainTurretMaterial].GetTextureScale("_Colormap"));
                });
                turretVisualController.GetComponent<ColormapScript>().DisableColormap();
                return;
            }
            turretVisualController.GetComponent<ColormapScript>().Setup(turretVisualController.colormapSkinConfig, colormapResource);
        }

        protected override void OnRemoveController()
        {
            TurretVisualDefaultController.OnRemoveControllerShared(this);
        }

        public static new void OnRemoveControllerShared(ITurretVisualController turretVisualController)
        {
            turretVisualController.MuzzlePoints.ForEach(x => Destroy(x));
            Destroy(turretVisualController.MuzzlePointExample);
            Destroy(turretVisualController.turretManager);
        }

        public override void SetGhostMode(bool enabled)
        {
            TurretVisualDefaultController.SetGhostModeShared(this, enabled);
        }

        public static new void SetGhostModeShared(ITurretVisualController turretVisualController, bool enable)
        {
            turretVisualController.GetComponent<MeshRenderer>().materials.ForEach(x =>
            {
                if(enable)
                {
                    x.shader = turretVisualController.TurretSkinResources.GetElement<Shader>("TransparentShader");
                    x.SetFloat("_Opacity", 1f - (Convert.ToInt32(enable) * 0.5f));
                }
                else
                {
                    x.shader = turretVisualController.TurretSkinResources.GetElement<Shader>("Shader");
                }
            });
            turretVisualController.GetComponent<ColormapScript>().Setup(turretVisualController.colormapSkinConfig, turretVisualController.ColormapResources);
        }

        public override void SetTemperature(float temperature)
        {
            TurretVisualDefaultController.SetTemperatureShared(this, temperature);
        }

        public static new void SetTemperatureShared(ITurretVisualController turretVisualController, float temperature)
        {
            turretVisualController.normalMaterials.ForEach(x =>
            {
                x.Value.SetFloat("_Temperature", temperature);
            });
        }
    }
}