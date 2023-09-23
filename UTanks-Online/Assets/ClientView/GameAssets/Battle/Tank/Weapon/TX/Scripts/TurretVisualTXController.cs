using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class TurretVisualTXController : ITurretVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {

        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {

        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
            return (T)null;
        }

        public override void SetupColormap(ItemCard colormapResource)
        {

        }

        protected override void OnRemoveController()
        {

        }

        public static new void OnRemoveControllerShared(ITurretVisualController turretVisualController)
        {

        }

        public override void SetGhostMode(bool enabled)
        {

        }

        public override void SetTemperature(float temperature)
        {
            
        }
    }
}