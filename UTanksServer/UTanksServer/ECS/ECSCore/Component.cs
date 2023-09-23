using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.Core.Logging;
using UTanksServer.Network.Simple.Net;

namespace UTanksServer.ECS.ECSCore
{
    public class ECSComponent : CachingSerializable, ICloneable
    {
        static public long Id { get; set; } = 0;

        public long instanceId = Guid.NewGuid().GuidToLong();
        [NonSerialized]
        public ECSEntity ownerEntity;

        [NonSerialized]
        public Type ComponentType;
        [NonSerialized]
        public bool DirectiveUpdate;
        [NonSerialized]
        public INetSerializable DirectiveUpdateContainer;
        [NonSerialized]
        public ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        //public List<Action>
        [NonSerialized]
        public List<string> ConfigPath = new List<string>();
        //[ServerOnlyData]
        //[NonSerialized]
        public ConcurrentDictionary<long, ECSComponentGroup> ComponentGroups = new ConcurrentDictionary<long, ECSComponentGroup>();//ECSComponentGroup
        [NonSerialized]
        static public List<Action> StaticOnChangeHandlers = new List<Action>();
        [NonSerialized]
        public List<Action<ECSEntity, ECSComponent>> OnChangeHandlers = new List<Action<ECSEntity, ECSComponent>>();
        [NonSerialized]
        protected long ReflectionId = 0;
        [NonSerialized]
        public bool Unregistered = true;

        public void DirectiveSerialize()
        {
            if(DirectiveUpdate)
            {
                ImplDirectiveSerialization();
            }
        }

        protected virtual void ImplDirectiveSerialization()
        {

        }

        public long GetId()
        {
            if (Id == 0)
                try
                {
                    if(ComponentType == null)
                    {
                        ComponentType = GetType();
                    }
                    if (ReflectionId == 0)
                        ReflectionId = ComponentType.GetCustomAttribute<TypeUidAttribute>().Id;
                    return ReflectionId;
                }
                catch
                {
                    Logger.Error(this.GetType().ToString() + "Could not find Id field");
                    return 0;
                }
            else
                return Id;
        }

        public List<Action<ECSEntity, ECSComponent>> GetOnChangeComponentCallback()
        {
            if (ComponentType == null)
            {
                ComponentType = GetType();
            }
            try
            {
                if(OnChangeHandlers == null)
                {
                    //OnChangeHandlers = (List<Action>)ComponentType.GetField("StaticOnChangeHandlers", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                    OnChangeHandlers = ECSComponentManager.OnChangeCallbacksDB[this.GetId()];
                }
                
                return OnChangeHandlers;
            }
            catch
            {
                Logger.Log(ComponentType);
                Logger.Log("Type not has callbacks");
                return null;
            }
            
        }

        public void DirectiveSetChanged()
        {
            if (ownerEntity != null)
            {
                ownerEntity.entityComponents.DirectiveChange(this.GetType());
            }
        }

        public void MarkAsChanged(bool silent = false)
        {
            if (ownerEntity != null)
            {
                ownerEntity.entityComponents.MarkComponentChanged(this, silent);
            }
        }

        public ECSComponent SetGlobalComponentGroup()
        {
            this.ComponentGroups[ECSComponentManager.GlobalProgramComponentGroup.GetId()] = ECSComponentManager.GlobalProgramComponentGroup;
            return this; 
        }

        public ECSComponent AddComponentGroup(ECSComponentGroup componentGroup)
        {
            this.ComponentGroups[componentGroup.GetId()] = componentGroup;
            return this;
        }

        public Type GetTypeFast()
        {
            if (ComponentType == null)
            {
                ComponentType = GetType();
            }
            return ComponentType;
        }

        /// overridable functional for damage transformer, after adding component of damage effect - in this method we send transformer action to damage transformers agregator
        public virtual void OnAdded(ECSEntity entity)
        {
            this.MarkAsChanged();
        }

        public virtual void OnRemoving(ECSEntity entity)
        {

        }

        public void OnRemove()
        {
            ConfigPath.Clear();
            ComponentGroups.Clear();
            OnChangeHandlers.Clear();
        }
        public void RunOnChangeCallbacks(ECSEntity parentEntity)
        {
            List<Action<ECSEntity, ECSComponent>> callbackActions;
            ECSComponentManager.OnChangeCallbacksDB.TryGetValue(this.GetId(), out callbackActions);
            if(callbackActions!=null)
            {
                foreach (var act in callbackActions)
                {
                    act(parentEntity, this);
                }
            }            
        }
        public object Clone() => MemberwiseClone();
    }
}
