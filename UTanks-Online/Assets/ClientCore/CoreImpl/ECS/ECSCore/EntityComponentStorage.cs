using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.ECSCore
{
    public class EntityComponentStorage
    {
        private ECSEntity entity;
        public static Type StorageType;
		public int ChangedComponent => changedComponents.Count;
        //private ComponentBitIdRegistry componentBitIdRegistry;
        private readonly IDictionary<Type, ECSComponent> components = new ConcurrentDictionary<Type, ECSComponent>();
        private readonly IDictionary<Type, int> changedComponents = new ConcurrentDictionary<Type, int>();
        public readonly IDictionary<long, Type> IdToTypeComponent = new ConcurrentDictionary<long, Type>();
        public ConcurrentDictionary<long, object> SerializationContainer = new ConcurrentDictionary<long, object>();
        public List<long> RemovedComponents = new List<long>();
        public object serializationLocker = new object();//ReaderWriterLockSlim();
		public object operationLocker = new object();
        public EntityComponentStorage(ECSEntity entity)
        {
            this.entity = entity;
            if(StorageType != null)
                StorageType = SerializationContainer.GetType();
        }
		
		public bool CheckChanged(Type typeComponent) => changedComponents.Keys.Contains(typeComponent);
        public void DirectiveChange(Type typeComponent)
        {
            lock (this.serializationLocker)
            {
                lock (this.operationLocker)
                {
                    if (components.Keys.Contains(typeComponent))
                    {
                        changedComponents[typeComponent] = 1;
                    }

                }

            }
        }

        public void RestoreComponentsAfterSerialization(ECSEntity entity)
        {
            this.entity = entity;
            if (components.Count == 0)
            {
                foreach (var objPair in SerializationContainer)
                {
                    ECSComponent objComponent = (ECSComponent)objPair.Value;
                    ECSComponent component;
                    if(ECSComponentManager.AllComponents.TryGetValue(objPair.Key, out component))
                    {
                        var typedComponent = (ECSComponent)Convert.ChangeType(objPair.Value, component.GetTypeFast());
                        if (typedComponent is DBComponent)
                            TaskEx.RunAsync(() => (typedComponent as DBComponent).UnserializeDB());
                        AddComponentImmediately(component.GetTypeFast(), typedComponent, true, true);
                    }
                }
            }
        }

        public void AddComponentImmediately(Type comType, ECSComponent component, bool restoringMode = false, bool silent = false)
        {
            bool exception = false;
            lock (this.serializationLocker)///////try
            {
                lock (this.operationLocker)
                {
                    if (this.components.Keys.Contains(comType))
                    {
                        exception = true;
                    }
                    else
                    {
                        component.ownerEntity = this.entity;
                        this.components.Add(comType, component);
                        if (this.entity != null)
                            this.entity.fastEntityComponentsId.TryAdd(component.instanceId, 0);
                        else
                            ULogger.Error("null owner entity");
                        if (restoringMode)
                            this.SerializationContainer.TryAdd(component.GetId(), component);
                        else
                            this.SerializationContainer[component.GetId()] = component;
                        this.IdToTypeComponent.Add(component.GetId(), component.GetTypeFast());
                    }


                }


                //this.bitId.Set(this.componentBitIdRegistry.GetComponentBitId(comType));
            }
            if (exception)
            {
                ULogger.Log("try add presented component");
                throw new Exception("try add presented component");
            }
            else
            {
                if (!silent)
                {
                    component.Unregistered = false;
                    component.OnAdded(this.entity);
                }
                //////catch (ArgumentException)
                {
                    //throw new ComponentAlreadyExistsInEntityException(this.entity, comType);
                }
            }
        }

        public void RegisterAllComponents()
        {
            foreach(var component in components)
            {
                if(component.Value.Unregistered)
                {
                    component.Value.Unregistered = false;
                    component.Value.OnAdded(entity);
					this.entity.manager.OnAddComponent(this.entity, component.Value);
                }
            }
        }

        public void AddComponentsImmediately(IList<ECSComponent> addedComponents)
        {
            addedComponents.ForEach<ECSComponent>(component => this.AddComponentImmediately(component.GetTypeFast(), component));
        }

        private void AssertComponentFound(Type componentClass)
        {
            if (!this.components.ContainsKey(componentClass))
            {
                //throw new ComponentNotFoundException(this.entity, componentClass);
            }
        }

        public void MarkComponentChanged(ECSComponent component, bool silent = false)
        {
            lock (this.serializationLocker)
            {
                lock (this.operationLocker)
                {
                    Type componentClass = component.GetTypeFast();
                    //this.AssertComponentFound(componentClass);
                    if (!silent)
                    {
                        changedComponents[componentClass] = 1;
                    }
                    //else
                    //{
                    //    component.Unregistered = true;
                    //}

                }

            }
            TaskEx.RunAsync(() => {
                component.RunOnChangeCallbacks(this.entity);
            });
        }

        public void ChangeComponent(ECSComponent component, bool silent = false, ECSEntity restoringOwner = null)
        {
            Type componentClass = component.GetTypeFast();
            //this.AssertComponentFound(componentClass);
            lock (this.operationLocker)
            {
                if (restoringOwner != null)
                    component.ownerEntity = restoringOwner;
                component.Unregistered = false;//for afterserialize changing
                this.components[componentClass] = component;
            }


            this.MarkComponentChanged(component, silent);
        }

        public ECSComponent GetComponent(Type componentClass)
        {
            ECSComponent component = null;
            try
            {
                component = this.components[componentClass];
            }
            catch (Exception ex)
            {
                ULogger.Log(componentClass.ToString() + ex.Message);
                //throw new ComponentAlreadyExistsInEntityException(this.entity, comType);
            }
            return component;
        }

        public ECSComponent GetComponent(long componentTypeId)
        {
            ECSComponent component = null;
            try
            {
                //var jit = this.IdToTypeComponent[componentTypeId];
                component = this.components[this.IdToTypeComponent[componentTypeId]];
            }
            catch (Exception ex)
            {
                ULogger.Log(componentTypeId.ToString() + ex.Message);
                //throw new ComponentAlreadyExistsInEntityException(this.entity, comType);
            }
            return component;
        }

        public ECSComponent GetComponentUnsafe(Type componentType)
        {
            ECSComponent component;
            return (!this.components.TryGetValue(componentType, out component) ? null : component);
        }

        public ECSComponent GetComponentUnsafe(long componentTypeId)
        {
            ECSComponent component;
            return (!this.components.TryGetValue(this.IdToTypeComponent[componentTypeId], out component) ? null : component);
        }

        public bool HasComponent(Type componentClass) =>
            this.components.ContainsKey(componentClass);

        public bool HasComponent(long componentClassId) =>
            this.IdToTypeComponent.ContainsKey(componentClassId);

        public void OnEntityDelete()
        {
            lock (this.operationLocker)
            {
                foreach (var component in this.components.Values)
                {
                    component.OnRemove();
                }
                this.components.Clear();
                this.SerializationContainer.Clear();
                this.IdToTypeComponent.Clear();
                this.changedComponents.Clear();
                this.RemovedComponents.Clear();
                this.IdToTypeComponent.Clear();
            }
            
												
										   
            //this.bitId.Clear();
        }

        public ECSComponent RemoveComponentImmediately(long componentTypeId)
        {
            return RemoveComponentImmediately(this.IdToTypeComponent[componentTypeId]);
        }

        public void RemoveComponentsWithGroup(long componentGroup)
        {
            List<ECSComponent> toRemoveComponent = new List<ECSComponent>();
            List<ECSComponent> notRemovedComponent = new List<ECSComponent>();
            bool exception = false;
            lock (this.serializationLocker)
            {
                foreach (var component in components)
                {
                    if (component.Value.ComponentGroups.TryGetValue(componentGroup, out _))
                    {
                        toRemoveComponent.Add(component.Value);
                    }
                }
                lock (this.operationLocker)
                {
                    toRemoveComponent.ForEach((removedComponent) =>
                    {
                        if (!this.components.Keys.Contains(removedComponent.GetTypeFast()))
                        {
                            exception = true;
                            notRemovedComponent.Add(removedComponent);
                        }
                        else
                        {
                            this.changedComponents.Remove(removedComponent.GetTypeFast());
                            this.entity.fastEntityComponentsId.TryRemove(removedComponent.instanceId, out _);
                            this.components.Remove(removedComponent.GetTypeFast());
                            this.SerializationContainer.TryRemove(removedComponent.GetId(), out _);
                            this.IdToTypeComponent.Remove(removedComponent.GetId());
                            this.RemovedComponents.Add(removedComponent.GetId());
                            ManagerScope.entityManager.OnRemoveComponent(this.entity, removedComponent);
                        }
                    });
                }
            }
            if (exception)
            {
                ULogger.Error("try to remove non present component in group removing");
            }
            toRemoveComponent.ForEach((removedComponent) =>
            {
                if (!notRemovedComponent.Contains(removedComponent))
                    removedComponent.OnRemoving(this.entity);
            });
        }

        public void FilterRemovedComponents(List<long> filterList, List<long> filteringOnlyGroups)
        {
            var bufFilterList = new List<long>(filterList);
            foreach(var component in this.components)
            {
                if(filteringOnlyGroups.Count == 0)
                {
                    var id = component.Value.instanceId;
                    bool finded = false;
                    int i;
                    for (i = 0; i < bufFilterList.Count; i++)
                    {
                        if (id == bufFilterList[i])
                        {
                            finded = true;
                        }
                    }
                    if (!finded)
                    {
                        this.RemoveComponentImmediately(component.Key);
                        ULogger.Log(component.Key);
                    }
                }
                else
                {
                    foreach (var group in filteringOnlyGroups)
                    {
                        foreach(var componentGroup in component.Value.ComponentGroups)
                        {
                            if (componentGroup.Key == group)
                            {
                                var id = component.Value.instanceId;
                                bool finded = false;
                                int i;
                                for (i = 0; i < bufFilterList.Count; i++)
                                {
                                    if (id == bufFilterList[i])
                                    {
                                        finded = true;
                                    }
                                }
                                if (!finded)
                                {
                                    this.RemoveComponentImmediately(component.Key);
                                    ULogger.Log(component.Key);
                                    //bufFilterList.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        public ECSComponent RemoveComponentImmediately(Type componentClass)
        {
            ECSComponent component2 = null;
            bool exception = false;
            lock (this.serializationLocker)//////////try
            {
                lock (this.operationLocker)
                {
                    if (!this.components.Keys.Contains(componentClass))
                    {
                        exception = true;
                    }
                    else
                    {
                        ECSComponent component = this.components[componentClass];
                        this.changedComponents.Remove(componentClass);
                        this.components.Remove(componentClass);
                        this.SerializationContainer.TryRemove(component.GetId(), out _);
                        this.IdToTypeComponent.Remove(component.GetId());
                        this.entity.fastEntityComponentsId.TryRemove(component.instanceId, out _);
                        this.RemovedComponents.Add(component.GetId());
                        ManagerScope.entityManager.OnRemoveComponent(this.entity, component);
                        //this.bitId.Clear(this.componentBitIdRegistry.GetComponentBitId(componentClass));
                        component2 = component;
                    }

                }

            }
            if (exception)
            {
                ULogger.Error("try to remove non present component");
                throw new Exception("try to remove non present component");
            }
            else
            {
                component2.OnRemoving(this.entity);
            }
            //////////catch (KeyNotFoundException)
            {
                //throw new ComponentNotFoundException(this.entity, componentClass);
            }
            return component2;
        }

        //public BitSet bitId { get; private set; }

        public ICollection<Type> ComponentClasses =>
            this.components.Keys;

        public ICollection<ECSComponent> Components =>
            this.components.Values;
    }
}
