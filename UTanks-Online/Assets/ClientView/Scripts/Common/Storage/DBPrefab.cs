using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.DBResources
{
    public class DBPrefab : MonoBehaviour
    {
        [SerializeField]private SerializableDictionary<string, Object> DBStorage = new SerializableDictionary<string, Object>();
        public SerializableDictionary<string, Object> DB
        {
            get
            {
                return DBStorage;
            }
        }

        public T getObject<T>(string key) where T:Object
        {
            return (T)DB[key];
        }
    }
}