using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class LayerMasks
    {
        public static readonly int STATIC;
        public static readonly int VISUAL_STATIC;
        public static readonly int TANK_TO_TANK;
        public static readonly int ALL_STATIC;
        public static readonly int VISIBLE_FOR_CHASSIS_ACTIVE;
        public static readonly int VISIBLE_FOR_CHASSIS_SEMI_ACTIVE;
        public static readonly int VISIBLE_FOR_CHASSIS_ANIMATION;
        public static readonly int GUN_TARGETING_WITHOUT_DEAD_UNITS;
        public static readonly int GUN_TARGET;
        public static readonly int GUN_TARGETING_WITH_DEAD_UNITS;
        public static readonly int GUN_TARGETING_WITH_DEAD_UNITS_WITHOUT__VISUAL_STATIC;
        public static readonly int GUN_TARGETING_WITH_DEAD_UNITS_BY_SIMPLE_STATIC;
        public static readonly int VISUAL_TARGETING;
        public static readonly int BOT_COLLISION;

        static LayerMasks()
        {
            int[] layers = new int[] { Layers.STATIC };
            STATIC = LayerMasksUtils.CreateLayerMask(layers);
            int[] numArray2 = new int[] { Layers.VISUAL_STATIC };
            VISUAL_STATIC = LayerMasksUtils.CreateLayerMask(numArray2);
            int[] numArray3 = new int[] { Layers.TANK_TO_TANK };
            TANK_TO_TANK = LayerMasksUtils.CreateLayerMask(numArray3);
            ALL_STATIC = LayerMasksUtils.AddLayerToMask(STATIC, Layers.VISUAL_STATIC);
            VISIBLE_FOR_CHASSIS_ACTIVE = LayerMasksUtils.AddLayerToMask(STATIC, Layers.TANK_TO_TANK);
            VISIBLE_FOR_CHASSIS_SEMI_ACTIVE = STATIC;
            VISIBLE_FOR_CHASSIS_ANIMATION = ALL_STATIC;
            int[] numArray4 = new int[] { Layers.TARGET };
            GUN_TARGETING_WITHOUT_DEAD_UNITS = LayerMasksUtils.AddLayersToMask(VISUAL_STATIC, numArray4);
            GUN_TARGET = LayerMasksUtils.AddLayersToMask(Layers.TARGET, new int[0]);
            int[] numArray5 = new int[] { Layers.DEAD_TARGET };
            GUN_TARGETING_WITH_DEAD_UNITS = LayerMasksUtils.AddLayersToMask(GUN_TARGETING_WITHOUT_DEAD_UNITS, numArray5);
            GUN_TARGETING_WITH_DEAD_UNITS_WITHOUT__VISUAL_STATIC = LayerMasksUtils.RemoveLayerFromMask(GUN_TARGETING_WITH_DEAD_UNITS, Layers.VISUAL_STATIC);
            GUN_TARGETING_WITH_DEAD_UNITS_BY_SIMPLE_STATIC = LayerMasksUtils.AddLayerToMask(GUN_TARGETING_WITH_DEAD_UNITS_WITHOUT__VISUAL_STATIC, Layers.STATIC);
            int[] numArray6 = new int[] { Layers.TANK_PART_VISUAL };
            VISUAL_TARGETING = LayerMasksUtils.AddLayersToMask(VISUAL_STATIC, numArray6);
            BOT_COLLISION = LayerMasksUtils.AddLayerToMask(Layers.TANK_TO_TANK, Layers.STATIC);
        }
    }
}