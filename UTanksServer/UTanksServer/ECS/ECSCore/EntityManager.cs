using Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.Bonus;

namespace UTanksServer.ECS.ECSCore
{
    public class ECSEntityManager
    {

        public ConcurrentDictionary<long, ECSEntity> EntityStorage = new ConcurrentDictionary<long, ECSEntity>();
        public ConcurrentDictionary<string, ECSEntity> PreinitializedEntities = new ConcurrentDictionary<string, ECSEntity>();//for selectablemap, shopdb, ect.

        public void OnAddNewEntity(ECSEntity Entity)
        {
            Entity.manager = this;
            if (!EntityStorage.TryAdd(Entity.instanceId, Entity))
                Logger.LogError("error add entity to storage");
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    Entity.entityComponents.RegisterAllComponents();
                    ManagerScope.systemManager.OnEntityCreated(Entity);
                });
            };
            asyncUpd();
        }

        public void OnRemoveEntity(ECSEntity Entity)
        {
            EntityStorage.Remove(Entity.instanceId, out Entity);
            Entity.OnDelete();
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    ManagerScope.systemManager.OnEntityDestroyed(Entity);
                });
            };
            asyncUpd();
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
                });
            };
            asyncUpd();
        }

        public void OnRemoveComponent(ECSEntity Entity, ECSComponent Component)
        {
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => ManagerScope.systemManager.OnEntityComponentRemovedReaction(Entity, Component));
            };
            asyncUpd();
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
