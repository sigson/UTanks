using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle.Tank;
using SecuredSpace.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.Types;
using UTanksClient.Services;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IHullVisualController : ITankVisualController
    {
        public enum HullHappeningStates
        {
            Idle,
            Movement,
            Burning,
            Freezed
        }

        public HashSet<HullHappeningStates> hullState;

        public Vector3 localMountPoint = Vector3.zero;
        public Vector2 TrackTextureOffset = Vector2.zero;
        public AudioAnchor hullAudio => parentTankManager.hullAudioSource;

        private GameObject CacheTankFrictionCollidersObject;
        public GameObject TankFrictionCollidersObject { 
            get {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).TankFrictionCollidersObject : CacheTankFrictionCollidersObject;
            } 
            set {
                if (this.ownerManagerSpace == null)
                    CacheTankFrictionCollidersObject = value;
                else
                    (this.ownerManagerSpace as TankManager).TankFrictionCollidersObject = value;
            }
        }

        private GameObject CacheTankBoundsObject;
        public GameObject TankBoundsObject
        {
            get
            {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).TankBoundsObject : CacheTankBoundsObject;
            }
            set
            {
                if (this.ownerManagerSpace == null)
                    CacheTankBoundsObject = value;
                else
                    (this.ownerManagerSpace as TankManager).TankBoundsObject = value;
            }
        }

        private GameObject CacheHullCenterCollider;
        public GameObject HullCenterCollider
        {
            get
            {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).HullCenterCollider : CacheHullCenterCollider;
            }
            set
            {
                if (this.ownerManagerSpace == null)
                    CacheHullCenterCollider = value;
                else
                    (this.ownerManagerSpace as TankManager).HullCenterCollider = value;
            }
        }

        private GameObject CacheHullBalanceCollider;
        public GameObject HullBalanceCollider
        {
            get
            {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).HullBalanceCollider : CacheHullBalanceCollider;
            }
            set
            {
                if (this.ownerManagerSpace == null)
                    CacheHullBalanceCollider = value;
                else
                    (this.ownerManagerSpace as TankManager).HullBalanceCollider = value;
            }
        }

        private GameObject CacheHullVisibleModel;
        public GameObject HullVisibleModel
        {
            get
            {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).HullVisibleModel : CacheHullVisibleModel;
            }
            set
            {
                if (this.ownerManagerSpace == null)
                    CacheHullVisibleModel = value;
                else
                    (this.ownerManagerSpace as TankManager).HullVisibleModel = value;
            }
        }

        private GameObject CacheHullAngleColliderHeader;
        public GameObject HullAngleColliderHeader
        {
            get
            {
                return this.ownerManagerSpace != null ? (this.ownerManagerSpace as TankManager).HullAngleColliderHeader : CacheHullAngleColliderHeader;
            }
            set
            {
                if (this.ownerManagerSpace == null)
                    CacheHullAngleColliderHeader = value;
                else
                    (this.ownerManagerSpace as TankManager).HullAngleColliderHeader = value;
            }
        }

        public GameObject HullPrefab;
        public ItemCard HullResources;
        public ItemCard HullSkinResources;
        public ItemCard ColormapResources;
        public ItemCard DeadColormapResources;

        public ConfigObj colormapSkinConfig;
        
        public Material TrackMaterial;

        protected static string MainHullMaterial = "Material";
        protected static string MainTrackMaterial = "TrackMaterial";
        protected static string MainHullTransparentMaterial = "TPMaterial";
        protected static string MainTrackTransparentMaterial = "TrackTPMaterial";

        public abstract void SetupColormap(ItemCard colormapResource);
        public abstract void SetGhostMode(bool enabled);
        public abstract void SetTemperature(float temperature);
        public abstract void MoveAnimation(float MoveMomentX, float MoveMomentY);

        public static void OnRemoveControllerShared(IHullVisualController hullVisualController)
        {

        }

        public static void SetGhostModeShared(IHullVisualController hullVisualController, bool enable)
        {

        }

        public static void SetTemperatureShared(IHullVisualController hullVisualController, float temperature)
        {

        }

        public static void SetupColormapShared(IHullVisualController turretVisualController, ItemCard colormapResource)
        {

        }

        public static void MoveAnimationShared(IHullVisualController hullVisualController, float MoveMomentX, float MoveMomentY, int trackgroup)
        {

        }

        public static IHullVisualController InitializeController(GameObject ownerObject, VisualizableEquipment visualizableEquipment, bool preview = false, IEntityManager entityManager = null)
        {
            var oldController = ownerObject.GetComponent<ITankVisualController>();
            if (oldController != null)
                oldController.RemoveController();

            var playerHull = visualizableEquipment.Hulls[0];
            var skinHullConfig = ConstantService.instance.GetByConfigPath(playerHull.Skins.Where(x => x.Equiped).ToList()[0].SkinPathName);

            var eraItemCard = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin\\tank").GetChildFSObject("item").GetContent<ItemCard>().GetElement<ItemCard>(skinHullConfig.GetObject<string>("skinType\\skinEra"));

            var hullSkinEraController = eraItemCard.GetElement<GameObject>("VisualController").GetComponent<IHullVisualController>();

            IHullVisualController HullVisualController = null;

            if (hullSkinEraController != null)
            {
                HullVisualController = IController.AppendControllerToTarget(ownerObject, hullSkinEraController.GetType()) as IHullVisualController;
                if(entityManager != null)
                    HullVisualController.ownerManagerSpace = entityManager;

                HullVisualController.DeadColormapResources = ResourcesService.instance.GameAssets.GetDirectory("garage\\colormap\\dead").FillChildContentToItem();
                HullVisualController.gameplayConfig = ConstantService.instance.ConstantDB["battle\\tank" + visualizableEquipment.Hulls[0].PathName.Substring(visualizableEquipment.Hulls[0].PathName.LastIndexOf("\\"))];

                HullVisualController.Preview = preview;
                if (preview)
                    HullVisualController.BuildPreviewVisual(visualizableEquipment, eraItemCard.GetClone());
                else
                    HullVisualController.BuildVisual(visualizableEquipment, eraItemCard.GetClone());
            }

            

            return HullVisualController;
        }
    }
}