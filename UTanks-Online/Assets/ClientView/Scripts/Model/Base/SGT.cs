using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.Core.Logging;

namespace SecuredSpace.ClientControl.Model
{
    /// <summary>
    /// Singleton
    /// </summary>
    public abstract class SGT : ProxyBehaviour
    {
        private static Dictionary<Type, SGT> instances = new Dictionary<Type, SGT>();

        public static T InitalizeSingleton<T>(GameObject behaviour = null) where T : SGT
        {
            return (T)InitalizeSingleton(typeof(T), behaviour);
        }

        public static SGT InitalizeSingleton(Type singletonType, GameObject behaviour = null)
        {
            SGT instance = null;
            lock (instances)
            {
                try
                {
                    instances.TryGetValue(singletonType, out instance);
                    if (instance == null)
                    {
                        instance = (SGT)behaviour.GetComponent(singletonType);
                        if (instance == null)
                        {
                            if (behaviour != null)
                                instance = (SGT)behaviour.gameObject.AddComponent(singletonType);
                            else
                            {
                                instance = (SGT)new GameObject().AddComponent(singletonType);
                                DontDestroyOnLoad(instance.gameObject);
                            }
                        }
                        instances.Add(singletonType, instance);
                    }
                }
                catch(Exception ex)
                {
                    ULogger.Error("Escape from lock: " + ex.Message + " _________ " + ex.StackTrace);
                }
            }
            instance.InitializeProcess();
            return instance;
        }

        public static T Get<T>(GameObject behaviour = null) where T : SGT
        {
            return (T)getInstance<T>(behaviour);
        }

        public static T tryGetInstance<T>(GameObject behaviour = null) where T : SGT
        {
            try
            {
                return getInstance<T>(behaviour);
            }
            catch(Exception ex)
            {
                ULogger.Log(ex.Message + "\n" + ex.StackTrace);
            }
            return null;
        }

        public static T getInstance<T>(GameObject behaviour = null) where T : SGT
        {
            SGT instance = null;
            instances.TryGetValue(typeof(T), out instance);
            if (instance == null)
            {
                throw new Exception($"Singleton { typeof(T) } not initialized");
                //return null;
            }
            return (T)instance;
        }

        public abstract void InitializeProcess();
        public abstract void OnDestroyReaction();

        protected virtual void OnDestroy()
        {
            lock(instances)
            {
                try
                {
                    SGT instance = null;
                    instances.TryGetValue(this.GetType(), out instance);
                    if (instance != null)
                    {
                        instances.Remove(this.GetType());
                    }
                }
                catch (Exception ex)
                {
                    ULogger.Error("Escape from lock: " + ex.Message + " _________ " + ex.StackTrace);
                }
            }
            OnDestroyReaction();
        }

        public static void DestroySGT<T>() where T : SGT
        {
            DestroySGT(typeof(T));
        }

        public static void DestroySGT(Type type)
        {
            lock (instances)
            {
                try
                {
                    if (instances.TryGetValue(type, out var sgt))
                    {
                        try
                        {
                            Destroy(sgt);
                        }
                        catch { }
                        instances.Remove(type);
                    }
                }
                catch (Exception ex)
                {
                    ULogger.Error("Escape from lock: " + ex.Message + " _________ " + ex.StackTrace);
                }
            }
        }
    }
}