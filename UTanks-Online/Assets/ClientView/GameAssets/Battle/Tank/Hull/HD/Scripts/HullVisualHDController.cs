using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullVisualHDController : IHullVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            HullVisualDefaultController.BuildVisualShared<HullVisualHDController>(visualizableEquipment, eraItemCard, this);
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if (eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            this.HullVisibleModel = this.gameObject;
            HullVisualDefaultController.BuildVisualShared<HullVisualHDController>(visualizableEquipment, eraItemCard, this);
        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
            return (T)null;
        }

        protected override void OnRemoveController()
        {
            HullVisualDefaultController.OnRemoveControllerShared(this);
        }

        public static new void OnRemoveControllerShared(IHullVisualController hullVisualController)
        {

        }

        public override void SetupColormap(ItemCard colormapResource)
        {
            HullVisualDefaultController.SetupColormapShared(this, colormapResource);
        }

        public override void SetGhostMode(bool enabled)
        {
            HullVisualDefaultController.SetGhostModeShared(this, enabled);
        }

        public override void SetTemperature(float temperature)
        {
            HullVisualDefaultController.SetTemperatureShared(this, temperature);
        }

        public override void MoveAnimation(float MoveMomentX, float MoveMomentY)
        {
            HullVisualDefaultController.MoveAnimationShared(this, MoveMomentX, MoveMomentY, 1);
        }
    }
}