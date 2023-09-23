using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.ECS.ECSCore
{
    class ECSObjectsEnum
    {
        Dictionary<long, dynamic> AllTypes;

        public void Initialize()
        {

        }

        public void Instantiate()
        {

        }
        static IEnumerable<string> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace)
                .Select(type => type.Name);
        }
    }
}
