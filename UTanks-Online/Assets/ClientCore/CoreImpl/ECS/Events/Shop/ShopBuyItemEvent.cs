using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Shop
{
    [TypeUid(213990956531693300)]
    public class ShopBuyItemEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long ItemId;
        public string ConfigPath;
        public override void Execute()
        {
            //
        }
    }
}
