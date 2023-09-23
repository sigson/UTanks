using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.ECS.Types;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullVisual20Controller : IHullVisualController
    {
        List<Material> trackgroup = new List<Material>();
        public override void BuildVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            HullVisualDefaultController.BuildVisualShared<HullVisual20Controller>(visualizableEquipment, eraItemCard, this);
            var newMaterialArray = HullVisibleModel.GetComponent<MeshRenderer>().materials.ToList();
            trackgroup.Add(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last());
            newMaterialArray.Add(Instantiate(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last()));
            HullVisibleModel.GetComponent<MeshRenderer>().materials = newMaterialArray.ToArray();
            trackgroup.Add(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last());
        }

        public override void BuildPreviewVisual(VisualizableEquipment visualizableEquipment, ItemCard eraItemCard)
        {
            if (eraItemCard.ItemData.ContainsKey("PreviewMaterial"))
            {
                eraItemCard.ItemData.Remove("Material");
                eraItemCard.ItemData["Material"] = eraItemCard.ItemData["PreviewMaterial"];
            }
            this.HullVisibleModel = this.gameObject;
            HullVisualDefaultController.BuildVisualShared<HullVisual20Controller>(visualizableEquipment, eraItemCard, this);
            var newMaterialArray = HullVisibleModel.GetComponent<MeshRenderer>().materials.ToList();
            trackgroup.Add(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last());
            newMaterialArray.Add(Instantiate(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last()));
            HullVisibleModel.GetComponent<MeshRenderer>().materials = newMaterialArray.ToArray();
            trackgroup.Add(HullVisibleModel.GetComponent<MeshRenderer>().materials.Last());
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
            HullVisual20Controller.MoveAnimationShared(this, MoveMomentX, MoveMomentY);
        }

        public static new void MoveAnimationShared(IHullVisualController hullVisualController, float MoveMomentX, float MoveMomentY)
        {
            var playedAudio = hullVisualController.hullAudio.audioManager.GetNowPlayingAudioName();
            if (MoveMomentX == 0)
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("idle") == -1))
                {
                    //hullVisualController.hullAudio.audioManager.StopAll();
                    hullVisualController.hullAudio.audioManager.Fade("audio_move");
                    hullVisualController.hullAudio.audioManager.Fade("audio_move_start");
                    hullVisualController.hullAudio.audioManager.Stop("audio_engineidle");
                    hullVisualController.hullAudio.audioManager.Stop("audio_engineidle_loop");
                    hullVisualController.hullAudio.audioManager.PlayBlock(new List<string> { "audio_engineidle", "audio_engineidle_loop" });
                }
            }
            else
            {
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("move") == -1))
                {
                    hullVisualController.hullAudio.audioManager.Fade("audio_engineidle");
                    hullVisualController.hullAudio.audioManager.Fade("audio_engineidle_loop");
                    hullVisualController.hullAudio.audioManager.Stop("audio_move");
                    hullVisualController.hullAudio.audioManager.Stop("audio_move_start");
                    hullVisualController.hullAudio.audioManager.PlayBlock(new List<string> { "audio_move_start", "audio_move" });
                }
            }

            var TrackTextureOffset = hullVisualController.TrackTextureOffset;
            var hullVisualController20 = hullVisualController as HullVisual20Controller;
            TrackTextureOffset.x += Mathf.Lerp(TrackTextureOffset.x, MoveMomentX * Time.fixedDeltaTime, Time.fixedDeltaTime);
            // TrackTextureOffset += new Vector2(chassisNode.chassis.EffectiveMoveAxis * 0.001f, 0f);
            try
            {
                hullVisualController20.trackgroup.ForEach(x => {
                    x.SetTextureOffset("_Lightmap", TrackTextureOffset);
                    x.SetTextureOffset("_Details", TrackTextureOffset);
                });
                
            }
            catch { }
            if (TrackTextureOffset.x > 10f || TrackTextureOffset.x < -10f)
                TrackTextureOffset.x = 0f;
            hullVisualController.TrackTextureOffset = TrackTextureOffset;
        }
    }
}