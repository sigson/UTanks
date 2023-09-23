﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.ECSComponentsGroup;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.ECSCore
{
    public class ECSComponentManager
    {
        public static Dictionary<long, ECSComponent> AllComponents = new Dictionary<long, ECSComponent>();

        public static Dictionary<long, List<Action<ECSEntity, ECSComponent>>> OnChangeCallbacksDB = new Dictionary<long, List<Action<ECSEntity, ECSComponent>>>();

        public static ECSComponentGroup GlobalProgramComponentGroup;
        static public void IdStaticCache()
        {
            var AllDirtyComponents = ECSAssemblyExtensions.GetAllSubclassOf(typeof(ECSComponent)).Where(x=>!x.IsAbstract).Select(x => (ECSComponent)Activator.CreateInstance(x)).ToList(); 
            foreach(var comp in AllDirtyComponents)
            {
                AllComponents[comp.GetId()] = comp;
            }
            ECSEntity entity = new ECSEntity();
            foreach (var comp in AllComponents.Values)
            {
                try
                {
                    var field = comp.GetType().GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    var customAttrib = comp.GetType().GetCustomAttribute<TypeUidAttribute>();
                    if (customAttrib != null)
                        field.SetValue(null, customAttrib.Id);
                    entity.AddComponentSilent((ECSComponent)comp.Clone());
                    //Console.WriteLine(comp.GetId().ToString() + "  " + comp.GetType().Name);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(comp.GetType().Name);
                    entity.AddComponentSilent((ECSComponent)comp.Clone());
                }
            }
            //var checkData = EntitySerialization.FullSerialize(entity); // fill json serialization cache
            entity.entityComponents.OnEntityDelete();
            if (GlobalProgramState.programType == GlobalProgramState.ProgramType.Client)
                GlobalProgramComponentGroup = new ClientComponentGroup();
            else if(GlobalProgramState.programType == GlobalProgramState.ProgramType.Server)
                GlobalProgramComponentGroup = new ServerComponentGroup();
        }
    }
}
