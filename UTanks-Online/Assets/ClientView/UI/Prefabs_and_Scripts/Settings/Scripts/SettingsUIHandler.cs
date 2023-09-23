using SecuredSpace.ClientControl.Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI.GameUI
{
    public class SettingsUIHandler : MonoBehaviour
    {
        public GameObject SettingsWindow;
        public MultiImageButton SettingsWindowCloseButton;
        [Header("GameSettings")]
        [Space(10)]
        public MultiImageButton ShowGameSettingsBlockButton;
        public GameObject GameSettingsBlockWindow;
        public Toggle ShowDamage;
        public Toggle ShowDropZones;
        public Toggle AlternateCameraBehaviour;
        public Toggle ShowChat;
        public Toggle ShowNotification;
        public Slider SoundVolumeRange;
        public Toggle BackgroundSound;
        public Toggle GameSound;
        [Header("GraphicsSettings")]
        [Space(10)]
        public MultiImageButton ShowGraphicsSettingsBlockButton;
        public GameObject GraphicsSettingsBlockWindow;
        public Toggle ShowSkybox;
        public Toggle EnableShadow;
        public Toggle DeepShadows;
        public Toggle LightShadows;
        public Toggle Antialiasing;
        public Toggle ShowFPSPing;
        public Toggle SoftParticles;
        public Toggle AutomaticGraphicsQuality;
        public Toggle Grass;
        public Toggle Dust;
        public Toggle SSAO;
        public Toggle Fog;
        public Toggle GlobalIllumination;
        public Toggle MapIllumination;
        public Toggle WeaponIllumination;
        public Toggle VolumetricLighting;
        public TMP_Dropdown vSyncDropdown;
        [Header("AccountSettings")]
        [Space(10)]
        public MultiImageButton ShowAccountSettingsBlockButton;
        public GameObject AccountSettingsBlockWindow;
        public MultiImageButton ChangePasswordAndEmailShowButton;
        public TMP_Text CurrentPasswordLabel;
        public TMP_InputField CurrentPasswordTextInput;
        public TMP_Text NewPasswordLabel;
        public TMP_InputField NewPasswordTextInput;
        public TMP_Text ReenterNewPasswordLabel;
        public TMP_InputField ReenterNewPasswordTextInput;
        public TMP_Text NewEmailLabel;
        public TMP_InputField NewEmailTextInput;
        public MultiImageButton ChangePasswordAndEmailActionButton;
        [Header("ControlsSettings")]
        [Space(10)]
        public MultiImageButton ShowControlsSettingsBlockButton;
        public GameObject ControlsSettingsBlockWindow;
        public GameObject MouseControls;
        public GameObject MouseLookSensitivityRange;
        public GameObject MouseLookVerticalInversion;
        public GameObject InverseTurnControlsWhileMovingBack;
        public GameObject RestoreDefaults;

        void Start()
        {
            #region settingHandlers
            ShowDamage.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.ShowDamage = ShowDamage.isOn);
            ShowDropZones.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.ShowDropZones = ShowDropZones.isOn);
            AlternateCameraBehaviour.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.AlternateCameraBehaviour = AlternateCameraBehaviour.isOn);
            ShowChat.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.ShowChat = ShowChat.isOn);
            //ShowNotification.onValueChanged.AddListener((bool changeValue) => ClientInit.gameSettings.ShowNotification = ShowNotification.isOn);
            SoundVolumeRange.onValueChanged.AddListener((float changeValue) => ClientInitService.instance.gameSettings.SoundVolumeRange = SoundVolumeRange.value);
            BackgroundSound.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.BackgroundSound = BackgroundSound.isOn);
            GameSound.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.GameSound = GameSound.isOn);
            ShowSkybox.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.ShowSkybox = ShowSkybox.isOn);
            EnableShadow.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.EnableShadow = EnableShadow.isOn);
            DeepShadows.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.DeepShadows = DeepShadows.isOn);
            LightShadows.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.LightShadows = LightShadows.isOn);
            Antialiasing.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.Antialiasing = Antialiasing.isOn);
            ShowFPSPing.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.ShowFPSPing = ShowFPSPing.isOn);
            SoftParticles.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.SoftParticles = SoftParticles.isOn);
            AutomaticGraphicsQuality.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.AutomaticGraphicsQuality = AutomaticGraphicsQuality.isOn);
            Grass.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.Grass = Grass.isOn);
            Dust.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.Dust = Dust.isOn);
            SSAO.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.SSAO = SSAO.isOn);
            Fog.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.Fog = Fog.isOn);
            GlobalIllumination.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.GlobalIllumination = GlobalIllumination.isOn);
            MapIllumination.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.MapIllumination = MapIllumination.isOn);
            WeaponIllumination.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.WeaponIllumination = WeaponIllumination.isOn);
            VolumetricLighting.onValueChanged.AddListener((bool changeValue) => ClientInitService.instance.gameSettings.VolumetricLighting = VolumetricLighting.isOn);
            vSyncDropdown.onValueChanged.AddListener((int changeValue) => ClientInitService.instance.gameSettings.VSyncCount = vSyncDropdown.value);
            #endregion
            //GoToBattlesButton.GetComponent<Button>().onClick.AddListener(OnGoToBattlesButton);
            //OpenSettingsWindowButton.GetComponent<Button>().onClick.AddListener(OnOpenSettings);
            SettingsWindowCloseButton.onClick.AddListener(() => UIService.instance.GameSettingsUI.SetActive(false));
            ShowGameSettingsBlockButton.onClick.AddListener(() => {
                HideAllTabs();
                GameSettingsBlockWindow.SetActive(true); 
            });
            ShowGraphicsSettingsBlockButton.onClick.AddListener(() => {
                HideAllTabs();
                GraphicsSettingsBlockWindow.SetActive(true);
            });
            ShowAccountSettingsBlockButton.onClick.AddListener(() => {
                HideAllTabs();
                AccountSettingsBlockWindow.SetActive(true);
            });
            ShowControlsSettingsBlockButton.onClick.AddListener(() => {
                HideAllTabs();
                ControlsSettingsBlockWindow.SetActive(true);
            });
        }

        public void UpdateSettings()
        {
            ShowDamage.isOn = ClientInitService.instance.gameSettings.ShowDamage;
            ShowDropZones.isOn = ClientInitService.instance.gameSettings.ShowDropZones;
            AlternateCameraBehaviour.isOn = ClientInitService.instance.gameSettings.AlternateCameraBehaviour;
            ShowChat.isOn = ClientInitService.instance.gameSettings.ShowChat;
            ShowNotification.isOn = false;
            SoundVolumeRange.value = ClientInitService.instance.gameSettings.SoundVolumeRange;
            BackgroundSound.isOn = ClientInitService.instance.gameSettings.BackgroundSound;
            GameSound.isOn = ClientInitService.instance.gameSettings.GameSound;
            ShowSkybox.isOn = ClientInitService.instance.gameSettings.ShowSkybox;
            EnableShadow.isOn = ClientInitService.instance.gameSettings.EnableShadow;
            DeepShadows.isOn = ClientInitService.instance.gameSettings.DeepShadows;
            LightShadows.isOn = ClientInitService.instance.gameSettings.LightShadows;
            Antialiasing.isOn = ClientInitService.instance.gameSettings.Antialiasing;
            ShowFPSPing.isOn = ClientInitService.instance.gameSettings.ShowFPSPing;
            SoftParticles.isOn = ClientInitService.instance.gameSettings.SoftParticles;
            AutomaticGraphicsQuality.isOn = ClientInitService.instance.gameSettings.AutomaticGraphicsQuality;
            Grass.isOn = ClientInitService.instance.gameSettings.Grass;
            Dust.isOn = ClientInitService.instance.gameSettings.Dust;
            SSAO.isOn = ClientInitService.instance.gameSettings.SSAO;
            Fog.isOn = ClientInitService.instance.gameSettings.Fog;
            GlobalIllumination.isOn = ClientInitService.instance.gameSettings.GlobalIllumination;
            MapIllumination.isOn = ClientInitService.instance.gameSettings.MapIllumination;
            WeaponIllumination.isOn = ClientInitService.instance.gameSettings.WeaponIllumination;
            VolumetricLighting.isOn = ClientInitService.instance.gameSettings.VolumetricLighting;
            vSyncDropdown.value = ClientInitService.instance.gameSettings.VSyncCount;
        }
    
        public void HideAllTabs()
        {
            GameSettingsBlockWindow.SetActive(false);
            GraphicsSettingsBlockWindow.SetActive(false);
            AccountSettingsBlockWindow.SetActive(false);
            ControlsSettingsBlockWindow.SetActive(false);
        }
    }
}