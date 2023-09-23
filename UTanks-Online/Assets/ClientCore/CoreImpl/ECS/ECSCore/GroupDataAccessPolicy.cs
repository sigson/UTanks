using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;

namespace UTanksClient.ECS.ECSCore
{
    public class GroupDataAccessPolicy
    {
        public static long Id;
        public long instanceId = Guid.NewGuid().GuidToLong();
        public Type GroupDataAccessPolicyType;
        protected long ReflectionId = 0;
        //public List<long> AvailableComponents = new List<long>();
        //public List<long> RestrictedComponents = new List<long>();
        
        //public string JsonAvailableComponents = "";
        //public string JsonRestrictedComponents = "";
        //public bool IncludeRemovedAvailable = false;
        //public bool IncludeRemovedRestricted = false;

        //public static string ComponentsFilter(ECSEntity baseEntity, ECSEntity otherEntity)
        //{
        //    string filteredComponents = "";
        //    bool includeRemovedAvailable = false;
        //    bool includeRemovedRestricted = false;
        //    foreach (var baseDataAP in baseEntity.dataAccessPolicies)
        //    {
        //        foreach (var otherDataAP in otherEntity.dataAccessPolicies)
        //        {
        //            if (baseDataAP.instanceId == otherDataAP.instanceId)
        //            {
        //                filteredComponents += otherDataAP.JsonAvailableComponents;
        //                if (otherDataAP.IncludeRemovedAvailable)
        //                    includeRemovedAvailable = true;
        //            }
        //            else if (baseDataAP.GetId() == otherDataAP.GetId())
        //            {
        //                filteredComponents += otherDataAP.JsonRestrictedComponents;
        //                if (otherDataAP.IncludeRemovedRestricted)
        //                    includeRemovedRestricted = true;
        //            }
        //        }
        //    }
        //    if (filteredComponents == "" && (includeRemovedAvailable || includeRemovedRestricted))
        //        return "#INCLUDEREMOVED#";
        //    else
        //        return filteredComponents;
        //}

        //public static (string, List<long>) ComponentsFilterRawStand(ECSEntity baseEntity, ECSEntity otherEntity)
        //{
        //    string filteredComponents = "";
        //    List<long> filteredComponentsId = new List<long>();
        //    foreach (var baseDataAP in baseEntity.dataAccessPolicies)
        //    {
        //        foreach (var otherDataAP in otherEntity.dataAccessPolicies)
        //        {
        //            if (baseDataAP.instanceId == otherDataAP.instanceId)
        //            {
        //                filteredComponents += otherDataAP.JsonAvailableComponents;
        //                filteredComponentsId = filteredComponentsId.Concat(otherDataAP.AvailableComponents).ToList();
        //            }
        //            else if (baseDataAP.GetId() == otherDataAP.GetId())
        //            {
        //                filteredComponents += otherDataAP.JsonRestrictedComponents;
        //                filteredComponentsId = filteredComponentsId.Concat(otherDataAP.RestrictedComponents).ToList();
        //            }
        //        }
        //    }
        //    return (filteredComponents, filteredComponentsId);
        //}
        //public static List<long> RawComponentsFilter(ECSEntity baseEntity, ECSEntity otherEntity)
        //{
        //    List<long> filteredComponents = new List<long>();
        //    foreach (var baseDataAP in baseEntity.dataAccessPolicies)
        //    {
        //        foreach (var otherDataAP in otherEntity.dataAccessPolicies)
        //        {
        //            if (baseDataAP.instanceId == otherDataAP.instanceId)
        //            {
        //                filteredComponents = filteredComponents.Concat(otherDataAP.AvailableComponents).ToList();
        //            }
        //            else if (baseDataAP.GetId() == otherDataAP.GetId())
        //            {
        //                filteredComponents = filteredComponents.Concat(otherDataAP.RestrictedComponents).ToList();
        //            }
        //        }
        //    }
        //    return filteredComponents;
        //}

        public long GetId()
        {
            if (Id == 0)
                try
                {
                    if (GroupDataAccessPolicyType == null)
                    {
                        GroupDataAccessPolicyType = GetType();
                    }
                    if (ReflectionId == 0)
                        ReflectionId = GroupDataAccessPolicyType.GetCustomAttribute<TypeUidAttribute>().Id;
                    return ReflectionId;
                }
                catch
                {
                    ULogger.Error(this.GetType().ToString() + "Could not find Id field");
                    return 0;
                }
            else
                return Id;
        }
        public object Clone() => MemberwiseClone();
    }

}
