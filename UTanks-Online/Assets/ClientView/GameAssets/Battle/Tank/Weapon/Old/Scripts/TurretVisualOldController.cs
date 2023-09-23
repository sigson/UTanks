using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class TurretVisualOldController : ITurretVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            TurretVisualDefaultController.BuildVisualShared<TurretVisualOldController>(visualizableEquipment, eraItemCard, this);
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if (eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            TurretVisualDefaultController.BuildVisualShared<TurretVisualOldController>(visualizableEquipment, eraItemCard, this);
        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
            return (T)null;
        }

        public override void SetupColormap(ItemCard colormapResource)
        {
            TurretVisualDefaultController.SetupColormapShared(this, colormapResource);
        }

        protected override void OnRemoveController()
        {
            TurretVisualDefaultController.OnRemoveControllerShared(this);
        }

        public static new void OnRemoveControllerShared(ITurretVisualController turretVisualController)
        {

        }

        public override void SetGhostMode(bool enabled)
        {
            TurretVisualDefaultController.SetGhostModeShared(this, enabled);
        }

        public override void SetTemperature(float temperature)
        {
            TurretVisualDefaultController.SetTemperatureShared(this, temperature);
        }
    }
}