using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public static class LayerMasksUtils
    {
        public static int AddLayersToMask(int layerMask, params int[] layers)
        {
            int num = layerMask;
            for (int i = 0; i < layers.Length; i++)
            {
                num = AddLayerToMask(num, layers[i]);
            }
            return num;
        }

        public static int AddLayerToMask(int layerMask, int layer)
        {
            ValidateLayer(layer);
            return (layerMask | (1 << (layer & 0x1f)));
        }

        public static int CreateLayerMask(params int[] layers) =>
            AddLayersToMask(0, layers);

        public static int RemoveLayerFromMask(int layerMask, int layer)
        {
            ValidateLayer(layer);
            return (layerMask & ~(1 << (layer & 0x1f)));
        }

        public static int RemoveLayersFromMask(int layerMask, params int[] layers)
        {
            int num = layerMask;
            for (int i = 0; i < layers.Length; i++)
            {
                num = RemoveLayerFromMask(num, layers[i]);
            }
            return num;
        }

        private static void ValidateLayer(int layer)
        {
            if ((layer < 0) || (layer > 0x1f))
            {
                //throw new LayerMasksValidationException($"Invalid layer: {layer}; Argument must be [0; 31]");
            }
        }
    }
}