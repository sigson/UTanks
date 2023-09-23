using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.ECSCore
{
    public abstract class ECSSystem
    {
        public long Id { get; set; }
        [NonSerialized]
        public Type SystemType;

        public bool Enabled { get; set; }
        public bool InWork { get; set; }

        public long LastEndExecutionTimestamp { get; set; }
        public long DelayRunMilliseconds { get; set; }

        public bool EventIgnoring { get; set; }

        public Dictionary<long, List<Func<ECSEvent, object>>> SystemEventHandler = new Dictionary<long, List<Func<ECSEvent, object>>>();//id of event and func
        public Dictionary<long, List<Action<ECSEntity, ECSComponent>>> ComponentsOnChangeCallbacks = new Dictionary<long, List<Action<ECSEntity, ECSComponent>>>();//id of component and func
        public ECSSystemManager systemManager { get; set; }

        public abstract void Initialize(ECSSystemManager SystemManager);

        public abstract void Run(long[] entities);

        public abstract void Operation(ECSEntity entity, ECSComponent Component);

        public abstract bool HasInterest(ECSEntity entity);

        public abstract bool UpdateInterestedList(List<long> ComponentsId);

        public abstract ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList();

        public abstract ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList();

        private void RegisterEventHandler(long eventId)
        {
            ManagerScope.eventManager.UpdateSystemHandlers(eventId, this.SystemEventHandler[eventId]);
        }

        public virtual void UpdateEventWatcher(ECSEvent eCSEvent)
        {
            eCSEvent.eventWatcher.Watchers--;
        }

        public Type GetTypeFast()
        {
            if (SystemType == null)
            {
                SystemType = GetType();
            }
            return SystemType;
        }
    }
}
