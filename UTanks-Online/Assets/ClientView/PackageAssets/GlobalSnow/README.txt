**************************************
*            GLOBAL SNOW             *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


How to use this asset
---------------------
Firstly, you should run the Demo Scene provided to get an idea of the overall functionality.
Later, please read the documentation and experiment with the system.

Hint: to quick start using the asset just add GlobalSnow script to your camera. It will show up in the Game view. Customize it using the custom inspector.


Documentation/API reference
---------------------------
The PDF is located in the Documentation folder. It contains instructions on how to use this asset as well as a useful Frequent Asked Question section.


Support
-------
Please read the documentation PDF and browse/play with the demo scene and sample source code included before contacting us for support :-)

* Support: contact@kronnect.com
* Website-Forum: http://kronnect.com
* Twitter: @Kronnect


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Global Snow will be eventually available on the Asset Store.


Version history
---------------

v6.8
- Added Snow Dust effect (under Features section) - see: https://youtu.be/aOf4UGGVZWA

v6.7.2
- [Fix] Fixed coverage flickering when update interval is set to 'Every Frame'

v6.7.1
- Added "Ignore This Collider" option to Global Snow Extra Collider Info script.
- New tutorial for humanoid characters: https://youtu.be/D1eGlELeW0U

v6.7
- Improved snow marks. New "isFootprint" parameter added to Global Snow Extra Collider Info script (check documentation for details)

v6.6.1
- Minimum Unity version supported 2019.4.13
- Support for fast domain reload
- [Fix] Fixes related to GlobalSnowIgnoreCoverage handling

v6.6
- Added "Exclusion Double Sided" option to support double-sided objects exclusion from snow rendering
- Added "Snow Normals Strength" option

v6.5.1
- [Fix] Fixed ignore coverage script issue in forward rendering path

v6.5
- Added "Noise Texture Scale" option to inspector

v6.4
- Added snow textures to inspector for additional customization options

v6.3.3
- Improvements to exclusion options related to character LODs

V6.3.2
- A few internal optimizations to save some performance in Editor
- "Show in Scene View" option in inspector is now disabled by default

V6.3.1
- [Fix] Fixed XR matrix projection issue

V6.3
- Added "Camera Frost Tint Color" option
- [Fix] Removed harmless console warning in Unity 2020.1

V6.2 30/Apr/2021
- Added "Exclusion Cut-Off" option to Global Snow Ignore Coverage script (only used in deferred)

V6.1.1
- [Fix] Fixed an issue with LOD groups which caused incorrect snow coverage

V6.1
- Added a solution to exclude snow on terrain in deferred rendering path (check page 20 of documentation for details)

V6.0.1
- [Fix] Fixed an issue with point lights in forward rendering path

V6.0
- New Global Snow lerp script
- General improvements and fixes

V5.9.1
- Added "Minimum Roof Distance" which allow finer control when objects can stamp terrain marks under other structures
- Minor shader optimization

V5.9
- API: added FootprintAt method to render footprints at custom positions

V5.8.1
- [Fix] Fixed selection bug issue in SceneView on Unity 2019.3

V5.8 Changes:
- New shader option to exclude FPS weapons from snow (check manual or edit GlobalSnowDeferredOptions.cginc file)

V5.7 Changes:
- Wheel colliders are now automatically detected from the Global Snow Collider Extra Info script (attach this script to the root gameobject of a vehicle)
- Added custom shaders for moving objects (GlobalSnow/Moving Object Snow/Overlay and Opaque)

V5.6 Changes:
- Optimized performance of command buffer rebuild in deferred rendering path

V5.5.1 Changes:
- [Fix] Fixed incorrect rendering of excluded objects when flat shading is used in VR Single Pass Stereo
- [Fix] Fixed flickering issue in large worlds due to Unity floating point limitation (deferred rendering path)

V5.5 Changes:
- Added "Allow Batched Meshes" option to inspector (only deferred) to control whether combined meshes or mesh from collider should be used when excluding objects
- Removed heap allocations while rebuilding command buffers which can cause GC if called many times (when ignored objects changing in the frustum)

V5.4.2 Changes:
- [Fix] Fixed issue when excluding objects affected by batching in deffered rendering path

V5.4 Changes:
- Added Snow Tint to forward rendering mode
- Added VertExmotion compatibility

V5.3 Changes:
- Snow Editor: added object snower tool to inspector. Allows you to add/remove snow from specific objects.
- Added ground check type option for footprints. Now you can choose to raycast to detect if player is grounded to leave footprints or not.
- Footprint texture can be set in Global Snow inspector

V5.2.1 Changes:
- [Fix] Fixed issue when rendering skinned mesh renderers with changing LOD

V5.2 Changes:
- Added "Show Coverage Gizmo" option to inspector
- [Fix] Fixed an issue that produced snow flickering when smooth coverage was not enabled

V5.1.3 Changes:
- Minor changes to inspector ranges and settings

V5.1.2 Changes:
- Support for LODGroup exclusion
- Added coverage depth debug visualization to inspector
- [Fix] Fixed issue with keyboard in SceneView
- [Fix] Fixed precision issue with exclusion mask in deferred rendering path
- [Fix] Fixed snow marks on roofs when crossing under

V5.1.1 Changes:
- [Fix] VR: fixes for forward rendering path
- [Fix] Added support for excluded objects with multiple submeshes

V5.1 Changes:
- Improvements to Camera Frost algorithm
- Improvements to Snowfall effect (faster snowfall updates)
- Improvements to Terrain Marks effect (x8 blit speed)
- Smooth trails for fast moving objects (also new parameter MaxStepDistance)

V5.0.3 Changes:
- Increased range for snow coverage & quality options
- [Fix] Disabling the snow effect does not remove snow on some trees and billboards
- [Fix] Terrain & footprints no longer show up on masked areas
- [Fix] Fixed render texture issue with smooth coverage option

V5.0.2 Changes:
- Added support to animatable properties
- Tree and grass billboards coverage now are also influenced by snow amount global setting

V5.0.1 Changes:
- Ability to ignore zenithal depth pass producing complete snow coverage (which also improves performance)
- [Fix] Fixed layer mask issue in deferred rendering path

V5.0 Changes:
- Added in-SceneView snow mask editor
- Optimization (deferred): snow exclusion logic now integrated in main CommandBuffer
- [Fix] Fixed VR exclusion issues on Unity 2018.1
- [Fix] Fixed inspector issues on Unity 2018.1

V4.3.4 Changes:
- Deferred or forward rendering path support files can now be safely removed to reduce app size and build time

V4.3.3 Changes:
- [Fix] Removed warning when building and SceneView snow preview cannot be unloaded

V4.3.2 Changes:
- Added "Deferred Camera Event" option in inspector (only deferred mode, useful to change commandBuffer event if deferred reflections are disabled)

V4.3.1 Changes:
- Added "Excluded Cast Shadows" option in inspector (only forward rendering path)
- [Fix] Fixed shadows on snow from objects not included in layer mask (only forward rendering path)

V4.3 Changes:
- Added snow tint color option (alpha controls saturation)
- [Fix] Fixed snow exclusion issue in Scene View

V4.2.1 Changes:
- Snowfall is more dynamic
- [Fix] Fixed issue with forward rendering workflow dependencies when deferred is used

V4.2 Changes:
- Added snow amount parameter
- Added altitude blending parameter
- Improved compatibility with CTS

V4.1 Changes:
- New holes prefabs
- Improved performance in deferred rendering path
- World Manager API integration

V4.0.2 Changes:
- [Fix] Added workaround for Unity 2017.3 surface shader normal bug

V4.0.1 Changes:
- [Fix] Fixed compiler error on PS4

V4.0 Changes:
- New inspector option to show snow in Scene View
- 2 new hole prefabs (CircularHole / QuadHole) in Resources/Prefabs folder

V3.3 Changes:
- Snowfall effect: added particle shadow support
- 3 snow footfalls audio clips included
- Support for water reflections in Unity Water

V3.2.1 Changes:
- [Fix] Fixed "moving" snow over some models when Relief mapping is enabled
- [Fix] Fixed minor naming typo in one of the shaders

V3.2 Changes:
- Grass can now be fully covered by snow
- Exposed character controller into the inspector
- GlobalSnow Volume: define areas where snow is deactivated automatically
- [Fix] Fixed incompatibility with Ceto ocean system underwater effect
- [Fix] Fixed coverage flashing artifact due to HDR issues
- [Fix] Fixed infinite loop bug when searching for character controller

V3.1 Changes:
- Added Zenithal mask to provide better control over which objects can occlude other objects beneath them
- Coverage algorithm now also takes into account camera culling mask
- Improved first person character component detection
- Additional controls for distance snow: ignore normals, slope controls, ignore coverage
- New quality presets: faster and fastest
- [Fix] Fixed bug when no light is found in the scene
- [Fix] Fixed render texture issue with Unity 2017.1

V3.0.1 Changes:
- Exposed internal default exclusion layer to allow greater flexibility when excluding specific objects from snow (see documentation)
- [Fix] Fixed layer culling issue

V3.0 Changes:
- New slope controls: threadhold, sharpness, noise
- Added Vegetation Offset parameter to control altitude cover over grass and trees
- Added option for hot keys "K" and "J" to change snow altitude at runtime

V2.4 Changes:
- Separated Tree Billboard coverage from Grass coverage parameters
- Added support for SpeedTree billboard shadows on snow
- Improved SpeedTree billboard snow shading
- Added optional support for Unity standard tree billboard (read FAQ)

V2.3 Changes:
- Support for Unity 5.6 speed tree shaders
- New footprints scale and obscurance parameters
- [Fix] Fixed Speed Tree replacement shader issue at build time

V2.2 Changes:
- Added Coverage Update parameter to control when coverage should be computed
- [Fix] Fixed coverage issue with overlapping objects

v2.1 Changes:
- Added option to remove leaves of trees (only SpeedTree)
- Added VR/Single Pass Stereo Rendering compatibility

v2.0.2 Changes:
- [Fix] Removed build warning on DX9 (Global Snow requires SM 3.0+)

v2.0.1 Changes:
- [Fix] Fixed FPS weapons cutout issue on the snow

v2.0 Changes:
- Added "Distance Optimization" option which greatly improve performance by reducing detail beyond a given distance to camera
- Improved snowfall performance
- Added new quality preset: "Medium"
- Improved snow altitude scattering quality
- Fixed snow disappearing over some areas

v1.1 Changes:
- Added ground coverage slider
- Added tree billboard / grass coverage slider
- Fixed Unity tree creator clipping issues
- Fixed Sun cone showing over mountains
- Fixed TransparentCutout materials being snowed when excluded
- Fixed snow disappearing when camera is moved away from scene

V1.0 First Release
