using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullVisualTXController : IHullVisualController
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
            //HullVisualDefaultController.OnRemoveControllerShared(this);
        }

        public static new void OnRemoveControllerShared(IHullVisualController hullVisualController)
        {

        }

        public override void SetGhostMode(bool enabled)
        {

        }

        public override void SetTemperature(float temperature)
        {
            
        }

        public override void MoveAnimation(float MoveMomentX, float MoveMomentY)
        {
            
        }
    }
}