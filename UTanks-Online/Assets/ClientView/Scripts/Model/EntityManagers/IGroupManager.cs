using SecuredSpace.ClientControl.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IGroupManager<T> : IManager, IDictionary<long, T> where T : IECSObject
    {
        protected IDictionary<long, T> entityStorage = new ConcurrentDictionary<long, T>();

        protected abstract void OnAdd(T entity);
        protected abstract void OnGet(T entity);
        protected abstract void OnSet(T entity);
        protected abstract void OnRemove(T entity);

        private void AddImpl(long key, T value)
        {
            
            lock (entityStorage)
            {
                if(!entityStorage.ContainsKey(key))
                {
                    entityStorage.Add(key, value);
                }
                else
                {
                    Debug.LogError("You trie add already added element! If you need it - use [] indexer");
                }
                EntityGroupManagersStorageService.instance.AddEntityCache(this, value.instanceId);
            }
            OnAdd(value);
        }

        private T GetImpl(long key)
        {
            T ret = null;
            lock (entityStorage)
                ret = entityStorage[key];
            OnGet(ret);
            return ret;
        }

        private void SetImpl(long key, T value)
        {
            lock (entityStorage)
                entityStorage[key] = value;
            OnSet(value);
        }

        private bool RemoveImpl(long key)
        {
            var onEntity = entityStorage[key];
            bool _return = false;
            lock (entityStorage)
            {
                _return = entityStorage.Remove(key);
                EntityGroupManagersStorageService.instance.RemoveEntityCache(this, key);
            }
            OnRemove(onEntity);
            return _return;
        }

        private bool RemoveImpl(KeyValuePair<long, T> item)
        {
            var onEntity = entityStorage[item.Value.instanceId];
            bool _return = false;
            lock (entityStorage)
            {
                _return = entityStorage.Remove(item.Value.instanceId);
                EntityGroupManagersStorageService.instance.RemoveEntityCache(this, item.Value.instanceId);
            }
            OnRemove(onEntity);
            return _return;
        }

        private void ClearImpl()
        {
            lock (entityStorage)
                this.ForEach(x => this.Remove(x.Key));
        }

        #region DictionaryRealization
        public T this[long key] { get => GetImpl(key); set => SetImpl(key, value); }

        public ICollection<long> Keys => entityStorage.Keys;

        public ICollection<T> Values => entityStorage.Values;

        public int Count => entityStorage.Count;

        public bool IsReadOnly => entityStorage.IsReadOnly;

        public void Add(long key, T value)
        {
            AddImpl(key, value);
        }

        public void Add(KeyValuePair<long, T> item)
        {
            AddImpl(item.Key, item.Value);
        }

        public void Clear()
        {
            ClearImpl();
        }

        public bool Contains(KeyValuePair<long, T> item)
        {
            return entityStorage.Contains(item);
        }

        public bool ContainsKey(long key)
        {
            return entityStorage.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<long, T>[] array, int arrayIndex)
        {
            entityStorage.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
        {
            return entityStorage.GetEnumerator();
        }

        public bool Remove(long key)
        {
            return RemoveImpl(key);
        }

        public bool Remove(KeyValuePair<long, T> item)
        {
            return RemoveImpl(item);
        }

        public bool TryGetValue(long key, out T value)
        {
            T _entity = null;
            try
            {
                _entity = GetImpl(key);
            }
            catch
            {
                Debug.LogWarning("Error get from GroupEntityManager " + this.GetType().ToString());
                value = null;
                return false;
            }
            value = _entity;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entityStorage.GetEnumerator();
        }
        #endregion
    }
}