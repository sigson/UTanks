using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullVisualOldController : IHullVisualController
    {
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            HullVisualDefaultController.BuildVisualShared<HullVisualOldController>(visualizableEquipment, eraItemCard, this);
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if (eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            this.HullVisibleModel = this.gameObject;
            HullVisualDefaultController.BuildVisualShared<HullVisualOldController>(visualizableEquipment, eraItemCard, this);
        }

        public static new T BuildVisualShared<T>(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard, T tankVisualController) where T : ITankVisualController
        {
            return (T)null;
        }

        public override void SetupColormap(ItemCard colormapResource)
        {
            HullVisualDefaultController.SetupColormapShared(this, colormapResource);
        }

        protected override void OnRemoveController()
        {
            HullVisualDefaultController.OnRemoveControllerShared(this);
        }

        public static new void OnRemoveControllerShared(IHullVisualController hullVisualController)
        {

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
            //HullVisualDefaultController.MoveAnimationShared(this, MoveMomentX, MoveMomentY, 0);
            var playedAudio = this.hullAudio.audioManager.GetNowPlayingAudioName();
            if (MoveMomentX == 0)
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("idle") == -1))
                {
                    //hullVisualController.hullAudio.audioManager.StopAll();
                    this.hullAudio.audioManager.Fade("audio_move");
                    this.hullAudio.audioManager.Fade("audio_move_start");
                    this.hullAudio.audioManager.Stop("audio_engineidle");
                    this.hullAudio.audioManager.Stop("audio_engineidle_loop");
                    this.hullAudio.audioManager.PlayBlock(new List<string> { "audio_engineidle", "audio_engineidle_loop" });
                }
            }
            else
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("move") == -1))
                {
                    this.hullAudio.audioManager.Fade("audio_engineidle");
                    this.hullAudio.audioManager.Fade("audio_engineidle_loop");
                    this.hullAudio.audioManager.Stop("audio_move");
                    this.hullAudio.audioManager.Stop("audio_move_start");
                    this.hullAudio.audioManager.PlayBlock(new List<string> { "audio_move_start", "audio_move" });
                }
            }
        }
    }
}