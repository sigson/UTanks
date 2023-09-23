using Core;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.ECSCore
{
    public class ECSSystemManager
    {
        public ConcurrentDictionary<ECSSystem, ConcurrentDictionaryEx<long, int>> SystemsInterestedEntityDatabase = new();//List of interested entity Instance ID
        public int SystemsInterestedEntityDatabaseCount;
        //забить все ид компонентов которые интересны системам заранее и по нему ориентироваться
        public ConcurrentDictionary<ECSSystem, ConcurrentDictionaryEx<long, int>> InterestedIDECSComponentsDatabase = new();
        public int InterestedIDECSComponentsDatabaseCount;

        public static bool LockSystems = false;

        public void InitializeSystems()
        {
            var AllSystems = ECSAssemblyExtensions.GetAllSubclassOf(typeof(ECSSystem)).Select(x => (ECSSystem)Activator.CreateInstance(x)).ToList();
            AllSystems = AllSystems.Except(ReturnExceptedSystems()).ToList<ECSSystem>();
            FullUpdateSystemListOfInterestECSComponents(AllSystems);
            foreach(ECSSystem system in AllSystems)
            {
                system.Initialize(this);
                if(system.Enabled)
                    SystemsInterestedEntityDatabase.TryAdd(system, new ConcurrentDictionaryEx<long, int>());
                
                foreach(var CallbackData in system.ComponentsOnChangeCallbacks)
                {
                    List<Action<ECSEntity, ECSComponent>> callBack;
                    if (ECSComponentManager.OnChangeCallbacksDB.TryGetValue(CallbackData.Key, out callBack))
                    {
                        ECSComponentManager.OnChangeCallbacksDB[CallbackData.Key] = callBack.Concat(CallbackData.Value).ToList();
                    }
                    else
                    {
                        ECSComponentManager.OnChangeCallbacksDB[CallbackData.Key] = CallbackData.Value;
                    }
                }
                
            }
        }

        public void RunSystems()
        {
            if (LockSystems)
                return;
            foreach(var SystemPair in SystemsInterestedEntityDatabase)
            {
                if (Interlocked.Equals(SystemPair.Key.Enabled, true) && Interlocked.Equals(SystemPair.Key.InWork, false) && SystemPair.Key.LastEndExecutionTimestamp + DateTimeExtensions.MillisecondToTicks
                    (SystemPair.Key.DelayRunMilliseconds) < DateTime.Now.Ticks)
                {
                    Func<Task> asyncUpd = async () =>
                    {
                        SystemPair.Key.InWork = true;
                        await Task.Run(() => {
                            SystemPair.Key.Run(SystemsInterestedEntityDatabase[SystemPair.Key].Keys.ToArray());
                        });
                    };
                    asyncUpd();
                }
                    
            }
        }

        public void OnEntityComponentAddedReaction(ECSEntity entity, ECSComponent component)
        {
            foreach (KeyValuePair<ECSSystem, ConcurrentDictionaryEx<long, int>> pair in this.InterestedIDECSComponentsDatabase)
            {
                int nulled = 0;
                if (pair.Value.TryGetValue(component.GetId(), out nulled))
                {
                    ConcurrentDictionaryEx<long, int> bufDict;
                    if (SystemsInterestedEntityDatabase.TryGetValue(pair.Key, out bufDict))
                        if (SystemsInterestedEntityDatabase[pair.Key].TryAdd(entity.instanceId, nulled))
                            Interlocked.Increment(ref SystemsInterestedEntityDatabase[pair.Key].FastCount);
                }
            }
        }

        public void OnEntityComponentRemovedReaction(ECSEntity entity, ECSComponent component)
        {
            foreach (KeyValuePair<ECSSystem, ConcurrentDictionaryEx<long, int>> pair in this.InterestedIDECSComponentsDatabase)
            {
                if (pair.Value.TryGetValue(component.GetId(), out _))
                {
                    ConcurrentDictionaryEx<long, int> bufDict;
                    if (SystemsInterestedEntityDatabase.TryGetValue(pair.Key, out bufDict))
                    {
                        if (bufDict.Keys.Contains(entity.instanceId))
                        {
                            if (SystemsInterestedEntityDatabase[pair.Key].Remove(entity.instanceId, out _))
                                Interlocked.Decrement(ref SystemsInterestedEntityDatabase[pair.Key].FastCount);
                        }
                    }
                }
            }
        }

        public void OnEntityDestroyed(ECSEntity entity)
        {
            bool cleared = false;
            foreach (KeyValuePair<ECSSystem, ConcurrentDictionaryEx<long, int>> pair in this.SystemsInterestedEntityDatabase)
            {
                int nulled = 0;
                ConcurrentDictionaryEx<long, int> bufDict;
                if(SystemsInterestedEntityDatabase.TryGetValue(pair.Key, out bufDict))
                    if(pair.Value.TryRemove(entity.instanceId, out nulled))
                    {
                        Interlocked.Decrement(ref SystemsInterestedEntityDatabase[pair.Key].FastCount);
                        cleared = true;
                    }
                    
            }
            if(!cleared)
            {
                Logger.LogError("core system error");
            }
        }

        public void OnEntityCreated(ECSEntity entity)
        {
            foreach (KeyValuePair<ECSSystem, ConcurrentDictionaryEx<long, int>> pair in this.InterestedIDECSComponentsDatabase)
            {
                int nulled = 0;
                ConcurrentDictionaryEx<long, int> bufDict;
                if(SystemsInterestedEntityDatabase.TryGetValue(pair.Key, out bufDict))
                {
                    if (Collections.FirstIntersect(pair.Value, entity.entityComponents.IdToTypeComponent.Keys))
                    {
                        if (SystemsInterestedEntityDatabase[pair.Key].TryAdd(entity.instanceId, nulled))
                            Interlocked.Increment(ref SystemsInterestedEntityDatabase[pair.Key].FastCount);
                    }
                }
            }
        }

        public List<ECSSystem> ReturnExceptedSystems()
        {
            List<ECSSystem> list = new List<ECSSystem> {

            };
            return list;
        }

        public void AppendSystemInRuntime(ECSSystem system)
        {
            FullUpdateSystemListOfInterestECSComponents(new List<ECSSystem> { system });
            SystemsInterestedEntityDatabase.TryAdd(system, new ConcurrentDictionaryEx<long, int>());
        }

        private void FullUpdateSystemListOfInterestECSComponents(List<ECSSystem> allSystems)
        {
            foreach(ECSSystem system in allSystems)
            {
                if(InterestedIDECSComponentsDatabase.TryAdd(system, system.ReturnInterestedComponentsList()))
                {
                    Interlocked.Increment(ref InterestedIDECSComponentsDatabase[system].FastCount);
                }
            }
            //foreach (Type type in AssemblyExtensions.GetAllSubclassOf(typeof(ECSComponent)))
            //{
            //    var instance = (ECSComponent)Activator.CreateInstance(type);
            //    foreach(ECSSystem system in AllSystems)
            //    {
            //        system.HasInterest()
            //    }
            //}
            //var instance = Convert.ChangeType(Activator.CreateInstance(AllComponents[0]), AllComponents[0]);
            
        }

        public void UpdateSystemListOfInterestECSComponents(ECSSystem system, List<long> updatedIds)
        {

        }
    }
}
