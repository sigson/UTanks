using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace GlobalSnowEffect {
    [CustomEditor(typeof(GlobalSnow))]
    public class GlobalSnowEditor : Editor {

        static GUIStyle titleLabelStyle, sectionHeaderStyle, whiteBack;
        static Color titleColor;
        readonly static bool[] expandSection = new bool[6];
        const string SECTION_PREFS = "GlobalSnowExpandSection";
        readonly static string[] sectionNames = new string[] {
                                                "Scene Setup", "Quality", "Coverage", "Appearance", "Features", "Mask Editor"
                                };
        const int SCENE_SETTINGS = 0;
        const int QUALITY_SETTINGS = 1;
        const int COVERAGE_SETTINGS = 2;
        const int APPEARANCE_SETTINGS = 3;
        const int FEATURE_SETTINGS = 4;
        const int MASK_EDITOR = 5;

        SerializedProperty sun, layerMask, deferredCameraEvent, excludedCastShadows, zenithalMask, minimumAltitude, altitudeScatter, minimumAltitudeVegetationOffset;
        SerializedProperty smoothCoverage, coverageResolution, coverageExtension, snowQuality, reliefAmount;
        SerializedProperty defaultExclusionLayer, exclusionDoubleSided, coverageMask, coverageMaskTexture, coverageMaskWorldSize, coverageMaskWorldCenter, groundCoverage, coverageUpdateMethod, coverageDepthDebug;
        SerializedProperty slopeThreshold, slopeSharpness, slopeNoise;
        SerializedProperty occlusion, occlusionIntensity, glitterStrength, maxExposure, debugSnow, showSnowInSceneView;
        SerializedProperty forceSPSR;
        SerializedProperty snowTint;
        SerializedProperty snowNormalsTex, snowNormalsStrength, noiseTex, noiseTexScale;
        SerializedProperty distanceOptimization, detailDistance, distanceSnowColor, distanceIgnoreNormals, distanceIgnoreCoverage, distanceSlopeThreshold;
        SerializedProperty groundCheck, characterController, groundDistance, footprints, footprintsTexture, footprintsDuration, footprintsAutoFPS, footprintsScale, footprintsObscurance;
        SerializedProperty terrainMarks, terrainMarksDuration, terrainMarksDefaultSize, terrainMarksAutoFPS, terrainMarksViewDistance, terrainMarksRoofMinDistance;
        SerializedProperty snowfall, snowfallIntensity, snowfallSpeed, snowfallWind, snowfallDistance, snowfallUseIllumination, snowfallReceiveShadows;
        SerializedProperty snowdustIntensity, snowdustVerticalOffset;
        SerializedProperty cameraFrost, cameraFrostIntensity, cameraFrostSpread, cameraFrostDistortion, cameraFrostTintColor;
        SerializedProperty terrainMarksTextureSize, terrainMarksStepMaxDistance, updateSpeedTree, speedTreeRemoveLeaves, opaqueCutout, billboardCoverage, grassCoverage, fixMaterials;
        SerializedProperty forceForwardRenderingPath, floatingPointNormalsBuffer, showCoverageGizmo, smoothness, snowAmount, altitudeBlending, enableWMAPI;
        SerializedProperty maskEditorEnabled, maskTextureResolution, maskBrushMode, maskBrushWidth, maskBrushFuzziness, maskBrushOpacity;

        Texture2D _headerTexture;
        GlobalSnow gs;
        Texture2D currentMaskTexture;
        bool mouseIsDown;
        Color32[] maskColors;
        bool cameraFrostedChanged;
        Material matDepthPreview;
        MeshRenderer snowMR;
        static float objectSnowerBorder;
        static float objectSnowerOpacity = 0.25f;

        void OnEnable() {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            for (int k = 0; k < expandSection.Length; k++) {
                expandSection[k] = EditorPrefs.GetBool(SECTION_PREFS + k, false);
            }
            _headerTexture = Resources.Load<Texture2D>("GlobalSnow_EditorHeader");
            whiteBack = new GUIStyle();
            whiteBack.normal.background = MakeTex(4, 4, Color.white);

            sun = serializedObject.FindProperty("_sun");
            debugSnow = serializedObject.FindProperty("_debugSnow");
            deferredCameraEvent = serializedObject.FindProperty("_deferredCameraEvent");
            showSnowInSceneView = serializedObject.FindProperty("_showSnowInSceneView");
            forceForwardRenderingPath = serializedObject.FindProperty("_forceForwardRenderingPath");
            floatingPointNormalsBuffer = serializedObject.FindProperty("_floatingPointNormalsBuffer");
            showCoverageGizmo = serializedObject.FindProperty("_showCoverageGizmo");
            smoothness = serializedObject.FindProperty("_smoothness");
            snowAmount = serializedObject.FindProperty("_snowAmount");
#if UNITY_5_5_OR_NEWER
            forceSPSR = serializedObject.FindProperty("_forceSPSR");
#endif
            updateSpeedTree = serializedObject.FindProperty("_updateSpeedTree");
            speedTreeRemoveLeaves = serializedObject.FindProperty("_speedTreeRemoveLeaves");
            fixMaterials = serializedObject.FindProperty("_fixMaterials");
            opaqueCutout = serializedObject.FindProperty("_opaqueCutout");
            defaultExclusionLayer = serializedObject.FindProperty("_defaultExclusionLayer");
            exclusionDoubleSided = serializedObject.FindProperty("_exclusionDoubleSided");
            layerMask = serializedObject.FindProperty("_layerMask");
            excludedCastShadows = serializedObject.FindProperty("_excludedCastShadows");
            zenithalMask = serializedObject.FindProperty("_zenithalMask");
            minimumAltitude = serializedObject.FindProperty("_minimumAltitude");
            altitudeScatter = serializedObject.FindProperty("_altitudeScatter");
            snowTint = serializedObject.FindProperty("_snowTint");
            snowNormalsTex = serializedObject.FindProperty("snowNormalsTex");
            snowNormalsStrength = serializedObject.FindProperty("_snowNormalsStrength");
            noiseTex = serializedObject.FindProperty("noiseTex");
            noiseTexScale = serializedObject.FindProperty("_noiseTexScale");
            altitudeBlending = serializedObject.FindProperty("_altitudeBlending");
            minimumAltitudeVegetationOffset = serializedObject.FindProperty("_minimumAltitudeVegetationOffset");
            distanceOptimization = serializedObject.FindProperty("_distanceOptimization");
            detailDistance = serializedObject.FindProperty("_detailDistance");
            distanceSnowColor = serializedObject.FindProperty("_distanceSnowColor");
            distanceIgnoreNormals = serializedObject.FindProperty("_distanceIgnoreNormals");
            distanceIgnoreCoverage = serializedObject.FindProperty("_distanceIgnoreCoverage");
            distanceSlopeThreshold = serializedObject.FindProperty("_distanceSlopeThreshold");
            smoothCoverage = serializedObject.FindProperty("_smoothCoverage");
            coverageResolution = serializedObject.FindProperty("_coverageResolution");
            coverageExtension = serializedObject.FindProperty("_coverageExtension");
            coverageMask = serializedObject.FindProperty("_coverageMask");
            coverageUpdateMethod = serializedObject.FindProperty("_coverageUpdateMethod");
            coverageDepthDebug = serializedObject.FindProperty("coverageDepthDebug");
            groundCoverage = serializedObject.FindProperty("_groundCoverage");
            coverageMaskTexture = serializedObject.FindProperty("_coverageMaskTexture");
            coverageMaskWorldSize = serializedObject.FindProperty("_coverageMaskWorldSize");
            coverageMaskWorldCenter = serializedObject.FindProperty("_coverageMaskWorldCenter");
            slopeThreshold = serializedObject.FindProperty("_slopeThreshold");
            slopeSharpness = serializedObject.FindProperty("_slopeSharpness");
            slopeNoise = serializedObject.FindProperty("_slopeNoise");
            snowQuality = serializedObject.FindProperty("_snowQuality");
            reliefAmount = serializedObject.FindProperty("_reliefAmount");
            occlusion = serializedObject.FindProperty("_occlusion");
            occlusionIntensity = serializedObject.FindProperty("_occlusionIntensity");
            glitterStrength = serializedObject.FindProperty("_glitterStrength");
            groundCheck = serializedObject.FindProperty("_groundCheck");
            groundDistance = serializedObject.FindProperty("_groundDistance");
            characterController = serializedObject.FindProperty("_characterController");
            footprints = serializedObject.FindProperty("_footprints");
            footprintsTexture = serializedObject.FindProperty("_footprintsTexture");
            footprintsDuration = serializedObject.FindProperty("_footprintsDuration");
            footprintsAutoFPS = serializedObject.FindProperty("_footprintsAutoFPS");
            footprintsScale = serializedObject.FindProperty("_footprintsScale");
            footprintsObscurance = serializedObject.FindProperty("_footprintsObscurance");
            terrainMarks = serializedObject.FindProperty("_terrainMarks");
            terrainMarksDuration = serializedObject.FindProperty("_terrainMarksDuration");
            terrainMarksDefaultSize = serializedObject.FindProperty("_terrainMarksDefaultSize");
            terrainMarksAutoFPS = serializedObject.FindProperty("_terrainMarksAutoFPS");
            terrainMarksViewDistance = serializedObject.FindProperty("_terrainMarksViewDistance");
            terrainMarksTextureSize = serializedObject.FindProperty("terrainMarksTextureSize");
            terrainMarksStepMaxDistance = serializedObject.FindProperty("_terrainMarksStepMaxDistance");
            terrainMarksRoofMinDistance = serializedObject.FindProperty("_terrainMarksRoofMinDistance");
            snowfall = serializedObject.FindProperty("_snowfall");
            snowfallIntensity = serializedObject.FindProperty("_snowfallIntensity");
            snowfallSpeed = serializedObject.FindProperty("_snowfallSpeed");
            snowfallWind = serializedObject.FindProperty("_snowfallWind");
            snowfallDistance = serializedObject.FindProperty("_snowfallDistance");
            snowfallUseIllumination = serializedObject.FindProperty("_snowfallUseIllumination");
            snowfallReceiveShadows = serializedObject.FindProperty("_snowfallReceiveShadows");
            snowdustIntensity = serializedObject.FindProperty("_snowdustIntensity");
            snowdustVerticalOffset = serializedObject.FindProperty("_snowdustVerticalOffset");
            maxExposure = serializedObject.FindProperty("_maxExposure");
            cameraFrost = serializedObject.FindProperty("_cameraFrost");
            cameraFrostIntensity = serializedObject.FindProperty("_cameraFrostIntensity");
            cameraFrostSpread = serializedObject.FindProperty("_cameraFrostSpread");
            cameraFrostDistortion = serializedObject.FindProperty("_cameraFrostDistortion");
            cameraFrostTintColor = serializedObject.FindProperty("_cameraFrostTintColor");
            billboardCoverage = serializedObject.FindProperty("_billboardCoverage");
            grassCoverage = serializedObject.FindProperty("_grassCoverage");
            enableWMAPI = serializedObject.FindProperty("_enableWMAPI");

            maskEditorEnabled = serializedObject.FindProperty("_maskEditorEnabled");
            maskTextureResolution = serializedObject.FindProperty("_maskTextureResolution");
            maskBrushMode = serializedObject.FindProperty("_maskBrushMode");
            maskBrushWidth = serializedObject.FindProperty("_maskBrushWidth");
            maskBrushFuzziness = serializedObject.FindProperty("_maskBrushFuzziness");
            maskBrushOpacity = serializedObject.FindProperty("_maskBrushOpacity");

            gs = (GlobalSnow)target;
        }

        void OnDestroy() {
            // Save folding sections state
            for (int k = 0; k < expandSection.Length; k++) {
                EditorPrefs.SetBool(SECTION_PREFS + k, expandSection[k]);
            }
        }

        public override void OnInspectorGUI() {

            bool forceUpdateProperties = false;

#if UNITY_5_6_OR_NEWER
            serializedObject.UpdateIfRequiredOrScript();
#else
												serializedObject.UpdateIfDirtyOrScript ();
#endif

            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderStyle.SetFoldoutColor();

            if (titleLabelStyle == null) {
                titleLabelStyle = new GUIStyle(EditorStyles.label);
            }
            titleLabelStyle.normal.textColor = titleColor;
            titleLabelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.Separator();
            TextAnchor oldAnchor = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.BeginHorizontal(whiteBack);
            GUILayout.Label(_headerTexture, GUILayout.ExpandWidth(true));
            GUI.skin.label.alignment = oldAnchor;
            GUILayout.EndHorizontal();

            bool deferred = gs.snowCamera != null && gs.snowCamera.actualRenderingPath == RenderingPath.DeferredShading;

            EditorGUILayout.BeginHorizontal();
            expandSection[SCENE_SETTINGS] = EditorGUILayout.Foldout(expandSection[SCENE_SETTINGS], sectionNames[SCENE_SETTINGS], sectionHeaderStyle);
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                if (!EditorUtility.DisplayDialog("Global Snow", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Global Snow, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum")) {
                    Application.OpenURL("https://kronnect.com/support");
                }
            }
            EditorGUILayout.EndHorizontal();
            if (expandSection[SCENE_SETTINGS]) {
                EditorGUILayout.PropertyField(sun, new GUIContent("Sun", "Used to compute basic lighting over snow."));
                if (deferred) {
                    EditorGUILayout.PropertyField(forceForwardRenderingPath, new GUIContent("Forward Rendering", "Forces use of forward rendering path."));
                }
                bool showForwardOptions = true;
                if (deferred && !gs.forceForwardRenderingPath) {
                    EditorGUILayout.PropertyField(deferredCameraEvent, new GUIContent("Deferred Camera Event", "The camera event to which the Command Buffer will be attached. Default setting is BeforeReflections. You can also try BeforeLighting (for example if you disable deferred reflections in Graphics Settings)."));
                    EditorGUILayout.PropertyField(floatingPointNormalsBuffer, new GUIContent("FP Normals Buffer", "Enables floating-point normals buffer (16 bit precision). If disabled, 8-bit precision will be used. Enable if you experiment some snow artifacts in the distance."));
                    if (updateSpeedTree.boolValue || opaqueCutout.boolValue || fixMaterials.boolValue) {
                        EditorGUILayout.HelpBox("Using deferred rendering workflow.\nThe options below should be disabled.", MessageType.Warning);
                    } else {
                        showForwardOptions = false;
                    }
                }
                if (showForwardOptions || gs.forceForwardRenderingPath) {
                    EditorGUILayout.PropertyField(updateSpeedTree, new GUIContent("Update SpeedTree", "Updates SpeedTree materials so they appear covered by snow."));
                    if (!updateSpeedTree.boolValue)
                        GUI.enabled = false;
                    EditorGUILayout.PropertyField(speedTreeRemoveLeaves, new GUIContent("   Remove Leaves", "Uses alternate SpeedTree shader that does not render leaves."));
                    GUI.enabled = true;
                    EditorGUILayout.PropertyField(opaqueCutout, new GUIContent("Opaque Cutout", "Enables alpha cutout on opaque objects. Usually all opaque objects will be covered by snow (unless they belong to an exclusion layer). Some trees or models uses opaque rendertype but they expose holes or transparent geometry. Enabling this feature will automatically cutout the opaque model based on the alpha value of the texture."));
                    EditorGUILayout.PropertyField(fixMaterials, new GUIContent("Fix Materials", "Overrides RenderType tag on those materials that don't specify one and set them to Opaque. Shaders with transparent render queue will be ignored. Use only if you experiment problems with some objects that get clipped by the snow."));
                }
                EditorGUILayout.PropertyField(showSnowInSceneView, new GUIContent("Show In Scene View", "Enabled rendering of snow in the Scene View."));
                if (!deferred || gs.forceForwardRenderingPath) {
                    EditorGUILayout.PropertyField(forceSPSR, new GUIContent("Force Stereo Rendering", "Force per-eye rendering in VR."));
                    EditorGUILayout.PropertyField(debugSnow, new GUIContent("Debug Mode", "Shows snow layer."));
                    EditorGUILayout.HelpBox("Please check the documentation or contact us (by email at contact@kronnect.com or using our support forum on kronnect.com) if you find any issue with custom shaders or objects being clipped by snow rendering.", MessageType.Info);
                }
            }

            EditorGUILayout.Separator();
            expandSection[QUALITY_SETTINGS] = EditorGUILayout.Foldout(expandSection[QUALITY_SETTINGS], sectionNames[QUALITY_SETTINGS], sectionHeaderStyle);
            if (expandSection[QUALITY_SETTINGS]) {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Preset", GUILayout.Width(120));
                if (GUILayout.Button(new GUIContent("Best Quality", "Enables relief, occlusion and better coverage quality."))) {
                    coverageResolution.intValue = 3;
                    smoothCoverage.boolValue = true;
                    snowQuality.intValue = (int)SNOW_QUALITY.ReliefMapping;
                    reliefAmount.floatValue = 0.3f;
                    occlusion.boolValue = true;
                    occlusionIntensity.floatValue = 1.2f;
                    glitterStrength.floatValue = 0.75f;
                    distanceOptimization.boolValue = false;
                }
                if (GUILayout.Button(new GUIContent("Medium", "Enables relief and occlusion, normal coverage quality and medium distance optimization."))) {
                    coverageResolution.intValue = 2;
                    smoothCoverage.boolValue = true;
                    snowQuality.intValue = (int)SNOW_QUALITY.ReliefMapping;
                    reliefAmount.floatValue = 0.3f;
                    occlusion.boolValue = true;
                    occlusionIntensity.floatValue = 1.2f;
                    glitterStrength.floatValue = 0.75f;
                    distanceOptimization.boolValue = true;
                    detailDistance.floatValue = 500f;
                }
                if (!deferred && !gs.forceForwardRenderingPath) {
                    if (GUILayout.Button(new GUIContent("Faster", "Enables flat shading and use a faster coverage computation plus shorter distance optimization."))) {
                        coverageExtension.intValue = 1;
                        coverageResolution.intValue = 1;
                        snowQuality.intValue = (int)SNOW_QUALITY.FlatShading;
                        smoothCoverage.boolValue = false;
                        distanceOptimization.boolValue = true;
                        detailDistance.floatValue = 100f;
                    }
                }
                if (GUILayout.Button(new GUIContent("Fastest", "Uses optimized snow renderer for distance snow on entire scene."))) {
                    coverageExtension.intValue = 1;
                    coverageResolution.intValue = 1;
                    snowQuality.intValue = (int)SNOW_QUALITY.FlatShading;
                    smoothCoverage.boolValue = false;
                    distanceOptimization.boolValue = true;
                    detailDistance.floatValue = 0f;
                }
                EditorGUILayout.EndHorizontal();
                if (!deferred || gs.forceForwardRenderingPath) {
                    EditorGUILayout.PropertyField(distanceOptimization, new GUIContent("Distance Optimization", "Reduces snow detail beyond a given distance from the camera."));
                    if (distanceOptimization.boolValue) {
                        EditorGUILayout.PropertyField(detailDistance, new GUIContent("   Detail Distance", "Beyond this limit the snow will be rendered in a simplified way, reducing GPU usage."));
                        EditorGUILayout.PropertyField(distanceIgnoreCoverage, new GUIContent("   Ignore Coverage", "Ignore coverage computation checking while rendering distant snow."));
                        EditorGUILayout.PropertyField(distanceIgnoreNormals, new GUIContent("   Ignore Normals", "Ignore surface normal on distance snow (makes distance snow rendering faster)."));
                        if (!distanceIgnoreNormals.boolValue) {
                            EditorGUILayout.PropertyField(distanceSlopeThreshold, new GUIContent("   Slope Threshold", "Custom slope threshold for distance snow."));
                        }
                        EditorGUILayout.PropertyField(distanceSnowColor, new GUIContent("   Tint", "Snow color on the distance."));
                    }
                }
            }

            EditorGUILayout.Separator();
            expandSection[COVERAGE_SETTINGS] = EditorGUILayout.Foldout(expandSection[COVERAGE_SETTINGS], sectionNames[COVERAGE_SETTINGS], sectionHeaderStyle);
            if (expandSection[COVERAGE_SETTINGS]) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(layerMask, new GUIContent("Layer Mask", "Optionally exclude some objects from being covered by snow. Alternatively you can add the script GlobalSnowIgnoreCoverage to any number of gameobjects to be exluded without changing their layer."));
                if (GUILayout.Button("Refresh", GUILayout.Width(80))) forceUpdateProperties = true;
                EditorGUILayout.EndHorizontal();
                if (!deferred || gs.forceForwardRenderingPath) {
                    EditorGUILayout.PropertyField(excludedCastShadows, new GUIContent("Excluded Cast Shadows", "If set to false, excluded objects from the layer mask won't cast shadows on snow (improves performance)."));
                }
                EditorGUILayout.PropertyField(zenithalMask, new GUIContent("Zenithal Mask", "Specify which objects are considered for top-down occlusion. Objects on top prevent snow on objects beneath them. Make sure to exclude any particle system to improve performance and avoid coverage issues."));
                EditorGUILayout.PropertyField(defaultExclusionLayer, new GUIContent("Ref Exclusion Layer", "This is the layer used to exclude temporary objects marked as not covered by snow. Use a layer number that you don't use."));
                EditorGUILayout.PropertyField(exclusionDoubleSided, new GUIContent("Exclusion Double Sided", "Enable this option when excluding double sided objects from snow."));
                EditorGUILayout.PropertyField(minimumAltitude, new GUIContent("Minimum Altitude", "Specify snow level."));
#if WORLDAPI_PRESENT
																if (enableWMAPI.boolValue) {
																EditorGUILayout.HelpBox("Minimum altitude controlled by World Manager API.", MessageType.Info);
																}
#endif
                string t1, t2;
                if (deferred && !gs.forceForwardRenderingPath) {
                    EditorGUILayout.LabelField("Billboard Only Options");
                    t1 = "   Tree Coverage";
                    t2 = "   Grass Coverage";
                } else {
                    t1 = "Tree Coverage";
                    t2 = "Grass Coverage";
                }
                EditorGUILayout.PropertyField(minimumAltitudeVegetationOffset, new GUIContent("   Altitude Offset", "Applies a vertical offset to the minimum altitude only to grass and trees. This option is useful to avoid showing full grass or trees covered with snow when altitude scattered is used and there's little snow on ground which causes unnatural visuals."));
                EditorGUILayout.PropertyField(billboardCoverage, new GUIContent(t1, "Amount of snow over tree billboards."));
                EditorGUILayout.PropertyField(grassCoverage, new GUIContent(t2, "Amount of snow over grass objects."));
                EditorGUILayout.PropertyField(groundCoverage, new GUIContent("Ground Coverage", "Increase or reduce snow coverage under opaque objects."));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(coverageExtension, new GUIContent("Coverage Extension", "Area included in the snow coverage. 1 = 512 meters, 2 = 1024 meters. Note that greater extension reduces quality."));
                GUILayout.Label(Mathf.Pow(2, 8f + coverageExtension.intValue).ToString(), GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(showCoverageGizmo, new GUIContent("Show Coverage Gizmo", "Shows a rectangle in SceneView which encloses the coverage area."));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(coverageResolution, new GUIContent("Coverage Quality", "Resolution of the coverage texture (1=512 pixels, 2=1024 pixels, 3=2048 pixels)."));
                GUILayout.Label(Mathf.Pow(2, 8f + coverageResolution.intValue).ToString(), GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(smoothCoverage, new GUIContent("Smooth Coverage", "Increase snow converage quality."));
                EditorGUILayout.PropertyField(coverageMask, new GUIContent("Coverage Mask", "Uses alpha channel of a custom texture as snow coverage mask."));
                if (coverageMask.boolValue) {
                    EditorGUILayout.PropertyField(coverageMaskTexture, new GUIContent("   Texture (A)", "Snow coverage mask. A value of alpha of zero means no snow."));
                    EditorGUILayout.PropertyField(coverageMaskWorldSize, new GUIContent("   World Size", "Mapping of the texture against the world in world units. Usually this should match terrain size."));
                    EditorGUILayout.PropertyField(coverageMaskWorldCenter, new GUIContent("   World Center", "Mapping of the texture center against the world in world units. Use this as an offset to apply coverage mask over a certain area."));
                }
                EditorGUILayout.PropertyField(coverageUpdateMethod, new GUIContent("Coverage Update", "Specifies when the snow coverage needs to be computed. Every frame, Discrete (every 50 meters of player movement), or Manual (requires manual call to UpdateSnowCoverage function)."));
                if (coverageUpdateMethod.intValue == (int)SNOW_COVERAGE_UPDATE_METHOD.Disabled) {
                    string optionsFile = deferred ? "GlobalSnowDeferredOptions.cginc" : "GlobalSnowForwardOptions.cginc";
                    EditorGUILayout.HelpBox("Additionally edit " + optionsFile + " and comment out the USE_ZENITHAL_DEPTH line.", MessageType.Info);
                } else {
                    EditorGUILayout.PropertyField(coverageDepthDebug, new GUIContent("Show Coverage Depth", "Shows zenital depth texture."));
                    if (coverageDepthDebug.boolValue) {
                        if (matDepthPreview == null) {
                            matDepthPreview = new Material(Shader.Find("GlobalSnow/Editor/DepthTexPreview"));
                        }
                        Rect space = EditorGUILayout.BeginVertical();
                        GUILayout.Space(EditorGUIUtility.currentViewWidth * 0.9f);
                        EditorGUILayout.EndVertical();
                        EditorGUI.DrawPreviewTexture(space, Texture2D.whiteTexture, matDepthPreview, ScaleMode.ScaleToFit);
                    }

                    if (Application.isPlaying && GUILayout.Button("Update Coverage Now")) {
                        forceUpdateProperties = true;
                    }
                }
            }

            EditorGUILayout.Separator();
            expandSection[APPEARANCE_SETTINGS] = EditorGUILayout.Foldout(expandSection[APPEARANCE_SETTINGS], sectionNames[APPEARANCE_SETTINGS], sectionHeaderStyle);

            if (expandSection[APPEARANCE_SETTINGS]) {
                if (!deferred && detailDistance.floatValue <= 0f) {
                    EditorGUILayout.HelpBox("Distance Optimization is enabled for the entire scene.", MessageType.Info);
                    GUI.enabled = false;
                }
                EditorGUILayout.PropertyField(snowAmount, new GUIContent("Snow Amount", "Global snow threshold."));

                EditorGUILayout.PropertyField(snowQuality, new GUIContent("Snow Complexity", "Choose the rendering scheme for the snow."));
                if (snowQuality.intValue == (int)SNOW_QUALITY.ReliefMapping) {
                    EditorGUILayout.PropertyField(reliefAmount, new GUIContent("   Relief Amount", "Relief intensity."));
                    EditorGUILayout.PropertyField(occlusion, new GUIContent("   Occlusion", "Enables occlusion effect."));
                    if (occlusion.boolValue) {
                        EditorGUILayout.PropertyField(occlusionIntensity, new GUIContent("      Intensity", "Occlusion intensity."));
                    }
                }
                if (snowQuality.intValue != (int)SNOW_QUALITY.FlatShading) {
                    EditorGUILayout.PropertyField(glitterStrength, new GUIContent("   Glitter Strength", "Snow glitter intensity. Set to zero to disable."));
                }

                EditorGUILayout.LabelField("Slope Options (DX11 only)");
                EditorGUILayout.PropertyField(slopeThreshold, new GUIContent("   Threshold", "The maximum slope where snow can accumulate."));
                EditorGUILayout.PropertyField(slopeSharpness, new GUIContent("   Sharpness", "The sharpness (or smoothness) of the snow at terrain borders."));
                EditorGUILayout.PropertyField(slopeNoise, new GUIContent("   Noise", "Amount of randomization to fill the transient area between low and high slope (determined by slope threshold)."));
                GUI.enabled = true;
                EditorGUILayout.PropertyField(altitudeScatter, new GUIContent("Altitude Scatter", "Defines random snow scattering around minimum altitude level."));
                EditorGUILayout.PropertyField(altitudeBlending, new GUIContent("Altitude Blending", "Defines vertical gradient length for snow blending."));
                EditorGUILayout.PropertyField(snowTint, new GUIContent("Snow Tint", "Snow tint color."));
                if (deferred) {
                    EditorGUILayout.PropertyField(smoothness, new GUIContent("Roughness", "Snow PBR roughness."));
                }
                EditorGUILayout.PropertyField(maxExposure, new GUIContent("Max Exposure", "Controls maximum snow brightness."));

                if (snowQuality.intValue != (int)SNOW_QUALITY.FlatShading) {
                    EditorGUILayout.ObjectField(snowNormalsTex, new GUIContent("Snow Normals Texture"));
                    EditorGUILayout.PropertyField(snowNormalsStrength, new GUIContent("Snow Normals Strength"));
                    EditorGUILayout.ObjectField(noiseTex, new GUIContent("Noise Texture"));
                    EditorGUILayout.PropertyField(noiseTexScale, new GUIContent("Noise Texture Scale"));
                }
            }

            EditorGUILayout.Separator();
            expandSection[FEATURE_SETTINGS] = EditorGUILayout.Foldout(expandSection[FEATURE_SETTINGS], sectionNames[FEATURE_SETTINGS], sectionHeaderStyle);

            if (expandSection[FEATURE_SETTINGS]) {
                if (footprints.boolValue || terrainMarks.boolValue) {
                    if (!deferred && distanceOptimization.boolValue && detailDistance.floatValue <= 0) {
                        EditorGUILayout.HelpBox("Footprints and Terrain Marks are not supported with Fastest quality setting.", MessageType.Info);
                        GUI.enabled = false;
                    } else if (coverageUpdateMethod.intValue == (int)SNOW_COVERAGE_UPDATE_METHOD.Disabled) {
                        EditorGUILayout.HelpBox("Coverage Update is disabled. Footprints and Terrain Marks may not render correctly.", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(footprints, new GUIContent("Footprints", "Enable footprints on snow surface."));
                if (footprints.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(footprintsAutoFPS, new GUIContent("Active", "Add automatic footprints when camera moves (use only in FPS camera). If disabled, no new footprints will be added."));
                    EditorGUILayout.PropertyField(groundCheck, new GUIContent("Ground Check", "How to detect if player is on ground."));
                    if (groundCheck.intValue == (int)GROUND_CHECK.CharacterController) {
                        EditorGUILayout.PropertyField(characterController, new GUIContent("Controller", "The character controller."));
                    } else if (groundCheck.intValue == (int)GROUND_CHECK.RayCast) {
                        EditorGUILayout.PropertyField(groundDistance, new GUIContent("Max Distance", "Max distance to the ground."));
                    }
                    EditorGUILayout.PropertyField(footprintsTexture, new GUIContent("Texture", "Texture for the footprint stamp."), true);
                    EditorGUILayout.PropertyField(footprintsDuration, new GUIContent("Duration", "Duration of the footprints in seconds before fading out completely."));
                    EditorGUILayout.PropertyField(footprintsScale, new GUIContent("Scale", "Increase to reduce the size of the footprints."));
                    EditorGUILayout.PropertyField(footprintsObscurance, new GUIContent("Obscurance", "Makes the footprints darker."));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(terrainMarks, new GUIContent("Terrain Marks", "Enable terrain marks based on collisions."));
                if (terrainMarks.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(terrainMarksDuration, new GUIContent("Duration", "Duration of the terrain marks in seconds before fading out completely."));
                    EditorGUILayout.PropertyField(terrainMarksDefaultSize, new GUIContent("Default Size", "Default size for a marks produced by automatic collisions. You can call MarkSnowAt() method to specify a custom size."));
                    EditorGUILayout.PropertyField(terrainMarksTextureSize, new GUIContent("Extents", "Size of the internal texture that holds terrain mark data."));
                    EditorGUILayout.PropertyField(terrainMarksViewDistance, new GUIContent("View Distance", "Maximum terrain marks render distance to camera. Reduce to avoid marks repetitions or increase extents parameter."));
                    EditorGUILayout.PropertyField(terrainMarksAutoFPS, new GUIContent("FPS Marks", "Add automatic terrain mark when camera moves (use only in FPS camera)"));
                    EditorGUILayout.PropertyField(terrainMarksStepMaxDistance, new GUIContent("Max Step", "Maximum object distance between positions in 2 consecutive frames. If an object changes position and the new position is further than this value, no trail will be left behind."));
                    EditorGUILayout.PropertyField(terrainMarksRoofMinDistance, new GUIContent("Min Roof Distance", "Minimum distance from stamp position to roof. This setting allows you to fine-control when terrain marks should be placed under a roof."));
                    EditorGUI.indentLevel--;
                }
                GUI.enabled = true;
                EditorGUILayout.PropertyField(snowfall, new GUIContent("Snowfall", "Enable snowfall."));
                if (snowfall.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(snowfallIntensity, new GUIContent("Intensity", "Snowflakes emission rate."));
#if WORLDAPI_PRESENT
																				if (enableWMAPI.boolValue) {
																				EditorGUILayout.HelpBox("Snow fall intensity controlled by World Manager API.", MessageType.Info);
																				}
#endif
                    EditorGUILayout.PropertyField(snowfallSpeed, new GUIContent("Speed", "Snowfall speed."));
                    EditorGUILayout.PropertyField(snowfallWind, new GUIContent("Wind", "Horizontal wind speed."));
                    EditorGUILayout.PropertyField(snowfallDistance, new GUIContent("Emission Distance", "Emission box scale. Reduce to produce more dense snowfall."));
                    EditorGUILayout.PropertyField(snowfallReceiveShadows, new GUIContent("Receive Shadows", "If enabled, snow particles will receive and cast shadows (affected by illumination in general)."));
                    if (snowfallReceiveShadows.boolValue) {
                        GUI.enabled = false;
                    }
                    EditorGUILayout.PropertyField(snowfallUseIllumination, new GUIContent("Use Illumination", "If enabled, snow particles will be affected by light."));
                    GUI.enabled = true;
                    EditorGUILayout.HelpBox("You can customize particle system prefab located in GlobalSnow/Resources/Prefab folder.", MessageType.Info);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(snowdustIntensity, new GUIContent("Snow Dust", "Snow dust intensity."));
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(snowdustVerticalOffset, new GUIContent("Vertical Offset", "Vertical offset for the emission volume with respect to the camera altitude."));
                EditorGUI.indentLevel--;
                if (snowdustIntensity.floatValue > 0) {
                    EditorGUILayout.HelpBox("Customize additional options like gravity or collision of snow dust in the SnowDustSystem prefab inside GlobalSnow/Resources/Common/Prefabs folder.", MessageType.Info);
                }

                bool prevBool = cameraFrost.boolValue;
                EditorGUILayout.PropertyField(cameraFrost, new GUIContent("Camera Frost", "Enable camera frost effect."));
                if (cameraFrost.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cameraFrostIntensity, new GUIContent("Intensity", "Intensity of camera frost effect."));
                    EditorGUILayout.PropertyField(cameraFrostSpread, new GUIContent("Spread", "Amplitude of camera frost effect."));
                    EditorGUILayout.PropertyField(cameraFrostDistortion, new GUIContent("Distortion", "Distortion magnitude."));
                    EditorGUILayout.PropertyField(cameraFrostTintColor, new GUIContent("Tint Color", "Tinting color for the frost effect."));
                    EditorGUI.indentLevel--;
                }
                if (prevBool != cameraFrost.boolValue) cameraFrostedChanged = true;
                EditorGUILayout.PropertyField(enableWMAPI, new GUIContent("Enable WMAPI", "Enables integration with World Manager API."));
#if !WORLDAPI_PRESENT
                if (enableWMAPI.boolValue) {
                    EditorGUILayout.HelpBox("World Manager API components are not installed. For more information, please visit: https://github.com/adamgoodrich/WorldManager", MessageType.Warning);
                }
#endif
            }
            EditorGUILayout.Separator();


            if (targets.Length == 1) {
                expandSection[MASK_EDITOR] = EditorGUILayout.Foldout(expandSection[MASK_EDITOR], sectionNames[MASK_EDITOR], sectionHeaderStyle);
                if (expandSection[MASK_EDITOR]) { 
                EditorGUILayout.PropertyField(maskEditorEnabled, new GUIContent("Enable Editor", "Activates terrain brush to paint/remove snow intensity at custom locations."));
                    if (maskEditorEnabled.boolValue) {
                        if (!coverageMask.boolValue) {
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            EditorGUILayout.LabelField("Coverage Mask feature is disabled. Enable it?");
                            if (GUILayout.Button("Enable Coverage Mask")) coverageMask.boolValue = true;
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Separator();
                            GUI.enabled = false;
                        }
                        EditorGUILayout.PropertyField(coverageMaskTexture, new GUIContent("Current Mask", "Snow coverage mask. A value of alpha of zero means no snow."));
                        Texture2D tex = (Texture2D)coverageMaskTexture.objectReferenceValue;
                        if (tex != null) {
                            if (currentMaskTexture != tex) {
                                currentMaskTexture = tex;
                                maskColors = currentMaskTexture.GetPixels32();
                                maskTextureResolution.intValue = currentMaskTexture.width;

                            }
                            EditorGUILayout.LabelField("   Texture Size", currentMaskTexture.width.ToString());
                            EditorGUILayout.LabelField("   Texture Path", AssetDatabase.GetAssetPath(currentMaskTexture));
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.PropertyField(maskTextureResolution, new GUIContent("Resolution", "Resolution of the mask texture. Higher resolution allows more detail but it can be slower."));
                        if (GUILayout.Button("Create New Mask Texture")) {
                            if (EditorUtility.DisplayDialog("Create Mask Texture", "A texture asset will be created with a size of " + maskTextureResolution.intValue + "x" + maskTextureResolution.intValue + ".\n\nContinue?", "Ok", "Cancel")) {
                                CreateNewMaskTexture();
                            }
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("World Texture Mapping");
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(coverageMaskWorldSize, new GUIContent("World Size", "Mapping of the texture against the world in world units. Usually this should match terrain size."));
                        EditorGUILayout.PropertyField(coverageMaskWorldCenter, new GUIContent("World Center", "Mapping of the texture center against the world in world units. Use this as an offset to apply coverage mask over a certain area."));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.PropertyField(maskBrushMode, new GUIContent("Brush Mode", "Select brush operation mode."));
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(maskBrushWidth, new GUIContent("Width", "Width of the snow editor brush."));
                        EditorGUILayout.PropertyField(maskBrushFuzziness, new GUIContent("Fuzziness", "Solid vs spray brush."));
                        EditorGUILayout.PropertyField(maskBrushOpacity, new GUIContent("Opacity", "Stroke opacity."));
                        EditorGUILayout.BeginHorizontal();
                        if (currentMaskTexture == null) GUI.enabled = false;
                        if (GUILayout.Button("Fill Mask")) {
                            FillMaskTexture(255);
                        }
                        if (GUILayout.Button("Clear Mask")) {
                            FillMaskTexture(0);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Separator();
                        snowMR = (MeshRenderer)EditorGUILayout.ObjectField("Object Snower", snowMR, typeof(MeshRenderer), true);
                        GUI.enabled = snowMR != null;
                        EditorGUI.indentLevel++;
                        objectSnowerBorder = EditorGUILayout.FloatField("Padding", objectSnowerBorder);
                        objectSnowerOpacity = EditorGUILayout.Slider("Opacity", objectSnowerOpacity, 0, 1f);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Cover With Snow")) {
                            FillObjectWithSnow(255, objectSnowerOpacity, objectSnowerBorder);
                        }
                        if (GUILayout.Button("Clear Snow")) {
                            FillObjectWithSnow(0, objectSnowerOpacity, objectSnowerBorder);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                        GUI.enabled = true;
                        EditorGUILayout.Separator();
                    }
                }
            }

            EditorGUILayout.Separator();

            if (serializedObject.ApplyModifiedProperties() || forceUpdateProperties) {
                gs.UpdateProperties();
                EditorUtility.SetDirty(gs);
                if (cameraFrostedChanged) {
                    cameraFrostedChanged = false;
                    GUIUtility.ExitGUI();
                }
            }
        }

        Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.hideFlags = HideFlags.DontSave;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


        private void OnSceneGUI() {
            Event e = Event.current;
            if (gs == null || !maskEditorEnabled.boolValue || e == null) return;

            Camera sceneCamera = null;
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null) sceneCamera = sceneView.camera;
            if (sceneCamera == null) return;

            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x < 0 || mousePos.x > sceneCamera.pixelWidth || mousePos.y < 0 || mousePos.y > sceneCamera.pixelHeight) return;

            Selection.activeGameObject = gs.gameObject;
            gs.UpdateProperties();

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                float handleSize = HandleUtility.GetHandleSize(hitInfo.point);
                Handles.color = new Color(0, 0, 1, 0.5f);
                Handles.SphereHandleCap(0, hitInfo.point, Quaternion.identity, handleSize, EventType.Repaint);
                HandleUtility.Repaint();

                if (e.isMouse && e.button == 0) {
                    var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    var eventType = e.GetTypeForControl(controlID);

                    if (eventType == EventType.MouseDown) {
                        GUIUtility.hotControl = controlID;
                        mouseIsDown = true;
                        PaintOnMaskPosition(hitInfo.point);
                    } else if (eventType == EventType.MouseUp) {
                        GUIUtility.hotControl = controlID;
                        mouseIsDown = false;
                    }

                    if (mouseIsDown && eventType == EventType.MouseDrag) {
                        GUIUtility.hotControl = controlID;
                        PaintOnMaskPosition(hitInfo.point);
                    }
                }
            }
        }


        #region Mask Texture support functions
        private void CreateNewMaskTexture() {
            int res = Mathf.Clamp(maskTextureResolution.intValue, 256, 8192);
            Texture2D tex = new Texture2D(res, res, TextureFormat.Alpha8, false, true);
            tex.wrapMode = TextureWrapMode.Clamp;
            int length = res * res;
            maskColors = new Color32[length];
            currentMaskTexture = tex;
            FillMaskTexture(255);
            AssetDatabase.CreateAsset(tex, "Assets/SnowMaskTexture.asset");
            AssetDatabase.SaveAssets();
            gs.coverageMask = true;
            gs.coverageMaskTexture = tex;
        }

        private void PaintOnMaskPosition(Vector3 pos) {
            // Get texture location
            if (currentMaskTexture == null) {
                EditorUtility.DisplayDialog("Global Snow Mask Editor", "Create or assign a coverage mask texture in Global Snow inspector before painting!", "Ok");
                return;
            }
            float x = (pos.x - coverageMaskWorldCenter.vector3Value.x) / coverageMaskWorldSize.vector3Value.x + 0.5f;
            float z = (pos.z - coverageMaskWorldCenter.vector3Value.z) / coverageMaskWorldSize.vector3Value.z + 0.5f;
            int tx = Mathf.Clamp((int)(x * currentMaskTexture.width), 0, currentMaskTexture.width - 1);
            int ty = Mathf.Clamp((int)(z * currentMaskTexture.height), 0, currentMaskTexture.height - 1);
            int th = currentMaskTexture.height;
            int tw = currentMaskTexture.width;

            // Prepare brush data
            int brushSize = Mathf.FloorToInt(currentMaskTexture.width * maskBrushWidth.intValue / coverageMaskWorldSize.vector3Value.x);
            byte color = maskBrushMode.intValue == (int)MASK_TEXTURE_BRUSH_MODE.AddSnow ? (byte)255 : (byte)0;
            float brushOpacity = 1f - maskBrushOpacity.floatValue * 0.2f;
            float fuzziness = 1.1f - maskBrushFuzziness.floatValue;
            byte colort = (byte)(color * (1f - brushOpacity));
            float radiusSqr = brushSize * brushSize;
            // Paint!
            for (int j = ty - brushSize; j < ty + brushSize; j++) {
                if (j < 0) continue; else if (j >= th) break;
                int jj = j * currentMaskTexture.width;
                int dj = (j - ty) * (j - ty);
                for (int k = tx - brushSize; k < tx + brushSize; k++) {
                    if (k < 0) continue; else if (k >= tw) break;
                    int distSqr = dj + (k - tx) * (k - tx);
                    float op = distSqr / radiusSqr;
                    float threshold = Random.value;
                    if (op <= 1f && threshold * op < fuzziness) {
                        maskColors[jj + k].a = (byte)(colort + maskColors[jj + k].a * brushOpacity);
                    }
                }
            }
            currentMaskTexture.SetPixels32(maskColors);
            currentMaskTexture.Apply(false);
            EditorUtility.SetDirty(currentMaskTexture);
        }

        void FillMaskTexture(byte value) {
            Color32 opaque = new Color32(255, 255, 255, value);
            for (int k = 0; k < maskColors.Length; k++) {
                maskColors[k] = opaque;
            }
            currentMaskTexture.SetPixels32(maskColors);
            currentMaskTexture.Apply();
            EditorUtility.SetDirty(currentMaskTexture);
        }

        void FillObjectWithSnow(byte value, float opacity, float border) {

            if (snowMR == null) return;
            int res = maskTextureResolution.intValue;
            Vector3 wpos = new Vector3();
            Vector3 worldSize = coverageMaskWorldSize.vector3Value;
            Vector3 worldCenter = coverageMaskWorldCenter.vector3Value;
            Vector3 min = snowMR.bounds.min;
            Vector3 max = snowMR.bounds.max;
            // Get triangle info
            MeshFilter mf = snowMR.GetComponent<MeshFilter>();
            if (mf == null) {
                Debug.LogError("No MeshFilter found on this object.");
                return;
            }
            if (mf.sharedMesh == null) {
                Debug.LogError("No Mesh found on this object.");
                return;
            }
            if (mf.sharedMesh.GetTopology(0) != MeshTopology.Triangles) {
                Debug.LogError("Only triangle topology is supported by this tool.");
                return;
            }
            int[] indices = mf.sharedMesh.triangles;
            Vector3[] vertices = mf.sharedMesh.vertices;

            for (int z = 0; z < res; z++) {
                int zz = z * res;
                wpos.z = (((z + 0.5f) / res) - 0.5f) * worldSize.z + worldCenter.z;
                if (wpos.z < min.z || wpos.z > max.z) continue;
                for (int x = 0; x < res; x++) {
                    wpos.x = (((x + 0.5f) / res) - 0.5f) * worldSize.x + worldCenter.x;
                    if (wpos.x < min.x || wpos.x > max.x) continue;

                    // Check if any triangle contains this position
                    for (int i = 0; i < indices.Length; i += 3) {
                        Vector3 v0 = snowMR.transform.TransformPoint(vertices[indices[i]]);
                        Vector3 v1 = snowMR.transform.TransformPoint(vertices[indices[i + 1]]);
                        Vector3 v2 = snowMR.transform.TransformPoint(vertices[indices[i + 2]]);
                        if (border > 0) {
                            Vector3 c = (v0 + v1 + v2) / 3f;
                            c.y = 0;
                            Vector3 w0 = new Vector3(v0.x, 0, v0.z);
                            Vector3 w1 = new Vector3(v1.x, 0, v1.z);
                            Vector3 w2 = new Vector3(v2.x, 0, v2.z);
                            v0 += (w0 - c).normalized * border;
                            v1 += (w1 - c).normalized * border;
                            v2 += (w2 - c).normalized * border;
                        }
                        if (PointInTriangle(wpos, v0, v1, v2)) {
                            byte v = (byte)(value * opacity + maskColors[zz + x].a * (1f - opacity));
                            maskColors[zz + x].a = v;
                            break;
                        }
                    }
                }
            }
            currentMaskTexture.SetPixels32(maskColors);
            currentMaskTexture.Apply();
            EditorUtility.SetDirty(currentMaskTexture);
        }


        float sign(Vector3 p1, Vector3 p2, Vector3 p3) {
            return (p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.z - p3.z);
        }

        bool PointInTriangle(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3) {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        #endregion


    }

}
