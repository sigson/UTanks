using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Init;
using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Core.Logging;

namespace SecuredSpace.ClientControl.Services
{
    public class ResourcesService : IService
    {
        public static ResourcesService instance => IService.getInstance<ResourcesService>();
        public DBPrefab DB;
        private Dictionary<PrefabID.PrefabsID, GameObject> constPrefabsId = new Dictionary<PrefabID.PrefabsID, GameObject>();
        private Dictionary<string, GameObject> constPrefabs = new Dictionary<string, GameObject>();

        public GameObject GetPrefab(PrefabID.PrefabsID iD)
        {
            if(constPrefabsId.TryGetValue(iD, out var prefab))
            {
                return prefab;
            }
            else
            {
                ULogger.Error("try to get non-cached prefab with " + iD.ToString());
                return null;
            }
        }

        public GameObject GetPrefab(string iD)
        {
            if (constPrefabs.TryGetValue(iD, out var prefab))
            {
                return prefab;
            }
            else
            {
                ULogger.Error("try to get non-cached prefab with " + iD.ToString());
                return null;
            }
        }

        public ResourceManager GameAssets => DB.DB["GameAssetsStorage"] as ResourceManager;
        public override void InitializeProcess()
        {
            DB = FindObjectOfType<InitServices>().DB;
        }
        
        public override void OnDestroyReaction()
        {
            
        }

        public override void PostInitializeProcess()
        {
            var prefabStorage = ResourcesService.instance.DB.getObject<ResourceManager>("PrefabStorage");
            foreach(var content in prefabStorage.EnumerateAllFiles())
            {
                if(content is GameObject)
                {
                    var gobject = content as GameObject;
                    var prefId = gobject.GetComponent<PrefabID>();
                    if (prefId != null)
                    {
                        if(prefId.cID == PrefabID.PrefabsID.None)
                        {
                            constPrefabs.Add(prefId.ID, gobject);
                        }
                        else
                        {
                            constPrefabsId.Add(prefId.cID, gobject);
                        }
                    }
                }
            }
        }
    }
}