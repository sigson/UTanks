#ifndef GLOBALSNOW_FORWARD_OPTIONS
#define GLOBALSNOW_FORWARD_OPTIONS

// Uncomment to enable mask support on grass and trees in forward rendering path
// #define GLOBALSNOW_MASK   

// Comment out to disable zenithal depth
#define USE_ZENITHAL_DEPTH


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