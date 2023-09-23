using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.Extensions
{
    public static class ECSAssemblyExtensions
    {
        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            var allAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in allAssembly)
                foreach (var t in a.GetTypes())
                    //if (t.IsAssignableTo(parent)) yield return t;
                    if (t.IsSubclassOf(parent))
                    {
                        yield return t;
                        //foreach(var xt in GetAllSubclassOf(t))
                        //    yield return xt;
                    }
        }
    }
}
