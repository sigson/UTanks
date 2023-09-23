using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.ECSCore
{
    public class ECSEntityManager
    {

        public ConcurrentDictionary<long, ECSEntity> EntityStorage = new ConcurrentDictionary<long, ECSEntity>();
        public ConcurrentDictionary<string, ECSEntity> PreinitializedEntities = new ConcurrentDictionary<string, ECSEntity>();//for selectablemap, shopdb, ect.

        public void OnAddNewEntity(ECSEntity Entity)
        {
            Entity.manager = this;
            EntityStorage.TryAdd(Entity.instanceId, Entity);
            TaskEx.RunAsync(() => {
                Entity.entityComponents.RegisterAllComponents();
                ManagerScope.systemManager.OnEntityCreated(Entity);
            });
        }

        public void OnRemoveEntity(ECSEntity Entity)
        {
            TaskEx.RunAsync(() => {
                EntityStorage.TryRemove(Entity.instanceId, out Entity);
                Entity.OnDelete();
                ManagerScope.systemManager.OnEntityDestroyed(Entity);
            });
        }


        public void OnAddComponent(ECSEntity Entity, ECSComponent Component)
        {
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    //if(Component.Unregistered == true)
                    //{
                    //    Component.OnAdded(Entity);
                    //}
                    ManagerScope.systemManager.OnEntityComponentAddedReaction(Entity, Component);
                }).LogExceptionIfFaulted();
            };
            asyncUpd();
        }

        public void OnRemoveComponent(ECSEntity Entity, ECSComponent Component)
        {
            TaskEx.RunAsync(() => ManagerScope.systemManager.OnEntityComponentRemovedReaction(Entity, Component));
        }

        public void OnAddEntityGroup(ECSEntity Entity, ECSComponentGroup EntityGroup)
        {

        }
        public void OnRemoveEntityGroup(ECSEntity Entity, ECSComponentGroup EntityGroup)
        {

        }


        void ScheduleEvent<T>() where T : ECSEvent, new()
        {

        }
        void ScheduleEvent(ECSEvent eventInstance)
        {

        }
        //T SendEvent<T>(T eventInstance) where T : IEvent
        //{

        //}

        //void AddComponent<T>() where T : ECSComponent, new()
        //{

        //}

        //dynamic CreateNewComponentInstance(dynamic Entity, Type ComponentType)
        //{
        //    return new ECSComponent();
        //}
        //ECSComponent GetComponent(ECSEntity Entity, Type ComponentType)
        //{
        //    return new BonusComponent();
        //}
        //bool HasComponent<T>()
        //{
        //    return true;
        //}
        //bool HasComponent(dynamic Entity, Type type)
        //{
        //    return true;
        //}
        //bool IsSameGroup(dynamic mainEntity, dynamic otherEntity)
        //{
        //    return true;
        //}

        //void RemoveComponent(dynamic Entity, Type ComponentType)
        //{

        //}
        //void RemoveComponentIfPresent(dynamic Entity, dynamic Component)
        //{

        //}
    }
}
