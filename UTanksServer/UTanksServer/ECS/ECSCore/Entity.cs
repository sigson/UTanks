using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UTanksServer.ECS.ECSCore
{
    [TypeUid(200155515534159360)]//base type of entity
    public class ECSEntity : CachingSerializable, ICloneable
    {
        static public long Id = 200155515534159360;
        public long instanceId = Guid.NewGuid().GuidToLong();
        [NonSerialized]
        public List<long> TemplateAccessorId = new List<long>();

        [NonSerialized]
        public ECSEntityManager manager;
        [NonSerialized]
        public ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        [NonSerialized]
        public object contextSwitchLocker = new object();
        [ServerOnlyData]
        [NonSerialized]
        public ConcurrentDictionary<long, ECSEntityGroup> entityGroups;//ECSEntityGroup
        [NonSerialized]
        public EntityComponentStorage entityComponents;//ECSComponent
        //[NonSerialized]
        public ConcurrentDictionary<long, int> fastEntityComponentsId;
        [NonSerialized]
        public List<GroupDataAccessPolicy> dataAccessPolicies;
        [NonSerialized]
        public string Name;
        [NonSerialized]
        public string serializedEntity;
        [NonSerialized]
        public bool emptySerialized = true;
        [JsonIgnore]
        public List<string> ConfigPath { get; }
        [NonSerialized]
        public bool Alive;

        public ECSEntity() {
            entityComponents = new EntityComponentStorage(this);
            fastEntityComponentsId = new ConcurrentDictionary<long, int>();
            dataAccessPolicies = new List<GroupDataAccessPolicy>();
            entityGroups = new ConcurrentDictionary<long, ECSEntityGroup>();
            //this.instanceId = Guid.NewGuid().GuidToLong();
        }

        public ECSEntity(EntityTemplate userTemplate, ECSComponent[] eCSComponents)
        {
            //this.instanceId = Guid.NewGuid().GuidToLong();
            entityComponents = new EntityComponentStorage(this);
            fastEntityComponentsId = new ConcurrentDictionary<long, int>();
            dataAccessPolicies = new List<GroupDataAccessPolicy>();
            entityGroups = new ConcurrentDictionary<long, ECSEntityGroup>();
            foreach (var component in eCSComponents)
            {
                this.AddComponentSilent(component);
            }
            this.TemplateAccessorId.Add(userTemplate.GetId());
        }
        public void AddComponent<T>() where T : ECSComponent, new()
        {
            this.AddComponent(typeof(T));
        }

        public void AddComponent(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            this.AddComponentImpl(component, true);
        }

        public bool TryAddComponent(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            try
            {
                if (!this.HasComponent(component.GetId()))
                {
                    this.AddComponentImpl(component, true);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public void AddComponents(IEnumerable<ECSComponent> components)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            foreach(var component in components)
            {
                this.AddComponentImpl(component, true);
            }
        }

        public void AddComponentsSilent(IEnumerable<ECSComponent> components)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            foreach (var component in components)
            {
                this.AddComponentSilent(component);
            }
        }

        public void AddComponent(Type componentType)
        {
            ECSComponent component = this.CreateNewComponentInstance(componentType);
            this.AddComponent(component);
        }

        public long GetId()
        {
            return Id;
        }
        private void AddComponentImpl(ECSComponent component, bool sendEvent)
        {
            Type componentClass = component.GetTypeFast();
            if (!this.entityComponents.HasComponent(componentClass))//|| !this.IsSkipExceptionOnAddRemove(componentClass)
            {
                this.entityComponents.AddComponentImmediately(component.GetTypeFast(), component, false, !sendEvent);
                
                //this.MakeNodes(component.GetType(), component);
                if (sendEvent)
                {
                    //this.entityComponents.AddComponentImmediately(component.GetType(), component);
                    this.manager.OnAddComponent(this, component);
                }
                //this.PrepareAndSendNodeAddedEvent(component);
            }
            else
            {
                new Exception();
            }
        }

        public void AddOrChangeComponent(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            this.AddOrChangeComponentImpl(component, true);
        }

        public void AddOrChangeComponentWithOwnerRestoring(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            this.AddOrChangeComponentImpl(component, true, true);
        }

        public void AddOrChangeComponentSilentWithOwnerRestoring(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            this.AddOrChangeComponentImpl(component, false, true);
        }

        public void AddOrChangeComponentSilent(ECSComponent component)
        {
            //if (component is ECSComponentGroup)
            //{
            //    component = GroupRegistry.FindOrRegisterGroup((ECSComponentGroup)component);
            //}
            this.AddOrChangeComponentImpl(component, false);
        }
        private void AddOrChangeComponentImpl(ECSComponent component, bool sendEvent, bool restoringOwner = false)
        {
            Type componentClass = component.GetTypeFast();
            if (!this.entityComponents.HasComponent(componentClass))//|| !this.IsSkipExceptionOnAddRemove(componentClass)
            {
                //if(!this.fastEntityComponentsId.TryAdd(component.InstanceId, 0))
                //{
                //    Logger.LogError
                //}
                this.entityComponents.AddComponentImmediately(component.GetTypeFast(), component, false, !sendEvent);
                
                //this.MakeNodes(component.GetType(), component);
                if (sendEvent)
                {
                    //this.entityComponents.AddComponentImmediately(component.GetType(), component);
                    this.manager.OnAddComponent(this, component);
                }
                //this.PrepareAndSendNodeAddedEvent(component);
            }
            else
            {
                if (restoringOwner)
				{
					if (component is DBComponent)
                        this.GetComponent<DBComponent>(component.GetId()).serializedDB = (component as DBComponent).serializedDB;
                    else
                        this.entityComponents.ChangeComponent(component, false, this);
				}
                else
                    this.entityComponents.ChangeComponent(component);
            }
        }

        public T AddComponentAndGetInstance<T>() where T : ECSComponent, new()
        {
            ECSComponent component = this.CreateNewComponentInstance(typeof(T));
            this.AddComponent(component);
            return (T) component;
        }

        public void AddComponentIfAbsent<T>() where T : ECSComponent, new()
        {
            if (!this.HasComponent<T>())
            {
                this.AddComponent(typeof(T));
            }
        }

        public void AddComponentSilent(ECSComponent component)
        {
            this.AddComponentImpl(component, false);
        }

        //public void AddEntityListener(EntityListener entityListener)
        //{
        //    this.entityListeners.Add(entityListener);
        //}

        //protected internal void AddNode(NodeDescription node)
        //{
        //    Flow.Current.NodeCollector.Attach(this, node);
        //    this.nodeDescriptionStorage.AddNode(node);
        //    this.SendNodeAddedForCollectors(node);
        //}

        private int calcHashCode() =>
            this.GetHashCode();

        //public bool CanCast(NodeDescription desc) =>
        //    this.nodeProvider.CanCast(desc);

        public void ChangeComponent(ECSComponent component)
        {
            bool flag = this.HasComponent(component.GetTypeFast()) && this.GetComponent(component.GetTypeFast()).Equals(component);
            this.entityComponents.ChangeComponent(component);
            if (!flag)
            {
                //this.nodeProvider.OnComponentChanged(component);
            }
            //this.NotifyChangedInEntity(component);
        }
        public void ChangeComponentSilent(ECSComponent component)//for fast components, who not must autoupdate, because we broadcast his event from user to other users, like moving or shooting
        {
            bool flag = this.HasComponent(component.GetTypeFast()) && this.GetComponent(component.GetTypeFast()).Equals(component);
            this.entityComponents.ChangeComponent(component, true);
            if (!flag)
            {
                //this.nodeProvider.OnComponentChanged(component);
            }
            //this.NotifyChangedInEntity(component);
        }

        //private void ClearNodes()
        //{
        //    new List<NodeDescription>(this.nodeDescriptionStorage.GetNodeDescriptions()).ForEach(new Action<NodeDescription>(this.RemoveNode));
        //    this.nodeProvider.CleanNodes();
        //}

        public int CompareTo(ECSEntity other) =>
            (int)(this.instanceId - other.instanceId);

        //public bool Contains(NodeDescription node)
        //{
        //    BitSet componentsBitId = this.ComponentsBitId;
        //    return (componentsBitId.Mask(node.NodeComponentBitId) && componentsBitId.MaskNot(node.NotNodeComponentBitId));
        //}

        //public T CreateGroup<T>() where T : ECSComponentGroup
        //{
        //    T component = GroupRegistry.FindOrCreateGroup<T>(this.Id);
        //    this.AddComponent(component);
        //    return component;
        //}

        public ECSComponent CreateNewComponentInstance(Type componentType)
        {
            return Activator.CreateInstance(componentType) as ECSComponent;
        }

        protected bool Equals(ECSEntity other) =>
            this.GetId() == other.GetId();

        public override bool Equals(object obj)
        {
            return (!ReferenceEquals(null, obj) ? (!ReferenceEquals(this, obj) ? (ReferenceEquals(obj.GetType(), base.GetType()) ? this.Equals((ECSEntity)obj) : false) : true) : false);
        }

        public T GetComponent<T>() where T : ECSComponent =>
            (T)this.GetComponent(typeof(T));

		public ECSComponent[] GetComponents(params long[] componentTypeId)
        {
            List<ECSComponent> returnComponents = new List<ECSComponent>();
            foreach(var compId in componentTypeId)
            {
                try { returnComponents.Add(this.entityComponents.GetComponent(compId)); } catch { }
            }
            return returnComponents.Where(x => x != null).ToArray();
        }

        public T TryGetComponent<T>() where T : ECSComponent
        {
            try { return (T)this.GetComponent(typeof(T)); } catch { return null; }
        }
            

        public ECSComponent GetComponent(Type componentType) =>
            this.entityComponents.GetComponent(componentType);

        public ECSComponent GetComponent(long componentTypeId) =>
            this.entityComponents.GetComponent(componentTypeId);

        public T GetComponent<T>(long componentTypeId) where T : ECSComponent =>
            (T)this.entityComponents.GetComponent(componentTypeId);

        public ECSComponent GetComponentUnsafe(Type componentType) =>
            this.entityComponents.GetComponentUnsafe(componentType);

        public ECSComponent GetComponentUnsafe(long componentTypeId) =>
            this.entityComponents.GetComponentUnsafe(componentTypeId);

        //public Node GetNode(NodeClassInstanceDescription instanceDescription) =>
        //    this.nodeProvider.GetNode(instanceDescription);

        public bool HasComponent<T>() where T : ECSComponent =>
            this.HasComponent(typeof(T));

        public bool HasComponent(Type type) =>
            this.entityComponents.HasComponent(type);

        public bool HasComponent(long componentClassId) =>
            this.entityComponents.HasComponent(componentClassId);

        public void Init()
        {
            this.Alive = true;
        }

        public bool IsSameGroup<T>(ECSEntity otherEntity) where T : ECSComponentGroup =>
            (this.HasComponent<T>() && otherEntity.HasComponent<T>()) && this.GetComponent<T>().GetId().Equals(otherEntity.GetComponent<T>().GetId());

        //private bool IsSkipExceptionOnAddRemove(Type componentType) =>
        //    componentType.IsDefined(typeof(SkipExceptionOnAddRemoveAttribute), true);

        //protected void MakeNodes(Type componentType, ECSComponent component)
        //{
        //    this.nodeProvider.OnComponentAdded(component, componentType);
        //    NodesToChange nodesToChange = this.nodeCache.GetNodesToChange(this, componentType);
        //    foreach (NodeDescription description in nodesToChange.NodesToAdd)
        //    {
        //        this.AddNode(description);
        //    }
        //    foreach (NodeDescription description2 in nodesToChange.NodesToRemove)
        //    {
        //        this.RemoveNode(description2);
        //    }
        //}

        //private void NotifyAddComponent(ECSComponent component)
        //{
        //    Collections.Enumerator<ComponentListener> enumerator = Collections.GetEnumerator<ComponentListener>(this.engineService.ComponentListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnComponentAdded(this, component);
        //    }
        //}

        //private void NotifyAttachToEntity(ECSComponent component)
        //{
        //    AttachToEntityListener listener = component as AttachToEntityListener;
        //    if (listener != null)
        //    {
        //        listener.AttachedToEntity(this);
        //    }
        //}

        //private void NotifyChangedInEntity(ECSComponent component)
        //{
        //    ComponentServerChangeListener listener = component as ComponentServerChangeListener;
        //    if (listener != null)
        //    {
        //        listener.ChangedOnServer(this);
        //    }
        //}

        //public void NotifyComponentChange(Type componentType)
        //{
        //    Component component = this.GetComponent(componentType);
        //    Collections.Enumerator<ComponentListener> enumerator = Collections.GetEnumerator<ComponentListener>(this.engineService.ComponentListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnComponentChanged(this, component);
        //    }
        //}

        //private void NotifyComponentRemove(Type componentType)
        //{
        //    Component component = this.entityComponents.GetComponent(componentType);
        //    Collections.Enumerator<ComponentListener> enumerator = Collections.GetEnumerator<ComponentListener>(this.engineService.ComponentListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnComponentRemoved(this, component);
        //    }
        //}

        //private void NotifyDetachFromEntity(Component component)
        //{
        //    DetachFromEntityListener listener = component as DetachFromEntityListener;
        //    if (listener != null)
        //    {
        //        listener.DetachedFromEntity(this);
        //    }
        //}

        public void OnDelete()
        {
            this.Alive = false;
            //this.ConfigPath.Clear();
            this.dataAccessPolicies.Clear();
            this.entityComponents.OnEntityDelete();
            this.entityGroups.Clear();
            this.fastEntityComponentsId.Clear();
        }

        private void PrepareAndSendNodeAddedEvent(ECSComponent component)
        {
            //this.nodeAddedEventMaker.MakeIfNeed(this, component.GetType());
        }

        public void RemoveComponent<T>() where T : ECSComponent
        {
            this.RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type componentType)
        {
            //this.UpdateHandlers(componentType);
            //this.NotifyComponentRemove(componentType);
            //this.RemoveComponentSilent(componentType);
            this.entityComponents.RemoveComponentImmediately(componentType);
        }

        public void RemoveComponentsWithGroup(ECSComponentGroup componentGroup)
        {
            this.entityComponents.RemoveComponentsWithGroup(componentGroup.GetId());
        }

        public void RemoveComponentsWithGroup(long componentGroup)
        {
            this.entityComponents.RemoveComponentsWithGroup(componentGroup);
        }

        public void RemoveComponent(long componentTypeId)
        {
            //this.UpdateHandlers(componentType);
            //this.NotifyComponentRemove(componentType);
            //this.RemoveComponentSilent(componentType);
            this.entityComponents.RemoveComponentImmediately(componentTypeId);
        }

        public void TryRemoveComponent(long componentTypeId)
        {
            //this.UpdateHandlers(componentType);
            //this.NotifyComponentRemove(componentType);
            //this.RemoveComponentSilent(componentType);
            if(this.HasComponent(componentTypeId))
            {
                try
                {
                    this.entityComponents.RemoveComponentImmediately(componentTypeId);
                }
                catch { };
            }
                
        }

        public void RemoveComponentIfPresent<T>() where T : ECSComponent
        {
            if (this.HasComponent<T>())
            {
                this.RemoveComponent(typeof(T));
            }
        }

        //public void RemoveComponentSilent(Type componentType)
        //{
        //    if (this.HasComponent(componentType) || (!this.HasComponent<DeletedEntityComponent>() && !this.IsSkipExceptionOnAddRemove(componentType)))
        //    {
        //        this.SendNodeRemoved(componentType);
        //        Component component = this.entityComponents.RemoveComponentImmediately(componentType);
        //        this.NotifyDetachFromEntity(component);
        //        this.UpdateNodesOnComponentRemoved(componentType);
        //    }
        //}

        //public virtual void RemoveEntityListener(EntityListener entityListener)
        //{
        //    this.entityListeners.Remove(entityListener);
        //}

        //internal void RemoveNode(NodeDescription node)
        //{
        //    this.SendNodeRemovedForCollectors(node);
        //    Flow.Current.NodeCollector.Detach(this, node);
        //    this.nodeDescriptionStorage.RemoveNode(node);
        //}

        public void ScheduleEvent<T>() where T : ECSEvent, new()
        {
            //EngineService.Engine.ScheduleEvent<T>(this);
        }

        public void ScheduleEvent(ECSEvent eventInstance)
        {
            //EngineService.Engine.ScheduleEvent(eventInstance, this);
        }

        //private void SendEntityDeletedForAllListeners()
        //{
        //    Collections.Enumerator<EntityListener> enumerator = Collections.GetEnumerator<EntityListener>(this.entityListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnEntityDeleted(this);
        //    }
        //    this.entityListeners.Clear();
        //}

        public T SendEvent<T>(T eventInstance) where T : ECSEvent
        {
            //EngineService.Engine.ScheduleEvent(eventInstance, this);
            return eventInstance;
        }

        //private void SendNodeAddedForCollectors(NodeDescription nodeDescription)
        //{
        //    Collections.Enumerator<EntityListener> enumerator = Collections.GetEnumerator<EntityListener>(this.entityListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnNodeAdded(this, nodeDescription);
        //    }
        //}

        //private void SendNodeRemoved(Type componentType)
        //{
        //    this.nodeRemoveEventMaker.MakeIfNeed(this, componentType);
        //}

        //private void SendNodeRemovedForCollectors(NodeDescription nodeDescription)
        //{
        //    Collections.Enumerator<EntityListener> enumerator = Collections.GetEnumerator<EntityListener>(this.entityListeners);
        //    while (enumerator.MoveNext())
        //    {
        //        enumerator.Current.OnNodeRemoved(this, nodeDescription);
        //    }
        //}

        //public T ToNode<T>() where T : Node, new()
        //{
        //    T local = Activator.CreateInstance<T>();
        //    local.Entity = this;
        //    return local;
        //}

        public override string ToString() =>
            $"{this.GetId()}({this.Name})";

        //public string ToStringWithComponentsClasses()
        //{
        //    if (<> f__am$cache1 == null)
        //    {
        //        <> f__am$cache1 = c => c.Name;
        //    }
        //    <> f__am$cache2 ??= c => c;
        //    string[] strArray = this.ComponentClasses.Select<Type, string>(<> f__am$cache1).OrderBy<string, string>(<> f__am$cache2).ToArray<string>();
        //    return $"Entity[id={this.Id}, name={this.Name}, components={string.Join(",", strArray)}]";
        //}

        //private void UpdateHandlers(Type componentType)
        //{
        //    if (componentType.IsSubclassOf(typeof(GroupComponent)))
        //    {
        //        if (<> f__am$cache0 == null)
        //        {
        //            <> f__am$cache0 = h => h.ChangeVersion();
        //        }
        //        this.engineService.HandlerCollector.GetHandlersByGroupComponent(componentType).ForEach<Handler>(<> f__am$cache0);
        //    }
        //}

        //private void UpdateNodes(ICollection<NodeDescription> nodes)
        //{
        //    BitSet componentsBitId = this.ComponentsBitId;
        //    Collections.Enumerator<NodeDescription> enumerator = Collections.GetEnumerator<NodeDescription>(nodes);
        //    while (enumerator.MoveNext())
        //    {
        //        NodeDescription current = enumerator.Current;
        //        if (componentsBitId.Mask(current.NodeComponentBitId))
        //        {
        //            if (componentsBitId.MaskNot(current.NotNodeComponentBitId))
        //            {
        //                this.AddNode(current);
        //                continue;
        //            }
        //            if (this.nodeDescriptionStorage.Contains(current))
        //            {
        //                this.RemoveNode(current);
        //            }
        //        }
        //    }
        //}

        //private void UpdateNodesOnComponentRemoved(Type componentClass)
        //{
        //    new List<NodeDescription>(this.nodeDescriptionStorage.GetNodeDescriptions(componentClass)).ForEach(new Action<NodeDescription>(this.RemoveNode));
        //    this.UpdateNodes(NodeDescriptionRegistry.GetNodeDescriptionsByNotComponent(componentClass));
        //}

        void AddEntityGroup(dynamic Entity, dynamic EntityGroup)
        {

        }
        void RemoveEntityGroup(dynamic Entity, dynamic EntityGroup)
        {

        }
        public object Clone() => MemberwiseClone();
    }
}
