using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;

namespace UTanksServer.ECS.Components.Battle
{
    [TypeUid(175153277831607600)]
    public class BattleSimpleInfoComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleSimpleInfo battleSimpleInfo = new BattleSimpleInfo();
        [JsonIgnore]
        public bool MarkedToUpdate = false;
    }
}
