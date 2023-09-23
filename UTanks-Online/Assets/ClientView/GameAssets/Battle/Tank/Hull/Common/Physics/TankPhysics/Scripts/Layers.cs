using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class Layers
    {
        public static readonly int DEFAULT = 0;
        public static readonly int TRANSPARENT_FX = 1;
        public static readonly int IGNORE_RAYCAST = 2;
        public static readonly int WATER = 4;
        public static readonly int UI = 5;
        public static readonly int TANK_AND_STATIC = 8;
        public static readonly int FRICTION = 9;
        public static readonly int MINOR_VISUAL = 10;
        public static readonly int STATIC = 11;
        public static readonly int TRIGGER_WITH_SELF_TANK = 14;
        public static readonly int TANK_TO_STATIC = 15;
        public static readonly int SELF_SEMIACTIVE_TANK_BOUNDS = 0x10;
        public static readonly int REMOTE_TANK_BOUNDS = 0x11;
        public static readonly int TARGET = 0x12;
        public static readonly int TANK_TO_TANK = 20;
        public static readonly int TANK_PART_VISUAL = 0x16;
        public static readonly int VISUAL_STATIC = 0x17;
        public static readonly int DEAD_TARGET = 0x18;
        public static readonly int GRASS_GENERATION = 0x19;
        public static readonly int SELF_TANK_BOUNDS = 0x1a;
        public static readonly int LOGIC_ELEMENTS = 0x1c;
        public static readonly int GRASS = 0x1d;
        public static readonly int HANGAR = 30;
        public static readonly int INVISIBLE_PHYSICS = IGNORE_RAYCAST;
        public static readonly int EXCLUSION_RAYCAST = IGNORE_RAYCAST;
    }
}