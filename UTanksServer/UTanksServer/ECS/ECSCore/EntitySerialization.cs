using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components;
using UTanksServer.ECS.Components.ECSComponentsGroup;
using UTanksServer.Network.Simple.Net;
using UTanksServer.Services;

namespace UTanksServer.ECS.ECSCore
{
    public class EntitySerialization
    {
        public static string[] FullSerialize(ECSEntity entity, bool serializeOnlyChanged = false)
        {
            string result;
            string strEntity;
            using (StringWriter writer = new StringWriter())
            {
                GlobalCachingSerialization.cachingSerializer.Serialize(writer, entity);
                strEntity = writer.ToString();
            }
            string strComponents = entity.entityComponents.SerializeStorage(GlobalCachingSerialization.cachingSerializer, serializeOnlyChanged, true);
            //result = "{\"Entity\":\"" + strEntity + "\",\"Components\":\"" + strComponents + "\"}";
            return new string[] { strEntity, strComponents };
        }

        public static Dictionary<long, string> SlicedSerialize(ECSEntity entity, bool serializeOnlyChanged = false, bool clearChanged = false)
        {
            string result;
            string strEntity;
            
            lock(entity.entityComponents.serializationLocker)//wtf double locking is work
            {
                var strComponents = entity.entityComponents.SlicedSerializeStorage(GlobalCachingSerialization.cachingSerializer, serializeOnlyChanged, clearChanged);
                using (StringWriter writer = new StringWriter())
                {
                    GlobalCachingSerialization.cachingSerializer.Serialize(writer, entity);
                    strEntity = writer.ToString();
                }
                strComponents[ECSEntity.Id] = strEntity;
                //result = "{\"Entity\":\"" + strEntity + "\",\"Components\":\"" + strComponents + "\"}";
                return strComponents;
            }
            
        }

        public static void SerializeEntity(ECSEntity entity, bool serializeOnlyChanged = false)
        {
            var serializedData = SlicedSerialize(entity, serializeOnlyChanged, true);
            bool emptyData = true;
            foreach(var GDAP in entity.dataAccessPolicies)
            {
                GDAP.JsonAvailableComponents = "";
                GDAP.rawUpdateAvailableComponents.Clear();
                GDAP.JsonRestrictedComponents = "";
                GDAP.rawUpdateRestrictedComponents.Clear();
                GDAP.IncludeRemovedAvailable = false;
                GDAP.IncludeRemovedRestricted = false;
                foreach (var availableComp in GDAP.AvailableComponents)
                {
                    string serialData = "";
                    if (entity.entityComponents.RemovedComponents.Contains(availableComp))
                    {
                        GDAP.IncludeRemovedAvailable = true;
                        emptyData = false;
                    }
                    if (entity.entityComponents.directSerialized.Keys.Contains(availableComp))   
                    {
                        GDAP.rawUpdateAvailableComponents.Add(availableComp, entity.entityComponents.directSerialized[availableComp]);
                        emptyData = false;
                    }
                    if (!serializedData.TryGetValue(availableComp, out serialData))
                        continue;
                    GDAP.JsonAvailableComponents += "\"" + availableComp + "\":" + serialData + ",";
                    emptyData = false;
                }
                foreach (var availableComp in GDAP.RestrictedComponents)
                {
                    string serialData = "";
                    if (entity.entityComponents.RemovedComponents.Contains(availableComp))
                    {
                        GDAP.IncludeRemovedRestricted = true;
                        emptyData = false;
                    }
                    if (entity.entityComponents.directSerialized.Keys.Contains(availableComp))
                    {
                        GDAP.rawUpdateAvailableComponents.Add(availableComp, entity.entityComponents.directSerialized[availableComp]);
                        emptyData = false;
                    }
                    if (!serializedData.TryGetValue(availableComp, out serialData))
                        continue;
                    GDAP.JsonRestrictedComponents += "\"" + availableComp + "\":" + serialData + ",";
                    emptyData = false;
                }
            }
            entity.entityComponents.RemovedComponents.Clear();
            entity.serializedEntity = serializedData[ECSEntity.Id];
            entity.emptySerialized = emptyData;
        }

        public static (string, List<INetSerializable>) BuildSerializedEntityWithGDAP(ECSEntity toEntity, ECSEntity fromEntity, bool ignoreNullData = false)
        {
            var data = GroupDataAccessPolicy.ComponentsFilter(toEntity, fromEntity);
            if(data.Item1 == "" && !ignoreNullData)
            {
                return ("", data.Item2);
            }
            if(data.Item1 == "#INCLUDEREMOVED#" || ignoreNullData)
            {
                data.Item1 = "";
                return ("{\"entity\":" + fromEntity.serializedEntity + ",\"SerializationContainer\":{" + data.Item1 + "}}", data.Item2);
            }
            //return "{\"Entity\":\"" + fromEntity.serializedEntity.Replace("\"", "\\\"") + "\",\"Components\":\"{" + data.Substring(0, data.Length-1).Replace("\\\"", "\\\\\"").Replace("\"", "\\\"") + "}\"}";
            return ("{\"entity\":" + fromEntity.serializedEntity + ",\"SerializationContainer\":{" + data.Item1.Substring(0, data.Item1.Length - 1) + "}}", data.Item2);
           // return "";
        }

        public static string BuildFullSerializedEntityWithGDAP(ECSEntity toEntity, ECSEntity fromEntity)
        {
            var componentData = GroupDataAccessPolicy.RawComponentsFilter(toEntity, fromEntity);
            if (componentData.Count == 0)
            {
                return "";
            }
            var serializedData = SlicedSerialize(fromEntity);
            string data = "";
            foreach(var comp in componentData)
            {
                string serialData = "";
                if (!serializedData.TryGetValue(comp, out serialData))
                    continue;
                data += "\"" + comp + "\":" + serialData + ",";
            }
            //return "{\"Entity\":\"" + fromEntity.serializedEntity.Replace("\"", "\\\"") + "\",\"Components\":\"{" + data.Substring(0, data.Length-1).Replace("\\\"", "\\\\\"").Replace("\"", "\\\"") + "}\"}";
            return "{\"entity\":" + fromEntity.serializedEntity + ",\"SerializationContainer\":{" + data.Substring(0, data.Length - 1) + "}}";
            // return "";
        }

        public static string BuildFullSerializedEntity(ECSEntity Entity)
        {
            var serializedData = FullSerialize(Entity, false);
            //return "{\"Entity\":\"" + fromEntity.serializedEntity.Replace("\"", "\\\"") + "\",\"Components\":\"{" + data.Substring(0, data.Length-1).Replace("\\\"", "\\\\\"").Replace("\"", "\\\"") + "}\"}";
            if(serializedData[1] == "")
            {
                return "";
            }
            return "{\"entity\":" + serializedData[0] + ",\"SerializationContainer\":{" + serializedData[1] + "}}";
            // return "";
        }

        public static ECSEntity Deserialize(string serializedData)
        {
            ECSEntity entity;
            UnserializedEntity bufEntity;
            EntityComponentStorage storage;

            //using (StringWriter writer = new StringWriter())
            //{
            //    UnserializedEntity ent = new UnserializedEntity { entity = new ECSEntity(), SerializationContainer = new ConcurrentDictionary<long, object>() };
            //    GlobalCachingSerialization.cachingSerializer.Serialize(writer, ent);
            //}


            using (StringReader reader = new StringReader(serializedData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                bufEntity = (UnserializedEntity)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(UnserializedEntity));
                entity = bufEntity.entity;
                bufEntity.ReworkDictionary();
                storage = new EntityComponentStorage(entity);
                storage.SerializationContainer = bufEntity.SerializationContainer;
                storage.RestoreComponentsAfterSerialization(entity);
            }
            
            entity.entityComponents = storage;
            //ConstantService.AllTemplates[entity.TemplateAccessorId].
            return entity;
        }
        public static void UpdateDeserialize(string serializedData)
        {
            ECSEntity entity;
            UnserializedEntity bufEntity;
            EntityComponentStorage storage;

            using (StringReader reader = new StringReader(serializedData))
            {
                JsonTextReader jreader = new JsonTextReader(reader);
                bufEntity = (UnserializedEntity)GlobalCachingSerialization.standartSerializer.Deserialize(jreader, typeof(UnserializedEntity));
                entity = null;
                if (!ManagerScope.entityManager.EntityStorage.TryGetValue(bufEntity.entity.instanceId, out entity))
                {
                    entity = bufEntity.entity;
                    bufEntity.ReworkDictionary();
                    storage = new EntityComponentStorage(entity);
                    storage.SerializationContainer = bufEntity.SerializationContainer;
                    storage.RestoreComponentsAfterSerialization(entity);
                    entity.entityComponents = storage;
                    entity.fastEntityComponentsId = new ConcurrentDictionary<long, int>(entity.entityComponents.Components.ToDictionary(k => k.instanceId, t => 0));
                    ManagerScope.entityManager.OnAddNewEntity(entity);
                    return;
                }
                bufEntity.ReworkDictionary();
                if (GlobalProgramState.programType == GlobalProgramState.ProgramType.Client)
                {
                    entity.entityComponents.FilterRemovedComponents(bufEntity.entity.fastEntityComponentsId.Keys.ToList(), new List<long>() { ServerComponentGroup.Id });
                }
                else if (GlobalProgramState.programType == GlobalProgramState.ProgramType.Server)
                {
                    entity.entityComponents.FilterRemovedComponents(bufEntity.entity.fastEntityComponentsId.Keys.ToList(), new List<long>() { ClientComponentGroup.Id });
                }
                entity.entityComponents.RegisterAllComponents();
                foreach (var component in bufEntity.SerializationContainer)
                {
                    entity.AddOrChangeComponentSilent((ECSComponent)component.Value);
                }
                entity.entityComponents.RegisterAllComponents();
            }
        }
        public static string ComponentsFilter(List<ECSComponent> acceptedComponents, string entitySerializedComponents)
        {
            Dictionary<string, string> parsedIdComponents = new Dictionary<string, string>();
            var bufComponents = new string(entitySerializedComponents);
            #region oldfuck
            //var startIdComponent = bufComponents.Substring(bufComponents.IndexOf("\""), bufComponents.IndexOf(":")-1);
            //while (bufComponents.IndexOf("ReflectionId") > -1)
            //{
            //    var componentData = bufComponents.Substring(bufComponents.IndexOf(":"), bufComponents.Substring(bufComponents.IndexOf("ReflectionId")).IndexOf("}"));
            //    parsedIdComponents[startIdComponent] = componentData;
            //    var Distinct = bufComponents.Substring(bufComponents.Substring(bufComponents.IndexOf("ReflectionId")).IndexOf("}"));
            //    if(Distinct.IndexOf("ReflectionId") > -1)
            //    {
            //        bufComponents = Distinct.Substring(Distinct.IndexOf(","));
            //        startIdComponent = bufComponents.Substring(bufComponents.IndexOf("\"") - 1, bufComponents.IndexOf(":"));
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            #endregion

            //while(bufComponents.Length > 0)
            //{

            //}
            Console.WriteLine("Jesssica");
            return null;

        }
    }
    public class UnserializedEntity : CachingSerializable
    {
        public ECSEntity entity { get; set; }
        public ConcurrentDictionary<long, object> SerializationContainer { get; set; }

        public void ReworkDictionary()
        {
            foreach(var keypair in SerializationContainer)
            {
                SerializationContainer[keypair.Key] = (object)(keypair.Value as JObject).ToObject(ECSComponentManager.AllComponents[keypair.Key].GetTypeFast());
            }
        }
    }
}
