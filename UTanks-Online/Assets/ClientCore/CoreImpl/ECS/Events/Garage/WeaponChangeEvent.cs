using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Events.Garage
{
    [TypeUid(211565668921936770)]
    public class WeaponChangeEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public long ItemId;
        public string ConfigPath;
		public string SkinConfigPath;
        public int Grade;
        public override void Execute()
        {
            //
        }
    }
}
