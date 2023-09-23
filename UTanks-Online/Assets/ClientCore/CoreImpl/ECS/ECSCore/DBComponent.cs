using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient.Core.Logging;
using UTanksClient.Extensions;
using static UTanksClient.ECS.ECSCore.ComponentsDBComponent;

namespace UTanksClient.ECS.ECSCore
{
    [TypeUid(224172216465378980)]
    public class DBComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public Dictionary<long, List<dbRow>> serializedDB = new Dictionary<long, List<dbRow>>();

        public virtual void SerializeDB(bool serializeOnlyChanged = false, bool clearChanged = true)
        {

        }

        public virtual void AfterSerializationDB()
        {

        }

        public virtual void UnserializeDB()
        {

        }

        public virtual void AfterDeserialize()
        { }
    }

    public class dbRow
    {
        [NonSerialized]
        public long componentInstanceId;
        public long componentId;
        public object component;
        public ComponentState componentState;
    }

    [TypeUid(230758971479671600)]
    public class ComponentsDBComponent : DBComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public enum ComponentState
        {
            Created,
            Changed,
            Removed,
            Null
        }

        [NonSerialized]
        public Dictionary<long, Dictionary<long, (ECSComponent, ComponentState)>> DB = new Dictionary<long, Dictionary<long, (ECSComponent, ComponentState)>>();
        [NonSerialized]
        public Dictionary<long, long> ComponentOwners = new Dictionary<long, long>();
        [NonSerialized]
        public Dictionary<long, int> ChangedComponents = new Dictionary<long, int>();

        #region add methods

        public virtual void AddComponent(ECSEntity ownerComponent, ECSComponent component)
        {
            Dictionary<long, (ECSComponent, ComponentState)> components = new Dictionary<long, (ECSComponent, ComponentState)>();
            ECSComponent addedComponent = null;
            lock (ownerEntity.entityComponents.serializationLocker)
            {
                lock (this.locker)
                {
                    DB.TryGetValue(ownerComponent.instanceId, out components);
                    if (components == null)
                        components = new Dictionary<long, (ECSComponent, ComponentState)>();
                    component.ownerEntity = ownerComponent;
                    components[component.instanceId] = (component, ComponentState.Created);
                    DB[ownerComponent.instanceId] = components;
                    ComponentOwners[component.instanceId] = ownerComponent.instanceId;
                    ChangedComponents[component.instanceId] = 1;
                    addedComponent = component;
                }
            }
            addedComponent.OnAdded(addedComponent.ownerEntity);
        }
        public virtual void AddOrChangeComponent(ECSEntity ownerComponent, ECSComponent component)
        {
            Dictionary<long, (ECSComponent, ComponentState)> components = new Dictionary<long, (ECSComponent, ComponentState)>();
            bool change = false;
            lock (ownerEntity.entityComponents.serializationLocker)
            {
                lock (this.locker)
                {
                    DB.TryGetValue(ownerComponent.instanceId, out components);
                    if (components == null)
                        components = new Dictionary<long, (ECSComponent, ComponentState)>();
                    if (components.ContainsKey(component.instanceId))
                    {
                        change = true;
                    }
                    else
                    {
                        lock (ownerEntity.entityComponents.serializationLocker)
                        {
                            component.ownerEntity = ownerComponent;
                            components[component.instanceId] = (component, ComponentState.Created);
                            ComponentOwners[component.instanceId] = ownerComponent.instanceId;
                            ChangedComponents[component.instanceId] = 1;
                        }
                    }
                    DB[ownerComponent.instanceId] = components;
                }
            }
            if(change)
                ChangeComponent(component, ownerComponent);
            else
                component.OnAdded(ownerComponent);
        }
        public virtual void AddComponents(ECSEntity ownerComponent, params ECSComponent[] component)
        {
            foreach (var comp in component)
            {
                AddComponent(ownerComponent, comp);
            }
        }
        public virtual void AddComponents(ECSEntity ownerComponent, List<ECSComponent> component)
        {
            foreach (var comp in component)
            {
                AddComponent(ownerComponent, comp);
            }
        }

        public virtual void AddComponent(long ownerComponentId, ECSComponent component)
        {
            AddComponent(ManagerScope.entityManager.EntityStorage[ownerComponentId], component);
        }
        public virtual void AddOrChangeComponent(long ownerComponentId, ECSComponent component)
        {
            AddOrChangeComponent(ManagerScope.entityManager.EntityStorage[ownerComponentId], component);
        }
        public virtual void AddComponents(long ownerComponentId, params ECSComponent[] component)
        {
            foreach (var comp in component)
            {
                AddComponent(ManagerScope.entityManager.EntityStorage[ownerComponentId], comp);
            }
        }
        public virtual void AddComponents(long ownerComponentId, List<ECSComponent> component)
        {
            foreach (var comp in component)
            {
                AddComponent(ManagerScope.entityManager.EntityStorage[ownerComponentId], comp);
            }
        }

        #endregion

        #region edit methods
        public virtual (ECSComponent, ComponentState) GetComponent(long componentId, ECSEntity ownerComponent = null)
        {
            lock (this.locker)
            {
                long owner = 0;
                if (ownerComponent == null)
                {
                    if (!ComponentOwners.TryGetValue(componentId, out owner))
                    {
                        ULogger.Error("error get component from db");
                        return (null, ComponentState.Null);
                    }
                }
                else
                {
                    owner = ownerComponent.instanceId;
                }
                (ECSComponent, ComponentState) comp;
                if (DB[owner].TryGetValue(componentId, out comp))
                    return comp;
                else
                {
                    ULogger.Error("error get component from db");
                    return (null, ComponentState.Null);
                }
            }
        }
        public virtual List<(ECSComponent, ComponentState)> GetComponentsByType(List<long> componentTypeId, ECSEntity ownerComponent = null)
        {
            List<(ECSComponent, ComponentState)> result = new List<(ECSComponent, ComponentState)>();
            lock (this.locker)
            {
                List<long> owners = new List<long>();
                if (ownerComponent == null)
                {
                    owners = DB.Keys.ToList();
                }
                else
                {
					if (DB.ContainsKey(ownerComponent.instanceId))
                        owners.Add(ownerComponent.instanceId);
                }
                foreach (var dbOwner in owners)
                {
                    var components = DB[dbOwner];
                    foreach (var comp in components)
                    {
                        if (componentTypeId.Contains(comp.Value.Item1.GetId()))
                        {
                            result.Add(comp.Value);
                        }
                    }
                }
            }
            return result;
        }

        public virtual void ChangeComponent(ECSComponent component, ECSEntity ownerComponent = null)
        {
            if (!ComponentOwners.ContainsKey(component.instanceId))
            {
                ULogger.Error("error change component from db");
                return;
            }
            lock (ownerEntity.entityComponents.serializationLocker)
            {
                lock (this.locker)
                {
                    long owner = 0;
                    if (ownerComponent == null)
                    {
                        if (!ComponentOwners.TryGetValue(component.instanceId, out owner))
                        {
                            ULogger.Error("error change component from db");
                        }
                    }
                    else
                        owner = ownerComponent.instanceId;
                    DB[owner][component.instanceId] = (component, ComponentState.Changed);
                    ChangedComponents[component.instanceId] = 1;
                }
            }

        }
        #endregion

        #region remove methods

        public virtual void RemoveComponent(long componentId, ECSEntity ownerComponent = null)
        {
            if (!ComponentOwners.ContainsKey(componentId))
            {
                ULogger.Error("error remove component from db");
                return;
            }
			ECSComponent removedComponent = null;
            lock (ownerEntity.entityComponents.serializationLocker)
            {
                lock (this.locker)
                {
                    long owner = 0;
                    if (ownerComponent == null)
                    {
                        if (!ComponentOwners.TryGetValue(componentId, out owner))
                        {
                            ULogger.Error("error remove component from db");
                        }
                    }
                    else
                        owner = ownerComponent.instanceId;
                    (ECSComponent, ComponentState) comp;
                    if (DB[owner].TryGetValue(componentId, out comp))
                    {
                        DB[owner][componentId] = (comp.Item1, ComponentState.Removed);
                        ChangedComponents[componentId] = 1;
                        removedComponent = comp.Item1;
                    }
                    else
                    {
                        ULogger.Error("error remove component from db");
                    }
                }
            }
            if(removedComponent != null)
                removedComponent.OnRemoving(removedComponent.ownerEntity);
        }
        public virtual void RemoveComponent(params long[] componentsId)
        {
            foreach (var comp in componentsId)
            {
                RemoveComponent(comp);
            }
        }
        public virtual void RemoveComponent(List<long> componentsId, ECSEntity ownerComponent = null)
        {
            foreach (var comp in componentsId)
            {
                RemoveComponent(comp);
            }
        }
        public virtual void RemoveComponent(List<ECSComponent> components, ECSEntity ownerComponent = null)
        {
            foreach (var comp in components)
            {
                RemoveComponent(comp.instanceId);
            }
        }
        public virtual void RemoveComponentsByType(List<long> componentTypeId, List<ECSEntity> ownerComponent = null)
        {
            List<ECSComponent> removedComponents = new List<ECSComponent>();
            lock (ownerEntity.entityComponents.serializationLocker)
            {
                lock (this.locker)
                {
                    List<long> owners = new List<long>();
                    if (ownerComponent == null)
                    {
                        owners = DB.Keys.ToList();
                    }
                    else
                    {
                        ownerComponent.ForEach(x =>
                        {
                            if (DB.ContainsKey(x.instanceId))
                                owners.Add(x.instanceId);
                        });
                    }
                    foreach (var dbOwner in owners)
                    {
                        var components = DB[dbOwner];
                        List<(ECSComponent, ComponentState)> removeList = new List<(ECSComponent, ComponentState)>();
                        foreach (var comp in components)
                        {
                            if (componentTypeId.Contains(comp.Value.Item1.GetId()))
                            {
                                removeList.Add(comp.Value);
                            }
                        }
                        foreach (var removedComp in removeList)
                        {
                            components[removedComp.Item1.instanceId] = (removedComp.Item1, ComponentState.Removed);
                            ChangedComponents[removedComp.Item1.instanceId] = 1;
                            removedComponents.Add(removedComp.Item1);
                        }
                        DB[dbOwner] = components;
                    }
                }
            }
            removedComponents.ForEach(x => x.OnRemoving(x.ownerEntity));
        }
        #endregion

        public override void SerializeDB(bool serializeOnlyChanged = false, bool clearChanged = true)
        {
            lock (this.locker)
            {
                serializedDB.Clear();
                List<long> errorChanged = new List<long>();
                if (serializeOnlyChanged)
                {
                    Dictionary<long, List<dbRow>> serializedComp = new Dictionary<long, List<dbRow>>();

                    foreach (var changedComponent in ChangedComponents)
                    {
                        try
                        {
                            var ownerId = ComponentOwners[changedComponent.Key];
                            var component = DB[ownerId][changedComponent.Key];

                            List<dbRow> components = null;
                            serializedComp.TryGetValue(ownerId, out components);
                            if (components == null)
                                components = new List<dbRow>();
                            components.Add(new dbRow()
                            {
                                component = component.Item1,
                                componentInstanceId = component.Item1.instanceId,
                                componentId = component.Item1.GetId(),
                                componentState = component.Item2
                            });
                            serializedComp[ownerId] = components;
                        }
                        catch (Exception ex)
                        {
                            errorChanged.Add(changedComponent.Key);
                        }
                    }
                    serializedDB = serializedComp;
                }
                else
                {
                    foreach (var entityRow in DB)
                    {
                        if (entityRow.Value == null)
                            continue;
                        List<dbRow> components = new List<dbRow>();
                        var entityRowValues = entityRow.Value.Values.ToList();
                        for (int i = 0; i < entityRowValues.Count; i++)
                        {
                            var ecsComponent = entityRowValues[i];
                            components.Add(new dbRow()
                            {
                                component = ecsComponent.Item1,
                                componentInstanceId = ecsComponent.Item1.instanceId,
                                componentId = ecsComponent.Item1.GetId(),
                                componentState = ecsComponent.Item2
                            });

                        }
                        serializedDB[entityRow.Key] = components;
                    }
                }
                if (clearChanged)
                    ChangedComponents.Clear();
                errorChanged.ForEach(x => ChangedComponents[x] = 1);
            }
        }

        public override void AfterSerializationDB()
        {
            lock (this.locker)
            {
                foreach (var entityRow in serializedDB)
                {
                    var entityRowValues = entityRow.Value.ToList();
                    for (int i = 0; i < entityRowValues.Count; i++)
                    {
                        var ownerList = DB[entityRow.Key];
                        var ecsComponent = ownerList[entityRowValues[i].componentInstanceId];
                        if (ecsComponent.Item2 == ComponentState.Removed)
                        {
                            ecsComponent.Item1.OnRemoving(ecsComponent.Item1.ownerEntity);
                            ownerList.Remove(ecsComponent.Item1.instanceId);
                            //i--;
                        }
                    }
                }
            }
        }

        public override void UnserializeDB()
        {
            lock(serializedDB)//race fix
            {
                ChangedComponents.Clear();
                foreach (var serializedRow in serializedDB)
                {
                    Dictionary<long, (ECSComponent, ComponentState)> components = new Dictionary<long, (ECSComponent, ComponentState)>();
                    DB.TryGetValue(serializedRow.Key, out components);
                    if (components == null)
                        components = new Dictionary<long, (ECSComponent, ComponentState)>();
                    ECSEntity entityOwner = null;
                    if (ManagerScope.entityManager.EntityStorage.TryGetValue(serializedRow.Key, out entityOwner))
                    {
                        foreach (var component in serializedRow.Value)
                        {
                            var unserComp = (ECSComponent)(component.component as JObject).ToObject(ECSComponentManager.AllComponents[component.componentId].GetTypeFast());
                            component.componentInstanceId = unserComp.instanceId;
                            unserComp.ownerEntity = entityOwner;
                            if (!components.ContainsKey(unserComp.instanceId))
                            {
                                components[unserComp.instanceId] = (unserComp, component.componentState);
                                ComponentOwners[unserComp.instanceId] = entityOwner.instanceId;
                                if (component.componentState != ComponentState.Created)
                                {
                                    unserComp.OnAdded(unserComp.ownerEntity);
                                }
                            }
                            else
                            {
                                unserComp.componentManagers = components[unserComp.instanceId].Item1.componentManagers;
                                components[unserComp.instanceId] = (unserComp, component.componentState);
                                unserComp.componentManagers.ForEach(x => x.Value.ConnectPoint = unserComp);
                            }
                            ChangedComponents[unserComp.instanceId] = 1;
                        }
                        DB[serializedRow.Key] = components;
                    }
                    else
                    {
                        ULogger.Error("error unserialize: no entity");
                    }
                }
                AfterDeserialize();
            }
        }

        public override void AfterDeserialize()
        {
            foreach (var entityRow in serializedDB)
            {
                var entityRowValues = entityRow.Value.ToList();
                for (int i = 0; i < entityRowValues.Count; i++)
                {
                    var ownerList = DB[entityRow.Key];
                    var ecsComponent = ownerList[entityRowValues[i].componentInstanceId];
                    if (ecsComponent.Item2 == ComponentState.Created)
                    {
                        ecsComponent.Item1.OnAdded(ecsComponent.Item1.ownerEntity);
                        //ownerList.Remove(ecsComponent.Item1.InstanceId);
                        //i--;
                    }
                    if (ecsComponent.Item2 == ComponentState.Changed)
                    {
                        //ecsComponent.Item1.OnAdded(ecsComponent.Item1.ownerEntity);
                        TaskEx.RunAsync(() =>
                        {
                            ecsComponent.Item1.RunOnChangeCallbacks(ecsComponent.Item1.ownerEntity);
                        });
                        //i--;
                    }
                    if (ecsComponent.Item2 == ComponentState.Removed)
                    {
                        ecsComponent.Item1.OnRemoving(ecsComponent.Item1.ownerEntity);
                        ownerList.Remove(ecsComponent.Item1.instanceId);
                        ComponentOwners.Remove(ecsComponent.Item1.instanceId);
                        //i--;
                    }
                }
            }
        }
    }
}
