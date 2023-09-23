using SecuredSpace.AudioManager.Settings;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UTanksClient;
using UTanksClient.Core.Logging;

namespace SecuredSpace.ClientControl.DBResources
{
    [CreateAssetMenu(fileName = "New ItemCard", menuName = "Data Objects/Item Card", order = 1)]
    public class ItemCard : ScriptableObject
    {
        public bool clearNameAssetsLoad = true;
        public bool parseInheritFiles;
        public bool loadAllRelatedAssets = false;
        public SerializableDictionary<string, UnityEngine.Object> ItemData = new SerializableDictionary<string, UnityEngine.Object>();
        public SerializableDictionary<string, UnityEngine.Object> AppendBufferData = new SerializableDictionary<string, UnityEngine.Object>();
        public SerializableDictionary<string, Color> ItemColors;

        public ItemCard GetClone()
        {
            return Instantiate(this);
        }

        public T GetElement<T>(string Key) where T : UnityEngine.Object
        {
            var content = this.ItemData[Key];
            if (content is T)
            {
                return (T)content;
            }
            else if (typeof(T) == typeof(Sprite) && content is Texture2D)
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
                Debug.LogError($"Error ItemCard casting {content.GetType().ToString()} to {typeof(T).ToString()}");
                return null;
            }
        }

        public static ItemCard operator +(ItemCard a, ItemCard b)
        {
            b.ItemData.ForEach(x => a.ItemData.Add(x.Key, x.Value));
            return a;
        }

        public List<T> GetListOfItems<T>() where T : Object
        {
            var result = new List<T>();
            this.ItemData.dictionary.ForEach(x => result.Add(x.value as T));
            return result;
        }

#if UNITY_EDITOR
        public void LoadAllLocalAssets()
        {
            var allAssetsPath = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/") + "/" + AssetDatabase.GetAssetPath(this);
#if PLATFORM_STANDALONE_WIN
            allAssetsPath = allAssetsPath.Replace("/", "\\");
#endif
            allAssetsPath = new FileInfo(allAssetsPath).Directory.FullName;
            string[] allfiles = Directory.GetFiles(allAssetsPath, "*.*", parseInheritFiles ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in allfiles)
            {
                var fileObj = new FileInfo(file);
                if (fileObj.Extension != ".meta")
                {
                    Object contentAsset = AssetDatabase.LoadAssetAtPath<Object>("Assets\\" + file.Substring(Application.dataPath.Length));
                    if (contentAsset != null)
                    {
                        var filename = clearNameAssetsLoad ? fileObj.Name.Substring(0, fileObj.Name.LastIndexOf(fileObj.Extension)) : fileObj.Name;
                        if (!ItemData.ContainsKey(filename))
                        {
                            ItemData.Add(filename, contentAsset);
                        }
                        if ((contentAsset.GetType() == typeof(AudioSourceSetting) && ItemData.TryGetValue(filename, out var audioclip) && audioclip.GetType() == typeof(AudioClip)))
                        {
                            ItemData[filename] = contentAsset;
                        }
                    }
                    if (loadAllRelatedAssets)
                    {
                        var relContentAsset = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets\\" + file.Substring(Application.dataPath.Length)).OfType<Object>().ToList();
                        if (relContentAsset != null)
                        {
                            var filename = clearNameAssetsLoad ? fileObj.Name.Substring(0, fileObj.Name.LastIndexOf(fileObj.Extension)) : fileObj.Name;
                            relContentAsset.ForEach(x =>
                            {
                                if (!ItemData.ContainsKey(x.name))
                                {
                                    ItemData.Add(x.name, x);
                                }
                            });
                        }
                    }
                }
            }
        }
#endif
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemCard))]
    public class ItemCardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (ItemCard)target;

            if (GUILayout.Button("Append buffer to item data", GUILayout.Height(40)))
            {
                script.AppendBufferData.ForEach(x => script.ItemData.Add(x.Key, x.Value));
                script.AppendBufferData.Clear();
            }
            if (GUILayout.Button("Load all local asset items", GUILayout.Height(40)))
            {
                script.LoadAllLocalAssets();
            }
        }
    }
#endif
}