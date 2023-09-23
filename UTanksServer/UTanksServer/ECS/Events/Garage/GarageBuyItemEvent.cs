using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Garage
{
    [TypeUid(182674787604609800)]
    public class GarageBuyItemEvent : ECSEvent
    {
        static public new long Id { get; set; }

        public long ItemId;
        public string ConfigPath;
        public string SkinConfigPath;
        public int Grade;
        public int Count;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
