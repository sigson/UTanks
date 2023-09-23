using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.Extensions;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IService : SGT
    {
        public abstract void PostInitializeProcess();

        #region static
        public static void GenerateServiceStorage()
        {
            ServiceStorage = new GameObject("ServiceStorage");
            DontDestroyOnLoad(ServiceStorage);
        }

        private static GameObject ServiceStorage;
        public static void InitializeAllServices()
        {
            //FindObjectsOfType<IService>();
            GenerateServiceStorage();
            var services = ECSAssemblyExtensions.GetAllSubclassOf(typeof(IService)).Where(x => !x.IsAbstract).Select(x => IService.InitalizeSingleton(x, ServiceStorage)).Cast<IService>().ToList();
            services.ForEach(x => x.PostInitializeProcess());
        }
        #endregion
    }
}