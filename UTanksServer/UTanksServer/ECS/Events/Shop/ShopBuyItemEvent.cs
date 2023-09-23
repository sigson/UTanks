using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Events.Shop
{
    [TypeUid(213990956531693300)]
    public class ShopBuyItemEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long ItemId;
        public string ConfigPath;
        public override void Execute()
        {
            //throw new NotImplementedException();
        }
    }
}
