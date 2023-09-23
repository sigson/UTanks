using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Globalization;
using System.IO;
using UTanksClient.ECS.Types.Battle;
using SecuredSpace.Effects;
using SecuredSpace.UnityExtend;
using SecuredSpace.Settings;
using UTanksClient;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.Services;
using UTanksClient.Extensions;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.Battle;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.Settings.SettingsObject;

namespace SecuredSpace.ClientControl.Managers
{
    namespace PropLibrary
    {
        [XmlRoot(ElementName = "texture")]
        public class Texture
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName = "diffuse-map")]
            public string Diffusemap { get; set; }
        }

        [XmlRoot(ElementName = "mesh")]
        public class Mesh
        {
            [XmlElement(ElementName = "texture")]
            public List<Texture> Texture { get; set; }
            [XmlAttribute(AttributeName = "file")]
            public string File { get; set; }
        }

        [XmlRoot(ElementName = "prop")]
        public class Prop
        {
            [XmlElement(ElementName = "mesh")]
            public Mesh Mesh { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "prop-group")]
        public class Propgroup
        {
            [XmlElement(ElementName = "prop")]
            public List<Prop> Prop { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "library")]
        public class Library
        {
            [XmlElement(ElementName = "prop-group")]
            public List<Propgroup> Propgroup { get; set; }
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        public class LibraryProps
        {
            public static Library LibraryPropsLoader(TextAsset xmlFile)
            {
                //XmlRootAttribute xRoot = new XmlRootAttribute();
                //xRoot.ElementName = "library";
                //xRoot.IsNullable = true;
                MemoryStream assetStream = new MemoryStream(xmlFile.bytes);
                XmlSerializer ser = new XmlSerializer(typeof(Library));
                using (XmlReader reader = XmlReader.Create(assetStream))
                {
                    return (Library)ser.Deserialize(reader);
                }
            }

            public static string[] ExtractTextureFile(Library library, string Name, string TextureName)
            {
                string[] ReturnString;
                try
                {
                    //var mesh = library.Propgroup.Prop.Where(x => x.Name == Name).ToList()[0].Mesh;
                    var mesh = library.Propgroup.Where(x => x.Prop.Where(y => y.Name == Name).ToList().Count != 0).ToList()[0].Prop.Where(x => x.Name == Name).ToList()[0].Mesh;
                    try
                    {
                        var textureName = mesh.Texture.Where(x => x.Name == TextureName).ToList()[0].Diffusemap;
                        return new string[] { mesh.File, textureName };//return textureName;
                    }
                    catch (Exception ex)
                    {
                        return new string[] { mesh.File };
                    }
                }
                catch (Exception ex)
                {
                    return new string[] { "NOTFINDRESOURCE" }; ;
                }
            }
        }

    }


    public class MapManager : IEntityManager
    {
        public Map map;
        public BattleManager battleManager => EntityGroupManagersStorageService.instance.GetGroupManager<BattleManager, ECSEntity>(this.ConnectPoint);
        public GameObject MapModelSpace;
        public GameObject MapGlobalLightingSpace;
        public GameObject MapLocalLightingSpace;
        public GameObject MapInteractableSpace;
        public GameObject MapGameElementsSpace;
        public GameObject MapMarksSpace;
        public GameObject MapTracesSpace;
        public GameObject PlayersSpace;
        public GameObject DropSpace;
        public GameObject EffectSpace;
        public GameObject CreatureSpace;
        public bool TestMode = false;
        public string TestSkyboxName = "Winter";
        public string TestMapVersion = "new";
        public bool grassEnable = false;

        public AudioAnchor Audio;


        [Range(0, 23)]
        public int MapTime = 0;
        public Color GlobalLightColor;
        public Quaternion GlobalLightRotation;
        public string WeatherMode;
        public bool MapLoaded = false;

        public static float MinX = 0f;
        public static float MaxX = 0f;
        public static float MinY = 0f;
        public static float MaxY = 0f;
        public static float MinZ = 0f;
        public static float MaxZ = 0f;
        [SerializeField]
        private int renderSpeed = 40;

        public SerializableDictionary<string, Material> propMaterials = new SerializableDictionary<string, Material>();

        public override void ActivateManager()
        {
            base.ActivateManager();
        }
        protected override void OnStartManager()
        {
            //Application.targetFrameRate = 60;
            //if (!MapNameFromEditor)
            //    MapName = File.ReadAllText(Application.streamingAssetsPath + "/selected_map.txt");
        }

        private string ResourcesBasePath = "Battle\\Landscape_";

        protected override void OnActivateManager()
        {
            if(TestMode)
            {
                Loader(Application.streamingAssetsPath + "/Data/Maps/" + "Серпухов" + ".xml");
                ClientInitService.instance.gameSettings = new GameSettings();
                ClientInitService.instance.gameSettings.EnableShadow = true;
                ClientInitService.instance.gameSettings.DeepShadows = true;
            }
                
        }

        public void LoadMap(BattleComponent battleComponent)
        {
            MapLoaded = false;
            MapTime = battleComponent.TimeMode;
            WeatherMode = battleComponent.WeatherMode;
            StartCoroutine(Loader(battleComponent.MapPath));
            LoadStaticMap(battleComponent);
            LightSetup();
            UpdateTime();
        }

        private void LoadStaticMap(BattleComponent battleComponent)
        {
            if(battleComponent.MapModel != "")
                Instantiate(ResourcesService.instance.GameAssets.GetDirectory(battleComponent.MapConfigPath).GetChildFSObject(battleComponent.MapModel).GetContent<GameObject>(), this.MapModelSpace.transform);
        }

        private void LightSetup()
        {
            var globalLight = new GameObject("GlobalLight");
            globalLight.transform.SetParent(MapGlobalLightingSpace.transform);
            var lightAnchor = globalLight.AddComponent<LightAnchor>();
            var light = globalLight.AddComponent<Light>();
            light.type = LightType.Directional;
            var ilight = globalLight.AddComponent<GlobalDirectionalLight>();
            lightAnchor.GlobalMapLight = true;
            lightAnchor.lights.Add(ilight);
            lightAnchor.ownerManagerSpace = this;
            ilight.ownerManagerSpace = this;

            lightAnchor.DirectRegisterObject();
            lightAnchor.UpdateObject();
        }

        public void UpdateTime()
        {
            var dayTimeConfig = ConstantService.instance.GetByConfigPath(@"battle\mapsettings\daytime");
            int rangeStart = 0;
            int rangeEnd = 0;
            foreach (var dayTime in dayTimeConfig.Deserialized["dayTimeVariants"])
            {
                if (dayTime["time"].ToObject<int>() == MapTime)
                {
                    rangeEnd = rangeStart;
                    break;
                }
                if (dayTime["time"].ToObject<int>() > MapTime)
                {
                    rangeEnd = rangeStart;
                    rangeStart--;
                    break;
                }
                rangeStart++;
            }
            if(rangeEnd != rangeStart)
            {
                var startTime = dayTimeConfig.Deserialized["dayTimeVariants"][rangeStart];
                var endTime = dayTimeConfig.Deserialized["dayTimeVariants"][rangeEnd];
                var coef = Convert.ToSingle(MapTime - startTime["time"].ToObject<int>()) / Convert.ToSingle(endTime["time"].ToObject<int>() - startTime["time"].ToObject<int>());

                GlobalLightColor = Color.Lerp(ColorEx.ToColor(Convert.ToInt32(startTime["globalLight"].ToString(),16)), ColorEx.ToColor(Convert.ToInt32(endTime["globalLight"].ToString(), 16)), coef);
                RenderSettings.ambientLight = Color.Lerp(ColorEx.ToColor(Convert.ToInt32(startTime["globalAmbient"].ToString(), 16)), ColorEx.ToColor(Convert.ToInt32(endTime["globalAmbient"].ToString(), 16)), coef);
                var splitStart = startTime["globalLightRotation"].ToString().Split(';');
                Vector3 startGlobalRot = new Vector3(float.Parse(splitStart[0], CultureInfo.InvariantCulture), float.Parse(splitStart[1], CultureInfo.InvariantCulture), float.Parse(splitStart[2], CultureInfo.InvariantCulture));
                var splitEnd = endTime["globalLightRotation"].ToString().Split(';');
                Vector3 endGlobalRot = new Vector3(float.Parse(splitEnd[0], CultureInfo.InvariantCulture), float.Parse(splitEnd[1], CultureInfo.InvariantCulture), float.Parse(splitEnd[2], CultureInfo.InvariantCulture));
                GlobalLightRotation = Quaternion.Euler(Vector3.Lerp(startGlobalRot, endGlobalRot, coef));
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
            else
            {
                var startTime = dayTimeConfig.Deserialized["dayTimeVariants"][rangeStart];
                GlobalLightColor = ColorEx.ToColor(Convert.ToInt32(startTime["globalLight"].ToString(), 16));
                RenderSettings.ambientLight = ColorEx.ToColor(Convert.ToInt32(startTime["globalAmbient"].ToString(), 16));
                var splitStart = startTime["globalLightRotation"].ToString().Split(';');
                Vector3 startGlobalRot = new Vector3(float.Parse(splitStart[0], CultureInfo.InvariantCulture), float.Parse(splitStart[1], CultureInfo.InvariantCulture), float.Parse(splitStart[2], CultureInfo.InvariantCulture));
                GlobalLightRotation = Quaternion.Euler(startGlobalRot);
                ClientInitService.instance.anchorHandler.UpdateAnchors<LightAnchor>((anchor) => true);
            }
        }

        System.Collections.IEnumerator Loader(string MapPath)
        {
            MinX = 0f;
            MaxX = 0f;
            MinY = 0f;
            MaxY = 0f;
            MinZ = 0f;
            MaxZ = 0f;
            yield return new WaitForEndOfFrame();
            var RealPath = MapPath.Contains(Application.streamingAssetsPath) ? MapPath : Application.streamingAssetsPath + "\\" + MapPath;
            XmlSerializer ser = new XmlSerializer(typeof(Map));
            using (XmlReader reader = XmlReader.Create(RealPath))
            {
                map = (Map)ser.Deserialize(reader);
            }
            UTanksClient.ECS.Types.Lobby.MapValue selectedMap = null;
            if (TestMode)
            {
                selectedMap = new UTanksClient.ECS.Types.Lobby.MapValue();
                selectedMap.SkyboxName = TestSkyboxName;
                selectedMap.MapVersion = TestMapVersion;
            }
            else
                selectedMap = GlobalGameDataConfig.SelectableMap.selectableMaps.GameMaps.Select(x => x.Maps.Where(x2 => x2.Path == MapPath)).Where(x => x.Count() > 0).ToList()[0].ToList()[0];
            Dictionary<string, PropLibrary.Library> loadedLibraryes = new Dictionary<string, PropLibrary.Library>();
            Dictionary<string, Material> batchedMaterial = new Dictionary<string, Material>();
            int renderer_count = 0;
            foreach (Prop prop in map.Staticgeometry.Prop)
            {
                PropLibrary.Library library;
                if (!loadedLibraryes.TryGetValue(prop.LibraryName, out library))
                {
                    var libraryExpo = PropLibrary.LibraryProps.LibraryPropsLoader((TextAsset)Resources.Load(ResourcesBasePath + selectedMap.MapVersion + $"\\{prop.LibraryName.Replace(" ", "")}\\library"));
                    loadedLibraryes.Add(prop.LibraryName, libraryExpo);
                    library = loadedLibraryes[prop.LibraryName];
                }
                else
                    library = loadedLibraryes[prop.LibraryName];
                var position = prop.Position;



                var PropName = (prop.Texturename == "" ? prop.Name : prop.Texturename).ToLower().Replace(" ", "_");
                //var pathString = $"Battle\\Landscape\\{prop.Libraryname.Replace(" ", "")}\\{PropName}";


                var PropData = PropLibrary.LibraryProps.ExtractTextureFile(library, prop.Name, prop.Texturename).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
                List<UnityEngine.Object> resources;
                if (PropData.Length == 1)
                {
                    resources = Resources.LoadAll(ResourcesBasePath + selectedMap.MapVersion + $"\\{prop.LibraryName.Replace(" ", "")}\\{PropData[0]}").ToList();
                }
                else
                {
                    var list = new List<UnityEngine.Object>();
                    var meshs = Resources.LoadAll(ResourcesBasePath + selectedMap.MapVersion + $"\\{prop.LibraryName.Replace(" ", "")}\\{PropData[0]}").ToList();//.Where(x => x.name == PropData[0])
                    var textures = Resources.LoadAll(ResourcesBasePath + selectedMap.MapVersion + $"\\{prop.LibraryName.Replace(" ", "")}\\{PropData[1]}").ToList();//.Where(x => x.GetType() == typeof(Texture2D))
                    var resources2 = new UnityEngine.Object[meshs.Count + textures.Count];
                    meshs.CopyTo(resources2, 0);
                    textures.CopyTo(resources2, meshs.Count);
                    resources = resources2.ToList();
                }

                GameObject replacer = null;
                if (!TestMode && false)
                {
                    resources.Clear();
                    resources.Add(replacer);
                    resources.Add(replacer.GetComponent<MeshRenderer>().sharedMaterial.mainTexture);
                }

                if (library.Name.ToLower().IndexOf("bush") != -1 ||//for new
                    library.Name.ToLower().IndexOf("sprite") != -1)//for old
                {
                    var spriteScale = selectedMap.MapVersion == "new" ? 0.75f : 2.5f;

                    var bushSprite = Resources.LoadAll(ResourcesBasePath + selectedMap.MapVersion + $"\\{prop.LibraryName.Replace(" ", "")}\\{PropName.Replace("_", "")}").ToList();
                    var billboardBush = Instantiate(ResourcesService.instance.GetPrefab("billboardSprite"), this.MapModelSpace.transform);
                    billboardBush.GetComponent<AnimationScript>().enabled = false;
                    billboardBush.GetComponent<SpriteRenderer>().enabled = false;
                    billboardBush.GetComponent<AnimationScript>().SpriteScaler = spriteScale;
                    var spriteGameObject = new GameObject();
                    spriteGameObject.transform.SetParent(billboardBush.transform);
                    var spriteRender = spriteGameObject.AddComponent<SpriteRenderer>();
                    spriteRender.sprite = SecuredSpace.UnityExtend.SpriteExtensions.TextureToSprite(bushSprite[0] as Texture2D);
                    spriteRender.material.shader = Shader.Find("Sprites/Diffuse");


                    billboardBush.transform.localScale = new Vector3(spriteScale, spriteScale, spriteScale);
                    //new Vector3().Set
                    spriteGameObject.transform.localPosition = new Vector3(spriteRender.sprite.bounds.size.x * 0.5f * -1, 0f, 0f);
                    resources.Add(billboardBush);

                    //resources = Resources.LoadAll(pathString);
                }

                if (resources.Count == 0 && library.Name != "Bush")
                {

                    //resources = Resources.LoadAll(pathString);
                }
                //resources = resources.Where(x => x.name == PropName).ToArray();
                GameObject mapObject = null;
                PropAnchor propAnchor = null;
                for (int i = 0; i < resources.Count; i++)
                {
                    MeshRenderer render;
                    if (resources[i] != null && resources[i].GetType() == typeof(GameObject) && mapObject == null)
                    {
                        var ZRotation = RadToDeg(float.Parse(prop.Rotation.Z, CultureInfo.InvariantCulture)) - 180;
                        mapObject = (GameObject)Instantiate(resources[i], new Vector3(float.Parse(prop.Position.X, CultureInfo.InvariantCulture), float.Parse(prop.Position.Z, CultureInfo.InvariantCulture), float.Parse(prop.Position.Y, CultureInfo.InvariantCulture)) * Const.ResizeResourceConst, (Quaternion.Euler(RadToDeg(float.Parse(prop.Rotation.X, CultureInfo.InvariantCulture)), ZRotation, RadToDeg(float.Parse(prop.Rotation.Y, CultureInfo.InvariantCulture)))), MapModelSpace.transform);
                        mapObject.tag = "Ground";
                        if (!mapObject.TryGetComponent<MeshRenderer>(out render))
                        {
                            continue;
                        }
                        if (replacer == null)
                            ChildMeshCleaner(mapObject);
                        mapObject.AddComponent<MeshCollider>();
                        var texture = mapObject.GetComponent<MeshRenderer>().material.GetTexture("_MainTex");
                        if (!TestMode && this.WeatherMode == "rain")
                        {
                            mapObject.GetComponent<MeshRenderer>().material = propMaterials["StandartPropRain"];
                        }
                        else
                            mapObject.GetComponent<MeshRenderer>().material = propMaterials["StandartProp"];
                        mapObject.GetComponent<MeshRenderer>().material.SetTexture("_AlbedoMap", texture);
                        mapObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
                        try
                        {
                            //mapObject.GetComponent<MeshRenderer>().material.SetTexture("_AlbedoMap", texture);
                        }
                        catch { }
                        //mapObject.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", 1);

                        //////if (mapObject.GetComponent<MeshFilter>().mesh.bounds.size.z == 0 || mapObject.GetComponent<MeshFilter>().mesh.bounds.size.y == 0)
                        //////    mapObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        //meshFilter.bounds.size.y
                        //mapObject.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", 1);
                        //mapObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Transparent Cutout");//Transparent Cutout
                        //var texture = mapObject.GetComponent<MeshRenderer>().material.mainTexture;
                        //mapObject.GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().materials[1];
                        //mapObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
                        //mapObject.GetComponent<MeshRenderer>().receiveShadows = false;
                        //mapObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;


                        mapObject.GetComponent<MeshRenderer>().lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                        mapObject.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                        

                        //mapObject.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic = false;

                        //if (renderer_count == 1)
                        //{
                        //    mapObject.AddComponent(this.GetComponent<CustomLightRenderer>().GetType());
                        //    mapObject.GetComponent<CustomLightRenderer>().m_CubeMesh = this.GetComponent<CustomLightRenderer>().m_CubeMesh;
                        //    mapObject.GetComponent<CustomLightRenderer>().m_LightShader = this.GetComponent<CustomLightRenderer>().m_LightShader;
                        //    mapObject.GetComponent<CustomLightRenderer>().m_SphereMesh = this.GetComponent<CustomLightRenderer>().m_SphereMesh;
                        //    renderer_count++;
                        //}
                        mapObject.transform.localScale *= 1.001f;
                        mapObject.isStatic = true;


                        //mapObject.isStatic = true;
                        //break;
                        if (resources.Count > 1)
                        {

                        }
                    }
                    else if (resources[i] != null && mapObject != null && resources[i].GetType() == typeof(Texture2D))
                    {
                        try
                        {
                            mapObject.GetComponent<MeshRenderer>().sharedMaterial = batchedMaterial[((Texture2D)resources[i]).GetHashCode().ToString()];
                            //Debug.Log("i batched");
                        }
                        catch
                        {
                            try
                            {
                                mapObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_AlbedoMap", (Texture2D)resources[i]);
                                mapObject.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", (Texture2D)resources[i]);
                                batchedMaterial.Add(((Texture2D)resources[i]).GetHashCode().ToString(), Instantiate(mapObject.GetComponent<MeshRenderer>().material));
                            }
                            catch
                            { }
                        }

                    }


                    //{
                    //    var ZRotation = RadToDeg(float.Parse(prop.Rotation.Z, CultureInfo.InvariantCulture)) - 180;
                    //    var mapObject = (GameObject)Instantiate(GroundPropObject, new Vector3(float.Parse(prop.Position.X, CultureInfo.InvariantCulture), float.Parse(prop.Position.Z, CultureInfo.InvariantCulture), float.Parse(prop.Position.Y, CultureInfo.InvariantCulture)), (Quaternion.Euler(RadToDeg(float.Parse(prop.Rotation.X, CultureInfo.InvariantCulture)), ZRotation, RadToDeg(float.Parse(prop.Rotation.Y, CultureInfo.InvariantCulture)))));
                    //    mapObject.name = resources[i].name;
                    //    mapObject.GetComponent<MeshRenderer>().material.SetTexture("_ MainTex", duplicateTexture((Texture2D)resources[i]));
                    //}
                    //else if (resources[i].GetType() != typeof(Texture2D) && resources[i].GetType() != typeof(GameObject) && resources[i].GetType() != typeof(Mesh) && resources[i].GetType() != typeof(Material))
                    //{
                    //    var ZRotation = RadToDeg(float.Parse(prop.Rotation.Z, CultureInfo.InvariantCulture)) - 180;
                    //    var mapObject = (GameObject)Instantiate(GroundPropObject, new Vector3(float.Parse(prop.Position.X, CultureInfo.InvariantCulture), float.Parse(prop.Position.Z, CultureInfo.InvariantCulture), float.Parse(prop.Position.Y, CultureInfo.InvariantCulture)), (Quaternion.Euler(RadToDeg(float.Parse(prop.Rotation.X, CultureInfo.InvariantCulture)), ZRotation, RadToDeg(float.Parse(prop.Rotation.Y, CultureInfo.InvariantCulture)))));
                    //    mapObject.name = resources[i].name;
                    //    mapObject.GetComponent<MeshRenderer>().material.SetTexture("_ MainTex", duplicateTexture((Texture2D)resources[i]));
                    //}
                }
                if (mapObject != null)
                {
                    propAnchor = mapObject.AddComponent<PropAnchor>();
                    mapObject.AddComponent<GlobalSnowEffect.GlobalSnowCollisionDetector>();

                }
                if (grassEnable && this.WeatherMode != "snowfall" && this.WeatherMode != "cloudinessnowfall")
                {
                    if (prop.LibraryName.Replace(" ", "") == "GrassTiles" || (prop.LibraryName.Replace(" ", "") == "LandTiles" && (mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("grass") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("dirt") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("ground") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("sand") != -1)) || (prop.LibraryName.Replace(" ", "") == "LandRocks" && (mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("rise") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("dirt") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("ground") != -1 || mapObject.GetComponent<MeshRenderer>().material.mainTexture.name.IndexOf("sand") != -1)))
                    {
                        mapObject.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { mapObject.GetComponent<MeshRenderer>().sharedMaterial, propMaterials["Grass"] };   
                        var grasstessel = mapObject.AddComponent<GrassTesselation>();
                        propAnchor.GrassProp = true;
                        grasstessel.grassMaterial = mapObject.GetComponent<MeshRenderer>().materials[1];
                        grasstessel.GrassEnabled = ClientInitService.instance.gameSettings.Grass;
                    }
                }

                if (mapObject != null && mapObject.GetComponent<MeshFilter>() != null)
                {
                    mapObject.GetComponent<MeshRenderer>().sharedMaterials.ForEach(x => propAnchor.PropMaterials.Add(x.shader.name, x));
                    var objPositionMinimal = mapObject.transform.position - mapObject.GetComponent<MeshFilter>().mesh.bounds.size * 2;
                    var objPositionMaximal = mapObject.transform.position + mapObject.GetComponent<MeshFilter>().mesh.bounds.size * 2;
                    if (objPositionMaximal.x > MaxX)
                        MaxX = objPositionMaximal.x;
                    if (objPositionMinimal.x < MinX)
                        MinX = objPositionMinimal.x;
                    if (objPositionMaximal.y > MaxY)
                        MaxY = objPositionMaximal.y;
                    if (objPositionMinimal.y < MinY)
                        MinY = objPositionMinimal.y;
                    if (objPositionMaximal.z > MaxZ)
                        MaxZ = objPositionMaximal.z;
                    if (objPositionMinimal.z < MinZ)
                        MinZ = objPositionMinimal.z;
                }
                    

                if (mapObject == null)
                {
                    if (true) { }
                }
                renderer_count++;
                if(renderer_count == renderSpeed)
                {
                    yield return new WaitForFixedUpdate();
                    renderer_count = 0;
                }
            }
            //var selectedMap = GlobalGameDataConfig.SelectableMap.selectableMaps.GameMaps.Select(x => x.Maps.Where(x2 => x2.Path == MapPath)).Where(x => x.Count() > 0).ToList()[0].ToList()[0];
            if(!TestMode)
            {
                BattleManager.SkyboxMaterial = ResourcesService.instance.GameAssets.GetDirectory($"maps\\skybox\\{selectedMap.SkyboxName}").FillChildContentToItem().GetElement<Material>("skybox");
                if(ClientInitService.instance.gameSettings.ShowSkybox)
                    RenderSettings.skybox = BattleManager.SkyboxMaterial;
                var audio = this.GetComponent<AudioAnchor>();
                audio.audioStorage.Add(ResourcesService.instance.GameAssets.GetDirectory($"maps\\sound").FillChildContentToItem().GetElement<AudioSourceSetting>(selectedMap.AudioName));
                audio.audioStorage[0].loop = true;
                audio.Build();
                audio.audioManager.PlayWithSettings(audio.audioStorage[0].soundName, 0f);

                //this.GetComponent<AudioSource>().loop = true;
                //this.GetComponent<AudioSource>().Play();
            }
            
            //MapParent.transform.localScale = new Vector3(Const.ResizeResourceConst, Const.ResizeResourceConst, Const.ResizeResourceConst);
            CombineMap();
            MapLoaded = true;
        }
        List<CustomRenderTexture> UpdateTexturesList = new List<CustomRenderTexture>();
        

        protected override void UpdateManager()
        {
            UpdateTexturesList.ForEach(x =>
            {
                x.Initialize();
                x.Update();
            });
        }

        public void CombineMap()
        {
            StaticBatchingUtility.Combine(MapModelSpace);
        }

        public static bool CheckBoundsPositon(Vector3 position)
        {
            bool result = true;
            if (position.x > MaxX)
                result = false;
            if (position.x < MinX)
                result = false;
            if (position.y > MaxY)
                result = false;
            if (position.y < MinY)
                result = false;
            if (position.z > MaxZ)
                result = false;
            if (position.z < MinZ)
                result = false;
            return result;
        }
        public void ClearMap()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            UpdateTexturesList.Clear();
            Destroy(this.gameObject);
        }
        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        private void ChildMeshCleaner(GameObject parent)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                try
                {
                    if (parent.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                    {
                        //parent.transform.GetChild(i).GetComponent<MeshRenderer>().material = this.GetComponent<MeshRenderer>().material;
                        ChildMeshCleaner(parent.transform.GetChild(i).gameObject);
                        parent.transform.GetChild(i).transform.gameObject.SetActive(false);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private float RadToDeg(float rad)
        {
            return ((rad * Mathf.Rad2Deg) > 360 ? 360 - ((rad * Mathf.Rad2Deg) - 360) : 360 - (rad * Mathf.Rad2Deg));
        }


        protected override void OnAwakeManager()
        {
            //MapModelSpace = new GameObject("MapModelSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //});
            //MapGlobalLightingSpace = new GameObject("MapGlobalLightingSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //MapLocalLightingSpace = new GameObject("MapLocalLightingSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //MapInteractableSpace = new GameObject("MapInteractableSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //MapGameElementsSpace = new GameObject("MapGameElementsSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //MapMarksSpace = new GameObject("MapMarksSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //MapTracesSpace = new GameObject("MapTracesSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //PlayersSpace = new GameObject("PlayersSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //DropSpace = new GameObject("DropSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //EffectSpace = new GameObject("EffectSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
            //CreatureSpace = new GameObject("CreatureSpace").LineFunction((gameobject) => {
            //    gameobject.transform.SetParent(this.gameObject.transform);
            //}); ;
        }

        protected override void OnRemoveManager()
        {
            
        }

        protected override void OnDeactivateManager()
        {
            
        }

        public override void AddManager()
        {
            
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapManager))]
    public class ResourceManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (MapManager)target;

            if (GUILayout.Button("Update time", GUILayout.Height(20)))
            {
                script.UpdateTime();
            }
        }
    }
#endif

}