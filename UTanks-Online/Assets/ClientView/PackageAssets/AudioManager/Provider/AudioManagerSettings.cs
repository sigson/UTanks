using SecuredSpace.AudioManager.Helper;
using SecuredSpace.AudioManager.Locator;
using SecuredSpace.AudioManager.Logger;
using SecuredSpace.AudioManager.Service;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Settings;
using UnityEngine;

namespace SecuredSpace.AudioManager.Provider {
    public class AudioManagerSettings : MonoBehaviour {
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
        private AudioSourceSetting[] settings;
        [SerializeField]
        private AudioAnchor audioAnchor = null;

        private static AudioManagerSettings m_instace;
        private bool inited = false;

        private void OnEnable() {
            // When the gameObject first gets enabled we set the given hideFlags.
            gameObject.hideFlags = customHideFlags;
        }

        private void Awake() {
            if (!inited)
                Initialize();
        }

        public void Initialize()
        {
            // Make gameObject persistent so that audio keeps playing over scene changes,
            // as all audioSources and emtpy gameObjects get attached or parented to the passed gameObject in the AudioManager constructor.
            DontDestroyOnLoad(gameObject);

            // Ensure this is the only instance that has been registered as DontDestroyOnLoad, if not detroy this instance.
            // This is done to ensure we don't create a new instance each time we reload the scene.
            if (m_instace != null)
            {
                Destroy(gameObject);
                return;
            }
            m_instace = this;

            SettingsHelper.SetupSounds(out var sounds, settings, this.gameObject);
            if(audioAnchor == null)
            {
                audioAnchor = this.gameObject.GetComponent<AudioAnchor>();
            }
            AudioServiceLocator.RegisterService(new DefaultAudioManager(sounds, this.gameObject) { audioAnchor = audioAnchor });
            // Only register a logger if we are in the Editor and the logging level is higher than LoggingLevel.NONE.
            // This is done to ensure no needless Debug.Log calls get made when the game is built and needs no debug output.
#if UNITY_EDITOR
            if (IsLoggingEnabled(loggingLevel))
            {
                AudioServiceLocator.RegisterLogger(new AudioLogger(loggingLevel), this);
            }
#endif // UNITY_EDITOR
            inited = true;
        }

        // Test methods used only for UnitTesting.
#if UNITY_EDITOR
        public void SetCustomHideFlags(HideFlags testHideFlags) {
            customHideFlags = testHideFlags;
        }

        public void SetLoggingLevel(LoggingLevel testLoggingLevel) {
            loggingLevel = testLoggingLevel;
        }

        public void SetSettings(AudioSourceSetting[] testSettings) {
            settings = testSettings;
        }

        public void TestOnEnable() {
            OnEnable();
        }

        public void TestAwake() {
            m_instace = null;
            Awake();
        }
#endif // UNITY_EDITOR

        private bool IsLoggingEnabled(LoggingLevel loggingLevel) {
            return loggingLevel != LoggingLevel.NONE;
        }
    }
}
