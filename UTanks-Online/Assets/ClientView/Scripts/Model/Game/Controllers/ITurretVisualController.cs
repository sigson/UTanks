using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle.Tank;
using SecuredSpace.Battle.Tank.Turret;
using SecuredSpace.Important.Aim;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.Types;
using UTanksClient.Services;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class ITurretVisualController : ITankVisualController
    {
        public enum TurretHappeningStates
        {
            Idle,
            Rotation,
            Shot,
            Burning,
            Freezed
        }

        public HashSet<TurretHappeningStates> turretState;

        public List<GameObject> MuzzlePoints = new List<GameObject>();
        private GameObject muzzlePointExampleCache;
        public GameObject MuzzlePointExample
        {
            get
            {
                if(muzzlePointExampleCache == null)
                {
                    muzzlePointExampleCache = Instantiate(ResourcesService.instance.GetPrefab("MuzzlePoint"), this.transform.parent);
                    muzzlePointExampleCache.SetActive(false);
                }
                return muzzlePointExampleCache;
            }
        }
        public GameObject MuzzlePoint;
        public int selectedMuzzlePoint = 0;
        public GameObject MuzzleCheckPoint;

        public RaycastAIM raycastAIM;
        public CloseDistanceAIM closeDistanceAIM;
        public ITurret turretManager;

        public GameObject TurretPrefab;
        public ItemCard TurretResources;
        public ItemCard TurretSkinResources;
        public ItemCard ColormapResources;
        public ItemCard DeadColormapResources;

        public ConfigObj colormapSkinConfig;

        protected static string MainTurretMaterial = "Material";
        protected static string MainTurretTransparentMaterial = "TPMaterial";

        public abstract void SetupColormap(ItemCard colormapResource);
        public abstract void SetGhostMode(bool enabled);
        public abstract void SetTemperature(float temperature);

        public static void OnRemoveControllerShared(ITurretVisualController turretVisualController)
        {

        }

        public static void SetupColormapShared(ITurretVisualController turretVisualController, ItemCard colormapResource)
        {

        }

        public static void SetGhostModeShared(ITurretVisualController turretVisualController, bool enable)
        {

        }

        public static void SetTemperatureShared(ITurretVisualController turretVisualController, float temperature)
        {

        }

        public static ITurretVisualController InitializeController(GameObject ownerObject, VisualizableEquipment visualizableEquipment, bool preview = false, IEntityManager entityManager = null)
        {
            var oldController = ownerObject.GetComponent<ITankVisualController>();
            if (oldController != null)
                oldController.RemoveController();

            var playerTurret = visualizableEquipment.Turrets[0];
            var skinTurretConfig = ConstantService.instance.GetByConfigPath(playerTurret.Skins.Where(x => x.Equiped).ToList()[0].SkinPathName);

            var eraItemCard = ResourcesService.instance.GameAssets.GetDirectory("garage\\skin\\weapon").GetChildFSObject("item").GetContent<ItemCard>().GetElement<ItemCard>(skinTurretConfig.GetObject<string>("skinType\\skinEra"));

            var turretSkinEraController = eraItemCard.GetElement<GameObject>("VisualController").GetComponent<ITurretVisualController>();

            ITurretVisualController TurretVisualController = null;

            if (turretSkinEraController != null)
            {
                TurretVisualController = IController.AppendControllerToTarget(ownerObject, turretSkinEraController.GetType()) as ITurretVisualController;
                if (entityManager != null)
                    TurretVisualController.ownerManagerSpace = entityManager;

                TurretVisualController.DeadColormapResources = ResourcesService.instance.GameAssets.GetDirectory("garage\\colormap\\dead").FillChildContentToItem();
                TurretVisualController.gameplayConfig = ConstantService.instance.ConstantDB["battle\\weapon" + visualizableEquipment.Turrets[0].PathName.Substring(visualizableEquipment.Turrets[0].PathName.LastIndexOf("\\"))];

                TurretVisualController.Preview = preview;
                if (preview)
                    TurretVisualController.BuildPreviewVisual(visualizableEquipment, eraItemCard.GetClone());
                else
                    TurretVisualController.BuildVisual(visualizableEquipment, eraItemCard.GetClone());
            }

            

            return TurretVisualController;
        }
    }
}