using System;
using UnityEngine;

namespace GlobalSnowEffect {

    [Serializable]
    public struct GlobalSnowProfile {
        [Range(0f, 2f)]
        public float snowAmount;

        public float minimumAltitude;

        [Range(0, 250f)]
        public float altitudeScatter;

        [Range(0, 500f)]
        public float altitudeBlending;

        [Range(0, 0.5f)]
        public float groundCoverage;

        [Range(0, 1f)]
        public float slopeThreshold;

        [Range(0, 1f)]
        public float slopeSharpness;

        [Range(0, 1f)]
        public float slopeNoise;

    }

    [ExecuteInEditMode]
    public class GlobalSnowLerp : MonoBehaviour {

        [Tooltip("Assign target Global Snow component that will be affected by this volume.")]
        public GlobalSnow targetGlobalSnow;

        [Range(0,1)]
        public float transition;

        public KeyCode leftKey, rightKey;
        public float keySpeed = 1f;

        public GlobalSnowProfile profile1;
        public GlobalSnowProfile profile2;


        void OnEnable() {
            if (targetGlobalSnow == null) {
                targetGlobalSnow = GlobalSnow.instance;
            }
            if (targetGlobalSnow != null && profile1.snowAmount == 0 && profile1.minimumAltitude == 0 && profile1.groundCoverage == 0) {
                // grab default values from current configuration
                profile1.snowAmount = targetGlobalSnow.snowAmount;
                profile1.minimumAltitude = targetGlobalSnow.minimumAltitude;
                profile1.altitudeScatter = targetGlobalSnow.altitudeScatter;
                profile1.altitudeBlending = targetGlobalSnow.altitudeBlending;
                profile1.groundCoverage = targetGlobalSnow.groundCoverage;
                profile1.slopeThreshold = targetGlobalSnow.slopeThreshold;
                profile1.slopeSharpness = targetGlobalSnow.slopeSharpness;
                profile1.slopeNoise = targetGlobalSnow.slopeNoise;
            }
        }

        private void OnValidate() {
            UpdateSettings();
        }

        public void UpdateSettings() {
            if (targetGlobalSnow == null) return;

            targetGlobalSnow.snowAmount = Mathf.Lerp(profile1.snowAmount, profile2.snowAmount, transition);
            targetGlobalSnow.minimumAltitude = Mathf.Lerp(profile1.minimumAltitude, profile2.minimumAltitude, transition);
            targetGlobalSnow.altitudeScatter = Mathf.Lerp(profile1.altitudeScatter, profile2.altitudeScatter, transition);
            targetGlobalSnow.altitudeBlending = Mathf.Lerp(profile1.altitudeBlending, profile2.altitudeBlending, transition);
            targetGlobalSnow.groundCoverage = Mathf.Lerp(profile1.groundCoverage, profile2.groundCoverage, transition);
            targetGlobalSnow.slopeThreshold = Mathf.Lerp(profile1.slopeThreshold, profile2.slopeThreshold, transition);
            targetGlobalSnow.slopeSharpness = Mathf.Lerp(profile1.slopeSharpness, profile2.slopeSharpness, transition);
            targetGlobalSnow.slopeNoise = Mathf.Lerp(profile1.slopeNoise, profile2.slopeNoise, transition);
        }

        private void Update() {
            if (Input.GetKey(leftKey)) {
                transition -= keySpeed * Time.deltaTime;
                if (transition < 0) transition = 0;
                UpdateSettings();
            }
            if (Input.GetKey(rightKey)) {
                transition += keySpeed * Time.deltaTime;
                if (transition < 1) transition = 1f;
                UpdateSettings();
            }

        }

    }

}