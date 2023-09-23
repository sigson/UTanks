using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle.Tank;
using SecuredSpace.UI.Special;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient.ClassExtensions;
using UTanksClient.ECS.Components.User;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class GarageTankPreviewHandler : MonoBehaviour
    {
        [SerializeField] private RawImage TankRenderer;
        [SerializeField] private CheckMouseOverUI TankRendererCheckOver;
        [SerializeField] private Texture renderTexture;
        [SerializeField] private Rigidbody CameraAxe;
        [SerializeField] private GameObject HullPreviewObject;
        [SerializeField] private GameObject TurretPreviewObject;
        [SerializeField] private GameObject TankPreviewObject;

        public float MouseRotationSpeed = 0.5f;
        public float MaxRotationSpeed = 0.25f;
        public float MouseRotationInertia = 0.01f;
        public float SecAwaitAfterRotation = 5f;

        private bool InAwait = false;
        private bool InPreviewRotation = false;
        void Update()
        {
            PreviewCameraControl();
            StaticRotation();
        }

        private void StaticRotation()
        {
            if(!InPreviewRotation && !InAwait)
            {
                if(CameraAxe.angularVelocity.y <= Mathf.Abs(MaxRotationSpeed))
                {
                    CameraAxe.AddRelativeTorque(new Vector3(0, MaxRotationSpeed, 0));
                }
            }
        }

        [SerializeField] private float cacheSpeed = 0;
        private void PreviewCameraControl()
        {
            var clickInput = Input.GetAxis("Mouse Click");
            if (!InPreviewRotation && clickInput > 0 && TankRendererCheckOver.IsPointerOverUIElement())
            {
                var speed = (Input.GetAxis("Mouse X") * MouseRotationSpeed);
                CameraAxe.isKinematic = true;
                CameraAxe.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
                if (speed == 0f)
                    cacheSpeed = cacheSpeed > 0 ? 0.01f : -0.01f;
                else
                    cacheSpeed = speed;
                InPreviewRotation = true;
                StopAllCoroutines();
            }
            else if (clickInput > 0 && InPreviewRotation)
            {
                var speed = (Input.GetAxis("Mouse X") * MouseRotationSpeed);
                CameraAxe.transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
                if (speed == 0f)
                    cacheSpeed = cacheSpeed > 0 ? 0.01f : -0.01f;
                else
                    cacheSpeed = speed;
                StopAllCoroutines();
            }
            else if(InPreviewRotation)
            {
                InPreviewRotation = false;
                MaxRotationSpeed = cacheSpeed > 0 ? Mathf.Abs(MaxRotationSpeed) : Mathf.Abs(MaxRotationSpeed) * -1;
                CameraAxe.isKinematic = false;
                InAwait = true;
                StartCoroutine(AwaitAfterRotation());
            }
            else
            {
                cacheSpeed = cacheSpeed - cacheSpeed * MouseRotationInertia;
                CameraAxe.transform.Rotate(0, (cacheSpeed * Time.deltaTime), 0, Space.Self);
            }
        }

        IEnumerator AwaitAfterRotation()
        {
            yield return new WaitForSeconds(SecAwaitAfterRotation);
            InAwait = false;
        }

        public void UpdateTankPreview(UserGarageDBComponent userGarageDBComponent)
        {
            #region Visual
            //var playerTurret = userGarageDBComponent.selectedEquipment.Turrets[0];
            //var playerHull = userGarageDBComponent.selectedEquipment.Hulls[0];
            //var playerColormap = userGarageDBComponent.selectedEquipment.Colormaps[0];
            //var skinHullConfig = ConstantService.instance.GetByConfigPath(playerHull.Skins.Where(x => x.Equiped).ToList()[0].SkinPathName);
            //var skinTurretConfig = ConstantService.instance.GetByConfigPath(playerTurret.Skins.Where(x => x.Equiped).ToList()[0].SkinPathName);
            //var colormapSkinConfig = ConstantService.instance.GetByConfigPath(playerColormap.PathName);

            //IHullVisualController.InitializeController(HullPreviewObject, userGarageDBComponent.selectedEquipment);
            //ITurretVisualController.InitializeController(TurretPreviewObject, userGarageDBComponent.selectedEquipment);


            //ItemCard resourcesTurretObject;
            //ItemCard resourcesHullObject;
            //ItemCard resourcesColormapObject = ResourcesService.instance.GameAssets.GetDirectory(playerColormap.PathName).FillChildContentToItem();

            //if (!skinHullConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            //{
            //    resourcesHullObject = ResourcesService.instance.GameAssets.GetDirectory(skinHullConfig.Path + "\\" + playerHull.Grade.ToString()).FillChildContentToItem();
            //}
            //else
            //{
            //    resourcesHullObject = ResourcesService.instance.GameAssets.GetDirectory(skinHullConfig.Path).FillChildContentToItem();
            //}

            //if (!skinTurretConfig.Deserialized["skinType"]["singleForAllModificationsSkin"].ToObject<bool>())
            //{
            //    resourcesTurretObject = ResourcesService.instance.GameAssets.GetDirectory(skinTurretConfig.Path + "\\" + playerTurret.Grade.ToString()).FillChildContentToItem();
            //}
            //else
            //{
            //    resourcesTurretObject = ResourcesService.instance.GameAssets.GetDirectory(skinTurretConfig.Path).FillChildContentToItem();
            //}

            //var playerTurretPrefab = resourcesTurretObject.GetElement<GameObject>("model");
            //var playerTurretModel = playerTurretPrefab.GetComponent<MeshFilter>().sharedMesh;

            //var turretRenderer = TurretPreviewObject.GetComponent<MeshRenderer>();
            //turretRenderer.enabled = true;
            //TurretPreviewObject.GetComponent<MeshFilter>().mesh = playerTurretModel;


            //var playerHullPrefab = resourcesHullObject.GetElement<GameObject>("model");
            //var playerHullModel = playerHullPrefab.GetComponent<MeshFilter>().sharedMesh;

            //var hullRenderer = HullPreviewObject.GetComponent<MeshRenderer>();
            //hullRenderer.enabled = true;
            //HullPreviewObject.GetComponent<MeshFilter>().mesh = playerHullModel;

            //var playerColormapPrefab = resourcesColormapObject.GetElement<Texture>("image");

            //foreach (var material in turretRenderer.materials)
            //{
            //    //material.SetTexture("_Color", playerColormapPrefab);
            //    material.SetTexture("_Details", resourcesTurretObject.GetElement<Texture2D>("details"));
            //    material.SetTexture("_Lightmap", resourcesTurretObject.GetElement<Texture2D>("lightmap"));
            //}
            //foreach (var material in hullRenderer.materials)
            //{
            //    //material.SetTexture("_Color", playerColormapPrefab);
            //    material.SetTexture("_Details", resourcesHullObject.GetElement<Texture2D>("details"));
            //    material.SetTexture("_Lightmap", resourcesHullObject.GetElement<Texture2D>("lightmap"));
            //}
            //hullRenderer.GetComponent<ColormapScript>().Setup(colormapSkinConfig, resourcesColormapObject, true);
            //turretRenderer.GetComponent<ColormapScript>().Setup(colormapSkinConfig, resourcesColormapObject, true);
            //Vector3 mountPosition = Vector3.zero;
            //for (int i = 0; i < playerHullPrefab.transform.childCount; i++)
            //{
            //    if (playerHullPrefab.transform.GetChild(i).name.Contains("mount"))
            //    {
            //        mountPosition = playerHullPrefab.transform.GetChild(i).transform.localPosition;
            //    }
            //}
            //TurretPreviewObject.transform.localPosition = HullPreviewObject.transform.localPosition + mountPosition;
            #endregion

            var turretController = ITurretVisualController.InitializeController(TurretPreviewObject, userGarageDBComponent.selectedEquipment, true);
            var hullController = IHullVisualController.InitializeController(HullPreviewObject, userGarageDBComponent.selectedEquipment, true);
            ITankVisualController.CombineTankParts(turretController, hullController);
        }
    }
}