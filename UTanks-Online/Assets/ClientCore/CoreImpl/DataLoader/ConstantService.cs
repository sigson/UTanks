using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;
using System.IO.Compression;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.GameData;
using UTanksClient.ClassExtensions;
using System.Threading;
using UTanksClient.Network.NetworkEvents.Communications;
using System.Security.Cryptography;
using SecuredSpace.ClientControl.Model;

namespace UTanksClient.Services
{
    public class ConstantService : IService
    {
        public static ConstantService instance => SGT.Get<ConstantService>();
        public ConcurrentDictionaryEx<string, ConfigObj> ConstantDB = new ConcurrentDictionaryEx<string, ConfigObj>();
        public Dictionary<long, List<ConfigObj>> TemplateInterestDB = new Dictionary<long, List<ConfigObj>>(); //TemplateAccessor Id
        public Dictionary<long, EntityTemplate> AllTemplates = new Dictionary<long, EntityTemplate>();
        public List<byte> loadedConfigFile = new List<byte>();
        public long checkedConfigVersion = 0;
        private long hashConfig = 0;
        public bool Loaded = false;

        public void PreInitialize()
        {
            hashConfig = 0;
            byte[] configFile = null;

            ClientNetworkService.instance.Socket.on<CheckConfigResult>((CheckConfigResult packet) => {
                ConstantService.instance.loadedConfigFile = packet.newConfig;
                ConstantService.instance.checkedConfigVersion = packet.hash;
                ConstantService.instance.Initialize();
            });

            if (File.Exists(GlobalProgramState.ConfigDir + "zippedconfig.zip"))
            {
                configFile = File.ReadAllBytes(GlobalProgramState.ConfigDir + "zippedconfig.zip");
                hashConfig = BitConverter.ToInt64(MD5.Create().ComputeHash(configFile), 0);
                ClientNetworkService.instance.Socket.emit<CheckConfigVersion>(new CheckConfigVersion()
                {
                    hash = hashConfig
                });
            }
            else
                ClientNetworkService.instance.Socket.emit<CheckConfigVersion>(new CheckConfigVersion()
                {
                    hash = BitConverter.ToInt64(MD5.Create().ComputeHash(BitConverter.GetBytes(12f)), 0)
                });
        }

        public void Initialize()
        {
            var gameConfDirectory = GlobalProgramState.ConfigDir + "GameConfig";
            
            if(checkedConfigVersion != hashConfig)
            {
                ULogger.Log("Constant service update config");
                if (Directory.Exists(GlobalProgramState.ConfigDir))
                    Directory.Delete(GlobalProgramState.ConfigDir, true);
                Directory.CreateDirectory(GlobalProgramState.ConfigDir);
                File.WriteAllBytes(GlobalProgramState.ConfigDir + "zippedconfig.zip", loadedConfigFile.ToArray());
                ZipExt.DecompressToDirectory(loadedConfigFile.ToArray(), GlobalProgramState.ConfigDir, (info) => { });
            }
            #region initload
            var nowLib = "";
            ConfigObj nowObject = new ConfigObj();
            foreach (var file in GetRecursFiles(gameConfDirectory))
            {
                if (file.Contains(".yml"))
                {
                    if (nowLib == "")
                    {
                        nowLib = Path.GetDirectoryName(file);
                        nowObject = new ConfigObj();
                    }
                    else if (nowLib != Path.GetDirectoryName(file))
                    {

                        ConstantDB[nowObject.Path] = nowObject;
                        nowLib = Path.GetDirectoryName(file);
                        nowObject = new ConfigObj();
                    }

                    var input = new StreamReader(file);
                    var yaml = new YamlDotNet.Serialization.Deserializer();
                    var yamlObject = yaml.Deserialize<ExpandoObject>(input);
                    Newtonsoft.Json.JsonSerializer js = new Newtonsoft.Json.JsonSerializer();
                    var w = new StringWriter();
                    js.Serialize(w, yamlObject);
                    string jsonText = w.ToString();
                    System.IO.MemoryStream mStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonText));
                    var reader = new JsonTextReader(new StreamReader(mStream));
                    var jObject = JObject.Load(reader);

                    switch (Path.GetFileNameWithoutExtension(file))
                    {
                        case "id":
                            nowObject.Id = jObject.GetValue("id").Value<long>();
                            break;
                        case "public":
                            nowObject.Deserialized = jObject;
                            break;
                        case "public_en":
                            nowObject.DeserializedInfo = jObject;
                            break;
                    }
                    nowObject.LibName = nowLib.Replace(Directory.GetParent(nowLib).FullName, "").Replace("\\", "");
                    nowObject.HeadLibName = Directory.GetParent(nowLib).Name;
                    nowObject.Path = file.Replace(gameConfDirectory, "").Replace("\\" + Path.GetFileName(file), "").Substring(1);
                    nowObject.LibTree = new Lib() { LibName = nowObject.LibName, Path = nowObject.Path };
                }
            }
            ConstantDB[nowObject.Path] = nowObject;



            var allTemplates = ECSAssemblyExtensions.GetAllSubclassOf(typeof(EntityTemplate)).Select(x => (EntityTemplate)Activator.CreateInstance(x)).ToList(); //load interested configs to template accessor db
            foreach (EntityTemplate templateAccessor in allTemplates)
            {
                List<ConfigObj> interestedList = new List<ConfigObj>();
                AllTemplates[templateAccessor.GetId()] = templateAccessor;
                foreach (var path in templateAccessor.ConfigPath)
                {
                    var result = ConstantDB.Values.Where(x => x.Path == path).FirstOrDefault();
                    if (result != null)
                        interestedList.Add(result);
                }
                try
                {
                    var field = templateAccessor.GetType().GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    var customAttrib = templateAccessor.GetType().GetCustomAttribute<TypeUidAttribute>();
                    if (customAttrib != null)
                        field.SetValue(null, customAttrib.Id);
                    //Console.WriteLine(comp.GetId().ToString() + "  " + comp.GetType().Name);
                }
                catch
                {
                    Console.WriteLine(templateAccessor.GetType().Name);
                }
                TemplateInterestDB.Add(templateAccessor.GetId(), interestedList);
            }
            #endregion
            Core.Logging.ULogger.LogLoad("4/5/Constants loaded...");
            InitializeDefaultDataObject.InitializeDataObjects();
            Core.Logging.ULogger.LogLoad("5/5/Defaults loaded...");
            Loaded = true;
        }

        public List<ConfigObj> GetByTemplate(EntityTemplate templateAccessor)
        {
            List<ConfigObj> result = new List<ConfigObj>();
            if (TemplateInterestDB.TryGetValue(templateAccessor.GetId(), out result))
            {
                return result;
            }
            return result;
        }

        public ConfigObj GetByConfigPath(string path)
        {
            ConfigObj obj;
            if (ConstantDB.TryGetValue(path, out obj))
                return obj;
            return null;
        }

        public ConfigObj[] GetByLibName(string libName)
        {
            List<ConfigObj> result = new List<ConfigObj>();
            foreach (ConfigObj configObj in ConstantDB.Values)
            {
                if (configObj.LibName == libName)
                    result.Add(configObj);
            }
            return result.ToArray();
        }
        public ConfigObj[] GetByHeadLibName(string libName)
        {
            List<ConfigObj> result = new List<ConfigObj>();
            foreach (ConfigObj configObj in ConstantDB.Values)
            {
                if (configObj.HeadLibName == libName)
                    result.Add(configObj);
            }
            return result.ToArray();
        }
        public List<string> GetRecursFiles(string start_path)
        {
            List<string> ls = new List<string>();
            try
            {
                string[] folders = Directory.GetDirectories(start_path);
                foreach (string folder in folders)
                {
                    ls.Add("Папка: " + folder);
                    ls.AddRange(GetRecursFiles(folder));
                }
                string[] files = Directory.GetFiles(start_path);
                foreach (string filename in files)
                {
                    ls.Add(filename);
                }
            }
            catch (System.Exception e)
            {
            }
            return ls;
        }

        public override void InitializeProcess()
        {
            
        }

        public override void OnDestroyReaction()
        {
            
        }

        public override void PostInitializeProcess()
        {
            
        }
    }

    public class ConfigObj
    {
        public static char delim = '\\';
        public long Id;
        public string Path;
        public string LibName;
        public string HeadLibName;
        public Lib LibTree;
        public JObject Deserialized;
        public JObject DeserializedInfo;

        public T GetObject<T>(string path)
        {
            return GetObjectImpl<T>(this.Deserialized, path);
        }

        public T GetObjectInfo<T>(string path)
        {
            return GetObjectImpl<T>(this.DeserializedInfo, path);
        }

        protected T GetObjectImpl<T>(JObject storage, string path)
        {
            var pathSplit = path.Split(delim);
            var nowStorage = storage[pathSplit[0]];
            for (int i = 1; i < pathSplit.Length; i++)
            {
                if (!Lambda.TryExecute(() => nowStorage = nowStorage[pathSplit[i]]))
                    if (!Lambda.TryExecute(() => nowStorage = nowStorage[int.Parse(pathSplit[i])]))
                        throw new Exception("Wrong JObject iterator");
            }
            return nowStorage.ToObject<T>();
        }
    }

    public class Lib
    {
        public string Path;
        public string LibName;
        public Lib HeadLib
        {
            get
            {
                var splitedPath = this.Path.Split('\\');
                var newPath = "";
                for(int i = 0; i < splitedPath.Length -1; i++)
                {
                    newPath += splitedPath[i] + "\\";
                }
                newPath = newPath.Substring(0, newPath.Length - 1);
                var newNameLib = splitedPath.ElementAt(splitedPath.Length - 2);
                return new Lib()
                {
                    Path = newPath,
                    LibName = newNameLib
                };
                //bmark: append nulllib wrapper for catching error headlib 
            }
        }
    }
}

