using SecuredSpace.ClientControl.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient.Core.Logging;

namespace UTanksClient.ECS.ECSCore
{
    public class ComponentManagers : IDictionary<Type, IComponentManager>
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ECSComponent ownerComponent = null;

        IDictionary<Type, IComponentManager> managersStorage = new ConcurrentDictionary<Type, IComponentManager>();

        protected virtual void AddImpl(Type key, IComponentManager value)
        {
            lock (managersStorage)
            {
                if (!managersStorage.ContainsKey(key))
                {
                    managersStorage.Add(key, value);
                    value.ConnectPoint = this.ownerComponent;
                }
                else
                {
                    ULogger.Error("You trie add already added element! If you need it - use [] indexer" + key.ToString());
                    return;
                }
            }
            value.AddManager();
        }

        protected virtual IComponentManager GetImpl(Type key)
        {
            lock (managersStorage)
                return managersStorage[key];
        }

        protected virtual void SetImpl(Type key, IComponentManager value)
        {
            if (managersStorage.ContainsKey(key))
            {
                var manager = managersStorage[key];
                manager.ExecuteInstruction(() => manager.RemoveManager());
            }
            lock (managersStorage)
                managersStorage[key] = value;
            value.ConnectPoint = this.ownerComponent;
        }

        protected virtual bool RemoveImpl(Type key)
        {
            if (managersStorage.ContainsKey(key))
            {
                var manager = managersStorage[key];
                manager.ExecuteInstruction(() => manager.RemoveManager());
            }
            lock (managersStorage)
            {
                if (managersStorage.ContainsKey(key))
                {
                    return managersStorage.Remove(key);
                }
                else
                {
                    ULogger.Error("You trie remove non presented component " + key.ToString());
                    return false;
                }
            }
        }

        protected virtual bool RemoveImpl(KeyValuePair<Type, IComponentManager> item)
        {
            if (managersStorage.ContainsKey(item.Key))
            {
                var manager = managersStorage[item.Key];
                manager.ExecuteInstruction(() => manager.RemoveManager());
            }
            lock (managersStorage)
            {
                if (managersStorage.ContainsKey(item.Key))
                {
                    return managersStorage.Remove(item);
                }
                else
                {
                    ULogger.Error("You trie remove non presented component " + item.Key.ToString());
                    return false;
                }
            }
        }

        protected virtual void ClearImpl()
        {
            lock (managersStorage)
                managersStorage.Values.ForEach(x => x.ExecuteInstruction(() => x.RemoveManager()));
        }

        #region DictionaryRealization

        public ICollection<Type> Keys => managersStorage.Keys;

        public ICollection<IComponentManager> Values => managersStorage.Values;

        public int Count => managersStorage.Count;

        public bool IsReadOnly => managersStorage.IsReadOnly;

        public IComponentManager this[Type key] { get => GetImpl(key); set => SetImpl(key, value); }

        public void Add(Type key, IComponentManager value)
        {
            AddImpl(key, value);
        }

        public bool ContainsKey(Type key)
        {
            return managersStorage.ContainsKey(key);
        }

        public bool Remove(Type key)
        {
            return RemoveImpl(key);
        }

        public bool Remove<T>() where T : IComponentManager
        {
            return RemoveImpl(typeof(T));
        }

        public T Get<T>() where T : IComponentManager
        {
            this.TryGetValue<T>(out var result);
            return result;
        }

        public bool TryGetValue<T>(out T value) where T : IComponentManager
        {
            IComponentManager value2 = null;
            var ret = TryGetValue(typeof(T), out value2);
            value = (T)value2;
            return ret;
        }

        public bool TryGetValue(Type key, out IComponentManager value)
        {
            IComponentManager _manager = null;
            try
            {
                _manager = GetImpl(key);
            }
            catch
            {
                Debug.LogWarning("Error get from GroupComponentManager " + this.GetType().ToString());
                value = null;
                return false;
            }
            value = _manager;
            return true;
        }

        public void Add(KeyValuePair<Type, IComponentManager> item)
        {
            AddImpl(item.Key, item.Value);
        }

        public void Clear()
        {
            ClearImpl();
        }

        public bool Contains(KeyValuePair<Type, IComponentManager> item)
        {
            return managersStorage.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<Type, IComponentManager>[] array, int arrayIndex)
        {
            managersStorage.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<Type, IComponentManager> item)
        {
            return RemoveImpl(item.Key);
        }

        public IEnumerator<KeyValuePair<Type, IComponentManager>> GetEnumerator()
        {
            return managersStorage.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return managersStorage.GetEnumerator();
        }

        #endregion
    }
}
