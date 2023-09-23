using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.Battle.Tank;
using SecuredSpace.Battle.Tank.Hull;
using SecuredSpace.Battle.Tank.Turret;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types;
using UTanksClient.Services;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class ITankVisualController : IController
    {
        #region HiddenDebugLinks
        private HullVisualDefaultController hullVisualDefaultController;
        private HullVisual20Controller hullVisual20Controller;
        private HullVisualOldController hullVisualOldController;
        private HullVisualTXController hullVisualTXController;
        private HullVisualHDController hullVisualHDController;

        private TurretVisualDefaultController turretVisualDefaultController;
        private TurretVisual20Controller turretVisual20Controller;
        private TurretVisualOldController turretVisualOldController;
        private TurretVisualTXController turretVisualTXController;
        private TurretVisualHDController turretVisualHDController;
        #endregion

        public Dictionary<string, Material> normalMaterials = new Dictionary<string, Material>();
        public Dictionary<string, Material> transparentMaterials = new Dictionary<string, Material>();
        public TankManager parentTankManager => (this.ownerManagerSpace as TankManager);
        public bool Preview = false;
        public ConfigObj skinConfig;
        public ConfigObj gameplayConfig;

        public static void CombineTankParts(ITurretVisualController turretVisualController, IHullVisualController hullVisualController)
        {
            var turretVisualControllerParent = (turretVisualController.Preview ? turretVisualController.transform : turretVisualController.transform.parent);
            var cacheTurretVisualControllerParent = turretVisualControllerParent.transform.parent;
            turretVisualControllerParent.transform.SetParent(hullVisualController.transform.parent);

            var cachePosition = hullVisualController.transform.localPosition;
            hullVisualController.transform.localPosition = Vector3.zero;
            turretVisualControllerParent.transform.localPosition = hullVisualController.transform.localPosition + hullVisualController.localMountPoint;

            //var turretParentCache = turretVisualControllerParent.transform.parent;
            //turretVisualControllerParent.transform.SetParent(hullVisualController.transform.parent);
            hullVisualController.transform.localPosition = Vector3.zero;

            turretVisualControllerParent.transform.localPosition = new Vector3(turretVisualControllerParent.transform.localPosition.x, turretVisualControllerParent.transform.localPosition.y + hullVisualController.HullVisibleModel.transform.localPosition.y, turretVisualControllerParent.transform.localPosition.z);
            hullVisualController.transform.localPosition = cachePosition;

            var turretCacheGlobal = turretVisualControllerParent.transform.position;
            
            turretVisualControllerParent.transform.SetParent(cacheTurretVisualControllerParent);
            turretVisualControllerParent.transform.position = turretCacheGlobal;
            turretVisualControllerParent.transform.localPosition += hullVisualController.transform.localPosition;
        }

        public abstract void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard);

        public abstract void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard);

        public static T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
            return (T)null;
        }

        protected abstract void OnRemoveController();

        public virtual void RemoveController()
        {
            OnRemoveController();
            Destroy(this);
        }
    }
}