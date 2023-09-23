using Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.Extensions;
using static UTanksServer.ECS.ECSCore.ECSSystem;

namespace UTanksServer.ECS.ECSCore
{
    public class ECSEventManager
    {
        public List<long> AcceptedOutsideEvents = new List<long>()
        {

        };//allowed events going from internet

        public ConcurrentDictionaryEx<long, ConcurrentDictionaryEx<ECSSystem, List<Func<ECSEvent, object>>>> SystemHandlers = new ConcurrentDictionaryEx<long, ConcurrentDictionaryEx<ECSSystem, List<Func<ECSEvent, object>>>>();
        public ConcurrentDictionaryEx<long, ECSEvent> EventBus = new ConcurrentDictionaryEx<long, ECSEvent>();
        public Dictionary<long, Type> EventSerializationCache = new Dictionary<long, Type>();

        public ObjectPool<EventWatcher> watcherPool;

        public ECSEventManager()
        {
            watcherPool = new ObjectPool<EventWatcher>(() => new EventWatcher(this, 0, 0));
        }

        public void IdStaticCache()
        {
            var AllEvents = ECSAssemblyExtensions.GetAllSubclassOf(typeof(ECSEvent)).Select(x => (ECSEvent)Activator.CreateInstance(x));
            foreach (var Event in AllEvents)
            {
                EventSerializationCache.Add(Event.GetId(), Event.GetType());
                try
                {
                    var field = Event.GetType().GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    var customAttrib = Event.GetType().GetCustomAttribute<TypeUidAttribute>();
                    if (customAttrib != null)
                        field.SetValue(null, customAttrib.Id);
                    //Console.WriteLine(comp.GetId().ToString() + "  " + comp.GetType().Name);
                }
                catch
                {
                    Console.WriteLine(Event.GetType().Name);
                }
            }
        }

        public void InitializeEventManager()
        {
            var AllEvents = ECSAssemblyExtensions.GetAllSubclassOf(typeof(ECSEvent)).Select(x => (ECSEvent)Activator.CreateInstance(x));
            
            foreach (ECSSystem system in ManagerScope.systemManager.InterestedIDECSComponentsDatabase.Keys.ToList())
            {
                var SystemInterest = system.ReturnInterestedEventsList();
                foreach (var Event in AllEvents)
                {
                    if(SystemInterest.Keys.Contains(Event.GetId()))
                    {
                        ConcurrentDictionaryEx<ECSSystem, List<Func<ECSEvent, object>>> NewDictionary;
                        if(SystemHandlers.TryGetValue(Event.GetId(), out NewDictionary))
                        {
                            List<Func<ECSEvent, object>> outfunc;
                            if(system.SystemEventHandler.TryGetValue(Event.GetId(), out outfunc))
                                NewDictionary.TryAdd(system, outfunc);
                        }
                        else
                        {
                            NewDictionary = new ConcurrentDictionaryEx<ECSSystem, List<Func<ECSEvent, object>>>();
                            List<Func<ECSEvent, object>> outfunc;
                            if (system.SystemEventHandler.TryGetValue(Event.GetId(), out outfunc))
                                NewDictionary.TryAdd(system, outfunc);
                            SystemHandlers.TryAdd(Event.GetId(), NewDictionary);
                        }
                    }
                }
            }
        }

        public void OnEventAdd(ECSEvent ecsEvent)
        {
            ecsEvent.Execute();
            ecsEvent.eventWatcher = watcherPool.Get().EventWatcherUpdate(SystemHandlers[ecsEvent.GetId()].Count, ecsEvent.instanceId);
            if (!EventBus.TryAdd(ecsEvent.instanceId, ecsEvent))
                Logger.LogError("error add event to bus");
            foreach(var system in SystemHandlers[ecsEvent.GetId()])
            {
                foreach(var func in system.Value)
                {
                    Func<Task> asyncUpd = async () =>
                    {
                        await Task.Run(() =>
                        {
                            func.DynamicInvoke(ecsEvent);
                        });
                    };
                    asyncUpd();
                }
            }
        }

        public void OnEventProcessed(long ecsEventId)
        {
            ECSEvent removed;
            if(!EventBus.TryRemove(ecsEventId, out removed))
            {
                Logger.LogError("core event error");
            }
            watcherPool.Return(removed.eventWatcher);
        }
        public void UpdateSystemHandlers(long eventId, List<Func<ECSEvent, object>> handler)
        {

        }

        public void RemoveSystemHandlers(long eventId, List<Func<ECSEvent, object>> handler)
        {

        }
    }

    public class EventWatcher
    {
        public ECSEventManager eventManager;
        private volatile int watchers;
        public long EventId;
        public int Watchers
        {
            get
            {
                return watchers;
            }
            set
            {
                Interlocked.Exchange(ref watchers, value);
                //watchers = value;
                if (watchers == 0)
                {
                    eventManager.OnEventProcessed(EventId);
                }
            }
        }
        public EventWatcher(ECSEventManager eCSEventManager, int allWatchers, long eventId)
        {
            eventManager = eCSEventManager;
            watchers = allWatchers;
            EventId = eventId;
        }

        public EventWatcher EventWatcherUpdate(ECSEventManager eCSEventManager, int allWatchers, long eventId)
        {
            eventManager = eCSEventManager;
            watchers = allWatchers;
            EventId = eventId;
            return this;
        }

        public EventWatcher EventWatcherUpdate(int allWatchers, long eventId)
        {
            watchers = allWatchers;
            EventId = eventId;
            return this;
        }
    }
}
