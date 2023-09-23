using SecuredSpace.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Settings.SettingsObject
{
    public class GameSettings
    {
        //gamesettings
        private bool showDamage;
        public bool ShowDamage { 
            get 
            {
                return showDamage;
            }
            set
            {
                showDamage = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool showDropZones;
        public bool ShowDropZones
        {
            get
            {
                return showDropZones;
            }
            set
            {
                showDropZones = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool alternateCameraBehaviour;
        public bool AlternateCameraBehaviour
        {
            get
            {
                return alternateCameraBehaviour;
            }
            set
            {
                alternateCameraBehaviour = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool showChat;
        public bool ShowChat
        {
            get
            {
                return showChat;
            }
            set
            {
                showChat = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private float soundVolumeRange;
        public float SoundVolumeRange
        {
            get
            {
                return soundVolumeRange;
            }
            set
            {
                soundVolumeRange = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<AudioAnchor>((anchor) => true);
            }
        }
        private int vSyncCount;
        public int VSyncCount
        {
            get
            {
                return vSyncCount;
            }
            set
            {
                vSyncCount = value;
                QualitySettings.vSyncCount = vSyncCount;
            }
        }
        private bool backgroundSound;
        public bool BackgroundSound
        {
            get
            {
                return backgroundSound;
            }
            set
            {
                backgroundSound = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<AudioAnchor>((anchor) => true);
            }
        }
        private bool gameSound;
        public bool GameSound
        {
            get
            {
                return gameSound;
            }
            set
            {
                gameSound = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<AudioAnchor>((anchor) => true);
            }
        }
        //graphsettings
        private bool showSkybox;
        public bool ShowSkybox
        {
            get
            {
                return showSkybox;
            }
            set
            {
                showSkybox = value;
                if (!showSkybox)
                    RenderSettings.skybox = null;
                else
                    RenderSettings.skybox = BattleManager.SkyboxMaterial;
            }
        }
        private bool enableShadow;
        public bool EnableShadow
        {
            get
            {
                return enableShadow;
            }
            set
            {
                enableShadow = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<PropAnchor>((anchor) => true);
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }
        private bool deepShadows;
        public bool DeepShadows
        {
            get
            {
                return deepShadows;
            }
            set
            {
                deepShadows = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }

        private bool volumetricLighting;
        public bool VolumetricLighting
        {
            get
            {
                return volumetricLighting;
            }
            set
            {
                volumetricLighting = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<PostProcessAnchor>((anchor) => true);
            }
        }

        private bool globalIllumination;
        public bool GlobalIllumination
        {
            get
            {
                return globalIllumination;
            }
            set
            {
                globalIllumination = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }

        private bool mapIllumination;
        public bool MapIllumination
        {
            get
            {
                return mapIllumination;
            }
            set
            {
                mapIllumination = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }

        private bool weaponIllumination;
        public bool WeaponIllumination
        {
            get
            {
                return weaponIllumination;
            }
            set
            {
                weaponIllumination = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }

        private bool lightShadows;
        public bool LightShadows
        {
            get
            {
                return lightShadows;
            }
            set
            {
                lightShadows = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }
        private bool antialiasing;
        public bool Antialiasing
        {
            get
            {
                return antialiasing;
            }
            set
            {
                antialiasing = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool showFPSPing;
        public bool ShowFPSPing
        {
            get
            {
                return showFPSPing;
            }
            set
            {
                showFPSPing = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool softParticles;
        public bool SoftParticles
        {
            get
            {
                return softParticles;
            }
            set
            {
                softParticles = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool automaticGraphicsQuality;
        public bool AutomaticGraphicsQuality
        {
            get
            {
                return automaticGraphicsQuality;
            }
            set
            {
                automaticGraphicsQuality = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool grass;
        public bool Grass
        {
            get
            {
                return grass;
            }
            set
            {
                grass = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<PropAnchor>((anchor) => true);
            }
        }
        private bool dust;
        public bool Dust
        {
            get
            {
                return dust;
            }
            set
            {
                dust = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool sSAO;
        public bool SSAO
        {
            get
            {
                return sSAO;
            }
            set
            {
                sSAO = value;
                ClientInitService.instance.anchorHandler.UpdateAnchors<PostProcessAnchor>((anchor) => true);
            }
        }
        private bool fog;
        public bool Fog
        {
            get
            {
                return fog;
            }
            set
            {
                fog = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool tankTraces;
        public bool TankTraces
        {
            get
            {
                return tankTraces;
            }
            set
            {
                tankTraces = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private bool showWeather;
        public bool ShowWeather
        {
            get
            {
                return showWeather;
            }
            set
            {
                showWeather = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
        private string clientLanguage;
        public string ClientLanguage
        {
            get
            {
                return clientLanguage;
            }
            set
            {
                clientLanguage = value;
                //ClientInit.anchorHandler.UpdateAnchors<PropAnchor>();
            }
        }
    }

}