using SecuredSpace.AudioManager.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SecuredSpace.AudioManager.Helper {
    public static class AudioHelper {

        public static AudioError LoadAudioClipFromPath(string path, out AudioClip clip) {
            clip = Resources.Load<AudioClip>(path);
            return clip ? AudioError.OK : AudioError.INVALID_PATH;
        }

        public static void AttachAudioSource(out AudioSource newSource, GameObject newGameObject, AudioClip clip, AudioMixerGroup mixerGroup, bool loop, float volume, float pitch, float spatialBlend, float dopplerLevel, float spreadAngle, AudioRolloffMode rolloffMode, float minDistance, float maxDistance) {
            AddAudioSourceComponent(newGameObject, out newSource);
            newSource.CopyAudioSourceSettings(clip, mixerGroup, loop, volume, pitch, spatialBlend, dopplerLevel, spreadAngle, rolloffMode, minDistance, maxDistance);
        }

        public static void AddAudioSourceComponent(GameObject parent, out AudioSource source) {
            source = parent.AddComponent<AudioSource>();
        }

        public static bool IsSound2D(float spatialBlend) {
            return spatialBlend <= Constants.SPATIAL_BLEND_2D;
        }

        public static float GetLengthOfClip(AudioSourceWrapper wrapper)
        {
            var startTime = wrapper.AudioSourceSettingCache.relativityOfTimeValues ? RelativeValueToSeconds(wrapper.AudioSourceSettingCache.audioClip, wrapper.AudioSourceSettingCache.startTime) : wrapper.AudioSourceSettingCache.startTime;
            var endTime = wrapper.AudioSourceSettingCache.relativityOfTimeValues ? RelativeValueToSeconds(wrapper.AudioSourceSettingCache.audioClip, wrapper.AudioSourceSettingCache.endTime, true) : wrapper.AudioSourceSettingCache.endTime;
            return wrapper.AudioSourceSettingCache.audioClip.length - startTime - endTime;
        }

        public static float GetStartOfClip(AudioSourceWrapper wrapper)
        {
            return wrapper.AudioSourceSettingCache.relativityOfTimeValues ? RelativeValueToSeconds(wrapper.AudioSourceSettingCache.audioClip, wrapper.AudioSourceSettingCache.startTime) : wrapper.AudioSourceSettingCache.startTime;
        }

        public static float RelativeValueToSeconds(AudioClip audioClip, float relativeValue, bool reverse = false)
        {
            return reverse ? audioClip.length - (audioClip.length *  ( relativeValue)) : audioClip.length * relativeValue;
        }

        public static bool IsEndValueValid(float startValue, float endValue) {
            return startValue - endValue >= float.Epsilon || endValue - startValue >= float.Epsilon;
        }

        public static GameObject CreateNewGameObject() {
            return new GameObject();
        }

        public static Transform GetTransform(GameObject gameObject) {
            return gameObject.transform;
        }

        public static void SetTransformParent(Transform child, Transform parent) {
            child.SetParent(parent);
        }

        public static void SetTransformPosition(Transform transform, Vector3 position) {
            transform.position = position;
        }

        public static AudioError ConvertToAudioError(AudioError? error) {
            return error ?? Constants.NULL_AUDIO_ERROR;
        }
    }
}
