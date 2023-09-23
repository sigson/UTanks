using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Events.Battle;
using UTanksClient.ECS.Events.Battle.BonusEvents;
using UTanksClient.ECS.Events.Battle.GoalEvents;
using UTanksClient.ECS.Events.Battle.GoalEvents.CP;
using UTanksClient.ECS.Events.Battle.TankEvents;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.ECS.Events.Chat;
using UTanksClient.ECS.Events.ECSEvents;
using UTanksClient.ECS.Events.Garage;
using UTanksClient.ECS.Events.Shop;
using UTanksClient.ECS.Events.User;
using UTanksClient.Extensions;
using static UTanksClient.ECS.ECSCore.ECSSystem;

namespace UTanksClient.ECS.ECSCore
{
    public class ECSEventManager
    {
        public List<long> AcceptedOutsideEvents = new List<long>()
        {
            BonusTakenEvent.Id,
            BonusTakingEvent.Id,
            EnterToPointZoneEvent.Id,
            ExitFromPointZoneEvent.Id,
            FlagDeliveredEvent.Id,
            FlagReturnEvent.Id,
            FlagTakenEvent.Id,
            EndShootingEvent.Id,
            HitEvent.Id,
            ShotEvent.Id,
            ShotHPResultEvent.Id,
            StartAimingEvent.Id,
            StartChargingEvent.Id,
            StartShootingEvent.Id,
            KillEvent.Id,
            AnalogMoveCommandServerEvent.Id,
            DestructionEvent.Id,
            GamePauseEvent.Id,
            MoveCommandEvent.Id,
            SelfDestructionRequestEvent.Id,
            SupplyUsedEvent.Id,
            TankRessurectionEvent.Id,
            BattleEndEvent.Id,
            BattleStartEvent.Id,
            CreateBattleEvent.Id,
            EnterToBattleEvent.Id,
            LeaveFromBattleEvent.Id,
            RemoveBattleEvent.Id,
            ChatSendMessageEvent.Id,
            ChatMessageCallbackEvent.Id,
            RemoveEntitiesEvent.Id,
            ReceiveEntitiesEvent.Id,
            TransferEntitiesEvent.Id,
            UpdateEntitiesEvent.Id,
            GarageBuyItemEvent.Id,
            WeaponChangeEvent.Id,
            ShopBuyItemEvent.Id,
            UserLogged.Id
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
            if(!SystemHandlers.ContainsKey(ecsEvent.GetId()))
            {
                //if event no have system handler
                return;
            }
            ecsEvent.eventWatcher = watcherPool.Get().EventWatcherUpdate(SystemHandlers[ecsEvent.GetId()].Count, ecsEvent.instanceId);
            if (!EventBus.TryAdd(ecsEvent.instanceId, ecsEvent))
                ULogger.Error("error add event to bus");
            foreach(var system in SystemHandlers[ecsEvent.GetId()])
            {
                foreach(var func in system.Value)
                {
                    TaskEx.RunAsync(() =>
                    {
                        func.DynamicInvoke(ecsEvent);
                    });
                }
            }
        }

        public void OnEventProcessed(long ecsEventId)
        {
            ECSEvent removed;
            if(!EventBus.TryRemove(ecsEventId, out removed))
            {
                ULogger.Error("core event error");
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
