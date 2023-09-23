using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace SecuredSpace.ClientControl.DBResources
{
    [CreateAssetMenu(fileName = "New ItemResourcesDB", menuName = "Data Objects/Item Resources DB", order = 1)]
    public class ItemResourcesDB : ScriptableObject
    {
        public SerializableDictionary<string, SerializableDictionary<string, ItemResource>> ItemsResourcesDB;
        public SerializableDictionary<string, SerializableDictionary<string, ItemResource>> BattleObjectsResourcesDB;
        public SerializableDictionary<string, SerializableDictionary<string, ItemResource>> MapObjectsDB;
        public SerializableDictionary<string, SerializableDictionary<string, ItemResource>> UIObjectsDB;
        public SerializableDictionary<string, ItemResource> WeaponEffectsDB;
        public SerializableDictionary<string, Sprite> BattleMapPreview;
        public SerializableDictionary<string, GameObject> MapPropReplacer;
        //public string FullName;
        //[TextArea(3, 10)]
        //public string GarageTextInfo;
        //public List<UnityEngine.Object> Object;
        //public bool ForSale;
        //public int Price;
        //public int Discount;
        //public Texture2D Preview;
        //[TextArea(3, 10)]
        //public string Hint;
        ////tech fields
        //public object Parent;
    }
    [Serializable]
    public class ItemResource
    {
        public SerializableDictionary<string, Texture2D> Preview;
        public SerializableDictionary<string, Texture> Lightmap;
        public SerializableDictionary<string, Texture2D> Details;
        public SerializableDictionary<string, Texture2D> Color;
        public SerializableDictionary<string, Color> ItemColors;
        public SerializableDictionary<string, Sprite> Sprites;
        public SerializableDictionary<string, Texture2D> MoveEffects;
        public SerializableDictionary<string, Texture2D> ShotComponents;
        public SerializableDictionary<string, GameObject> PrefabOfScripts;
        public SerializableDictionary<string, AudioClip> Sounds;
        public SerializableDictionary<string, VideoClip> Videos;
        public SerializableDictionary<string, Texture2D> EffectTextures;
        public SerializableDictionary<string, Animation> Animations;
        public SerializableDictionary<string, GameObject> ScriptAnimations;
        public SerializableDictionary<string, GameObject> Models;
        public SerializableDictionary<string, Mesh> MeshModels;
        public SerializableDictionary<string, Material> Materials;
        public SerializableDictionary<string, Shader> Shader;
        public bool ShaderedAnimationColor;
        public bool Animated;
        public bool BasicMoveAnimation;
    }
}