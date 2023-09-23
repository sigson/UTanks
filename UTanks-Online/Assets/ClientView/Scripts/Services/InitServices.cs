using SecuredSpace.ClientControl.DBResources;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UnityExtend.MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.ClientControl.Init
{
    public class InitServices : MonoBehaviour
    {
        public DBPrefab DB;
        void Awake()
        {
            DontDestroyOnLoad(this);
            IService.InitializeAllServices();
            ObjectPoolInit();
        }

        void ObjectPoolInit()
        {
            this.gameObject.AddComponent<EasyObjectPool>().ExternInit();
            EasyObjectPool.instance.AddPool(new PoolInfo
            {
                fixedSize = false,
                poolName = "PointLightSources",
                poolSize = 30,
                prefab = ResourcesService.instance.GetPrefab("PointLight")
            });
            EasyObjectPool.instance.AddPool(new PoolInfo
            {
                fixedSize = false,
                poolName = "ReflectionLightSources",
                poolSize = 30,
                prefab = ResourcesService.instance.GetPrefab("ReflectionProbeLight")
            });
        }
    }
}