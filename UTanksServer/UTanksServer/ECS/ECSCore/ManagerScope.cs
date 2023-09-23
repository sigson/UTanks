using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UTanksServer.ECS.ECSCore
{
    public static class ManagerScope
    {
        public static ECSSystemManager systemManager;
        public static ECSEntityManager entityManager;
        public static ECSComponentManager componentManager;
        public static ECSEventManager eventManager;

        public static void InitManagerScope()
        {
            entityManager = new ECSEntityManager();
            componentManager = new ECSComponentManager();
            ECSComponentManager.IdStaticCache();
            eventManager = new ECSEventManager();
            eventManager.IdStaticCache();
            systemManager = new ECSSystemManager();
            systemManager.InitializeSystems();
            eventManager.InitializeEventManager();
            Func<Task> asyncSystems = async () =>
            {
                await Task.Run(() => {
                    while(true)
                    {
                        systemManager.RunSystems();
                        Thread.Sleep(5);
                    }
                });
            };
            asyncSystems();
            Logger.Log("ECS managers initialized");
        }
    }
}
