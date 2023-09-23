using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.Log
{
    public class LogComponent : MonoBehaviour
    {
        public SerializableDictionary<string, LogObj> data = new SerializableDictionary<string, LogObj>();
        public static LogComponent instance;

        public void Write(string key, float f)
        {
            Write(key, new LogObj() { Value = f.ToString() });
        }

        public void Write(string key, Vector3 Vec)
        {
            Write(key, new LogObj() { Value = Vec.ToString() });
        }

        public void Write(string key, int i)
        {
            Write(key, new LogObj() { Value = i.ToString() });
        }

        public void Write(string key, Quaternion q)
        {
            Write(key, new LogObj() { Value = "Quaternion: " + q.ToString() + "\n" + "Euler rotation: " + q.eulerAngles.ToString() });
        }

        public void Write(string key, string ss)
        {
            Write(key, new LogObj() { Value = ss });
        }

        public void Write(string key, LogObj writedata)
        {
            LogObj value = null;
            data.TryGetValue(key, out value);

            if (value != null)
            {
                data[key] = writedata;
                return;
            }
            else
            {
                data.Add(key, writedata);
            }

        }
        void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(UpdateDate());
            instance = this;
        }

        IEnumerator UpdateDate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                data.OnBeforeSerialize();
                //data.OnAfterDeserialize();
                //data.OnBeforeSerialize();
            }
        }
    }

    [Serializable]
    public class LogObj
    {
        [TextArea(0, 35)]
        public string Value;
    }
    [Serializable]
    public class LogObjFloat
    {
        public float Float;
        public Vector3 Vector3;
        public Quaternion Quaternion;
        public string String;
    }
}