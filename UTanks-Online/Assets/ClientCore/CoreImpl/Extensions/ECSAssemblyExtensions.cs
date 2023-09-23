using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksClient.Extensions
{
    public static class ECSAssemblyExtensions
    {
        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in a.GetTypes())
                    if (t.IsSubclassOf(parent)) yield return t;
        }
    }
}
