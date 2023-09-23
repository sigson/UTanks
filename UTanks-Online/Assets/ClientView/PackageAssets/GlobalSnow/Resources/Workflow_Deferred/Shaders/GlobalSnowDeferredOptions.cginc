#ifndef GLOBALSNOW_DEFERRED_OPTIONS
#define GLOBALSNOW_DEFERRED_OPTIONS

// ************* Global shader feature options ********************

// Uncomment to support coverage mask on grass and trees in deferred rendering path
//#define GLOBALSNOW_MASK

// Comment out to disable zenithal depth
#define USE_ZENITHAL_DEPTH

// Uncomment and adjust distance to exclude First Person weapons from snow (if required)
//#define EXCLUDE_NEAR_SNOW
#define NEAR_DISTANCE_SNOW 0.00002


// Comment out to enable VertExmotion integration. Also make sure the include path is correct.
//#define ENABLE_VERTEXMOTION_INTEGRATION
//#include "Assets/VertExmotion/Shaders/VertExmotion.cginc" 


// ************* Common functions ******************
#if defined(ENABLE_VERTEXMOTION_INTEGRATION)
   #define APPLY_VERTEX_MODIFIER(v) v.vertex = VertExmotion( v.vertex, v.color );
#else
   #define APPLY_VERTEX_MODIFIER(v)
#endif
    
#endif