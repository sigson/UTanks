using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.Extensions;

namespace SecuredSpace.ClientControl.DBResources
{
//    [CreateAssetMenu(fileName = "New AudioClipCard", menuName = "Data Objects/Audio clip card", order = 1)]
//    public class AudioClipCard : ScriptableObject
//    {
//        public SerializableDictionary<string, AudioClipPartData> AudioClipParts = new SerializableDictionary<string, AudioClipPartData>(); //queue of audio
//    }

//#if UNITY_EDITOR
//    [CustomEditor(typeof(AudioClipCard))]
//    public class AudioClipCardEditor : Editor
//    {
//        string[] SelectableMusic = new string[] { };
//        int selectedMusic = 0;
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            var script = (AudioClipCard)target;
//            if(SelectableMusic.Length != script.AudioClipParts.Count)
//            {
//                var selectMusicNew = new List<string>();
//                script.AudioClipParts.Keys.ForEach(x => selectMusicNew.Add(x));
//                SelectableMusic = selectMusicNew.ToArray();
//            }
//            selectedMusic = EditorGUILayout.Popup("Music to test", selectedMusic, SelectableMusic);

//            try
//            {
//                if (script.AudioClipParts.Count > 0)
//                {
//                    var clipPart = script.AudioClipParts[SelectableMusic[selectedMusic]];
//                    if(clipPart.SourceAudioClip != null)
//                    {
//                        GUILayout.TextField($"start at {clipPart.SourceAudioClip.length * clipPart.timeStart}, and at {clipPart.SourceAudioClip.length * clipPart.timeEnd - clipPart.SourceAudioClip.length * clipPart.timeStart}");
//                        if (GUILayout.Button("Play audioclip part", GUILayout.Height(40)))
//                        {

//                            //var newclip = AudioControl.TrimAudioClip(, );
//                            //AudioSource.PlayClipAtPoint(newclip, Vector3.zero);
//                            EditorSFX.PlayClip(clipPart.SourceAudioClip);

//                        }
//                    }
//                }
//            }
//            catch { //ULogger.Error("Error while play audioclip,");
//                    }
//        }
//    }

//#endif

//    [System.Serializable]
//    public class AudioClipPartData
//    {
//        public AudioClip SourceAudioClip;
//        [Range(0.0f, 1.0f)]
//        public float timeStart = 0;
//        [Range(0.0f, 1.0f)]
//        public float timeEnd = 1.0f;
//        public bool reversePlay = false;
//        public bool loop = false;
//        public float minDistance = 1;
//        public float maxDistance = 99999;
//        public float playSpeed = 1;
//    }

}