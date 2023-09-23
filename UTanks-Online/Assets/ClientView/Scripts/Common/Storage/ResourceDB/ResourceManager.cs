using SecuredSpace.AudioManager.Settings;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;

namespace SecuredSpace.ClientControl.DBResources
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ResourceManager))]
    public class ResourceManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (ResourceManager)target;

            if (GUILayout.Button("Generate resource tree", GUILayout.Height(40)))
            {
                script.GenerateResourceDBTree();
            }
            if (GUILayout.Button("(Debug) Clear empty", GUILayout.Height(40)))
            {
                script.ClearEmptyFolders();
            }
        }
    }
#endif
    [CreateAssetMenu(fileName = "New ResourceManager", menuName = "Data Objects/Resource Manager", order = 1)]
    public class ResourceManager : ScriptableObject
    {
        public string rootPath;
        public bool localDirectoryNaming;
        public List<IncludeRuleRecord> includeRuleRecords = new List<IncludeRuleRecord>();
        public ResourceFileSystemObject rootDirectory;

        [System.Serializable]
        public enum IncludeResourcesRule
        {
            ExcludePath,
            ExcludeType,
            ExcludeFileExtension,
            IncludeOnlyFileExtension,
            IncludeOnlyType
        }
        [System.Serializable]
        public class IncludeRuleRecord
        {
            public IncludeResourcesRule includeResourcesRule;
            public string strValue;
            public Object objValue;
        }
    
        public ResourceFileSystemObject GetDirectory(string path)
        {
            return this.rootDirectory.childContent[path];
        }

        public List<Object> EnumerateAllFiles()
        {
            List<Object> ret = new List<Object>();
            if(this.rootDirectory.content != null)
                ret.Add(this.rootDirectory.content);
            foreach(var dir in this.rootDirectory.childContent)
            {
                if (dir.Value.content != null)
                    ret.Add(dir.Value.content);
                foreach (var dirCont in dir.Value.childContent)
                {
                    if (dirCont.Value.content != null)
                        ret.Add(dirCont.Value.content);
                }
            }
            return ret;
        }

        public void ClearEmptyFolders()
        {
            List<string> KeysToRemove = new List<string>();
            rootDirectory.childContent.ForEach(x =>
            {
                if (x.Value.childContent.Count == 0 && x.Value.content == null)
                    KeysToRemove.Add(x.Key);
            });
            KeysToRemove.ForEach(x => rootDirectory.childContent.Remove(x));
        }

#if UNITY_EDITOR

        public void GenerateResourceDBTree()
        {
            var allAssetsPath = Application.dataPath + "\\";
            string[] allfiles = Directory.GetFiles(allAssetsPath + rootPath, "*.*", SearchOption.AllDirectories);
            string[] alldirectories = Directory.GetDirectories(allAssetsPath + rootPath, "*.*", SearchOption.AllDirectories);

            System.Action<ResourceFileSystemObject> loadAllChildObjects = (resFSObject) =>
            {
                string[] allfiles = Directory.GetFiles(allAssetsPath + resFSObject.fullPath, "*.*", SearchOption.TopDirectoryOnly);

                if(includeRuleRecords.Count() > 0)
                {
                    allfiles = allfiles.Where(x =>
                    {
                        Dictionary<IncludeRuleRecord, bool> checkedRools = new Dictionary<IncludeRuleRecord, bool>();
                        var xFI = new FileInfo(x);
                        foreach (var rule in includeRuleRecords)
                        {
                            switch (rule.includeResourcesRule)
                            {
                                case IncludeResourcesRule.ExcludePath:
                                    if (x.Contains(rule.strValue))
                                    {
                                        checkedRools[rule] = false;
                                    }
                                    break;
                                case IncludeResourcesRule.ExcludeType:
                                    if (xFI.Extension.Contains(rule.strValue))
                                    {
                                        checkedRools[rule] = false;
                                    }
                                    break;
                                case IncludeResourcesRule.ExcludeFileExtension:
                                    if (xFI.Extension.Contains(rule.strValue))
                                    {
                                        checkedRools[rule] = false;
                                    }
                                    break;
                                case IncludeResourcesRule.IncludeOnlyFileExtension:
                                    if (!xFI.Extension.Contains(rule.strValue))
                                    {
                                        checkedRools[rule] = false;
                                    }
                                    break;
                                case IncludeResourcesRule.IncludeOnlyType:
                                    if (!xFI.Extension.Contains(rule.strValue))
                                    {
                                        checkedRools[rule] = false;
                                    }
                                    break;
                            }
                        }
                        if ((checkedRools.Values.Where(rl => !rl).Count()) == includeRuleRecords.Count)
                            return false;
                        else
                            return true;
                    }).ToArray();
                }
                

                foreach (var file in allfiles)
                {
                    var fileObj = new FileInfo(file);
                    if (fileObj.Extension != ".meta")
                    {
                        var contentAsset = AssetDatabase.LoadAssetAtPath<Object>("Assets\\" + file.Substring(allAssetsPath.Length));
                        if(contentAsset != null)
                        {
                            var clearKey = fileObj.Name.Substring(0, fileObj.Name.Length - fileObj.Extension.Length);
                            if (!resFSObject.childContent.ContainsKey(fileObj.Name))
                            {
                                resFSObject.childContent.Add(fileObj.Name, new ResourceFileSystemObject
                                {
                                    content = contentAsset,
                                    fullPath = resFSObject.fullPath + "\\" + fileObj.Name
                                });
                                
                                if (!resFSObject.childContentClearName.ContainsKey(clearKey))
                                    resFSObject.childContentClearName.Add(clearKey, fileObj.Name);
                            }
                            else
                            {
                                ULogger.Log("Key was added " + fileObj.Name);
                            }
                            if ((contentAsset.GetType() == typeof(AudioSourceSetting) && resFSObject.childContentClearName.TryGetValue(clearKey, out var audioclip) && audioclip.GetType() == typeof(AudioClip)))
                            {
                                resFSObject.childContentClearName[clearKey] = fileObj.Name;
                            }
                        }
                    }
                }
            };
            var rootdirFullName = new DirectoryInfo(allAssetsPath + rootPath).FullName;
            rootDirectory.fullPath = rootPath != ""? rootdirFullName.Substring(rootdirFullName.LastIndexOf(rootPath)) : "";
            rootDirectory.isFolder = true;
            rootDirectory.childContent.Clear();
            rootDirectory.childContentClearName.Clear();
            loadAllChildObjects(rootDirectory);
            foreach (var directory in alldirectories)
            {
                var splitedDirectory = directory.Substring((allAssetsPath + rootPath).Length + (rootPath == ""? 0 : 1)).Split('\\');
                var splitedBag = rootDirectory.fullPath == "" ? "" : "\\";
                var parentDirectory = rootDirectory;
                foreach(var splitPart in splitedDirectory)
                {
                    splitedBag += splitPart + "\\";
                    //if(!parentDirectory.childContent.ContainsKey(splitPart))
                    //{
                    //    parentDirectory.childContent.AddEx(splitPart, new ResourceFileSystemObject()
                    //    {
                    //        isFolder = true,
                    //        fullPath = rootDirectory.fullPath + splitedBag
                    //    });
                    //}
                    //parentDirectory = parentDirectory.childContent[splitPart];
                    //loadAllChildObjects(parentDirectory);
                    var clearSplited = splitedBag.Substring(0, splitedBag.Length - 1);
                    if (!rootDirectory.childContent.ContainsKey(rootDirectory.fullPath + clearSplited))
                    {
                        rootDirectory.childContent.Add(rootDirectory.fullPath + clearSplited, new ResourceFileSystemObject()
                        {
                            isFolder = true,
                            fullPath = rootDirectory.fullPath + clearSplited
                        });
                        if(!rootDirectory.childContentClearName.ContainsKey(splitPart))
                            rootDirectory.childContentClearName.Add(splitPart, rootDirectory.fullPath + clearSplited);
                    }
                    else
                    {

                    }
                    //parentDirectory = parentDirectory.childContent[splitPart];
                    loadAllChildObjects(rootDirectory.childContent[rootDirectory.fullPath + clearSplited]);
                }
            }
            var cacheFolders = new List<ResourceFileSystemObject>();
            if(localDirectoryNaming)
            {
                foreach (var childDirectory in rootDirectory.childContent)
                {
                    childDirectory.Value.fullPath = childDirectory.Value.fullPath.Substring(rootDirectory.fullPath.Length + 1);
                    cacheFolders.Add(childDirectory.Value);
                }
                rootDirectory.childContent.Clear();
                cacheFolders.ForEach(x => rootDirectory.childContent.Add(x.fullPath, x));
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }
    [System.Serializable]
    public class ResourceFileSystemObject
    {
        public SerializableDictionary<string, ResourceFileSystemObject> childContent = new SerializableDictionary<string, ResourceFileSystemObject>();
        public SerializableDictionary<string, string> childContentClearName = new SerializableDictionary<string, string>();
        public string fullPath = "";
        public bool isFolder = false;
        [System.NonSerialized]
        private System.Type contentType = null;
        [HideInInspector]
        public System.Type ContentType
        {
            get
            {
                if (contentType == null)
                    if (content != null)
                        contentType = content.GetType();
                return contentType;
            }
        }
        public Object content;
        public T GetContent<T>() where T : Object
        {
            if(content is T)
            {
                return (T)content;
            }
            else if(typeof(T) == typeof(Sprite) && content is Texture2D)
            {
                return SpriteExtensions.TextureToSprite(content as Texture2D) as T;
            }
            else if (typeof(T) == typeof(AudioSourceSetting) && content is AudioClip)
            {
                var audioClipCard = new AudioSourceSetting();
                audioClipCard.audioClip = content as AudioClip;
                audioClipCard.soundName = audioClipCard.audioClip.name;
                return audioClipCard as T;
            }
            else
            {
                Debug.LogError($"Error ResourceManager casting {content.GetType().ToString()} to {typeof(T).ToString()}");
                return null;
            }
        }
        public bool isType<T>() where T : Object => ContentType == typeof(T);

        public ItemCard FillChildContentToItem(string appendToKeyValue = "", bool includePathInKey = false)
        {
            ItemCard result = ItemCard.CreateInstance<ItemCard>();
            foreach(var child in this.childContentClearName)
            {
                result.ItemData.Add(appendToKeyValue + child.Key + (includePathInKey ? this.fullPath : ""), this.childContent[child.Value].content);
            }
            return result;
        }

        public ResourceFileSystemObject GetChildFSObject(string name)
        {
            return this.childContent[childContentClearName[name]];
        }
    }
}