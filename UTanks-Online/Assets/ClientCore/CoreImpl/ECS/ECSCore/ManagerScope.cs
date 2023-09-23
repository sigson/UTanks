using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.ECSCore
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
            ECSComponentManager.IdStaticCache(); //moved to client init
            eventManager = new ECSEventManager();
            eventManager.IdStaticCache();
            systemManager = new ECSSystemManager();
            systemManager.InitializeSystems();
            eventManager.InitializeEventManager();
            TaskEx.RunAsync(() => {
                while (true)
                {
                    systemManager.RunSystems();
                    Task.Delay(5).Wait();

                    //if(!ClientInitService.instance.StopECS)
                    //{
                    //}
                    //else
                    //{
                    //    Task.Delay(1000).Wait();
                    //}
                }
            });
            ULogger.LogLoad("3/5/Core loaded...");
        }
    }
}
