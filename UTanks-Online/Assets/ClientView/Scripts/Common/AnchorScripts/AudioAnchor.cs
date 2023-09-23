using SecuredSpace.AudioManager.Core;
using SecuredSpace.AudioManager.Helper;
using SecuredSpace.AudioManager.Locator;
using SecuredSpace.AudioManager.Logger;
using SecuredSpace.AudioManager.Provider;
using SecuredSpace.AudioManager.Service;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Common.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Settings
{
    public class AudioAnchor : IAnchor
    {
        public bool GameplaySound;
        public bool BackgroundSound;
        public float volume = 0;

        [SerializeField]
        [Header("Hide Settings:")]
        [Tooltip("Defines how much and if the gameObject this script is attached too, should be hidden in the scene hierarchy.")]
        private HideFlags customHideFlags;

        [SerializeField]
        [Header("Logger Settings:")]
        [Tooltip("Maximum level the logger should output messages at, any priority lower and the logger will not print that message.")]
        private LoggingLevel loggingLevel;

        [SerializeField]
        [Header("Sound Settings:")]
        [Tooltip("Inital sounds that should be registered on Awake with the AudioManager and the given settings.")]
        public List<AudioSourceSetting> audioStorage = new List<AudioSourceSetting>();
        [SerializeField]
        private AudioAnchor audioAnchor = null;
        private bool inited = false;
        [SerializeField]
        public IAudioManager audioManager;

        private void OnEnable()
        {
            gameObject.hideFlags = customHideFlags;
        }

        protected override IEnumerator UpdateObjectImpl()
        {
            if (GameplaySound)
                volume = (ClientInitService.instance.gameSettings.GameSound) ? ClientInitService.instance.gameSettings.SoundVolumeRange : 0f;
            if (BackgroundSound)
                volume = (ClientInitService.instance.gameSettings.BackgroundSound) ? ClientInitService.instance.gameSettings.SoundVolumeRange : 0f;
            if(audioManager != null)
                audioManager.UpdateSettings();
            yield return null;
        }

        protected override void OnDestroyImpl()
        {
            base.OnDestroyImpl();
            if (audioManager != null)
                audioManager.DestroyCleanup();

        }

        public void Build()
        {
            // Make gameObject persistent so that audio keeps playing over scene changes,
            // as all audioSources and emtpy gameObjects get attached or parented to the passed gameObject in the AudioManager constructor.

            // Ensure this is the only instance that has been registered as DontDestroyOnLoad, if not detroy this instance.
            // This is done to ensure we don't create a new instance each time we reload the scene.

            if(audioManager != null)
            {
                audioManager.DestroyCleanup();
            }

            SettingsHelper.SetupSounds(out var sounds, audioStorage.ToArray(), this.gameObject);
            if (audioAnchor == null)
            {
                audioAnchor = this;
            }
            foreach (var sound in sounds)
            {
                sound.Value.audioAnchor = audioAnchor;
                sound.Value.Source.playOnAwake = false;
            }
            audioManager = new DefaultAudioManager(sounds, this.gameObject) { audioAnchor = audioAnchor };
            // Only register a logger if we are in the Editor and the logging level is higher than LoggingLevel.NONE.
            // This is done to ensure no needless Debug.Log calls get made when the game is built and needs no debug output.
            inited = true;
        }

        // Test methods used only for UnitTesting.
#if UNITY_EDITOR
        public void SetCustomHideFlags(HideFlags testHideFlags)
        {
            customHideFlags = testHideFlags;
        }

        public void SetLoggingLevel(LoggingLevel testLoggingLevel)
        {
            loggingLevel = testLoggingLevel;
        }
#endif // UNITY_EDITOR
    }

}