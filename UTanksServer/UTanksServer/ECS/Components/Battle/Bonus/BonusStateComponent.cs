using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.Bonus;

namespace UTanksServer.ECS.Components.Battle.Bonus
{
    [TypeUid(210663413977887550)]
    public class BonusStateComponent_Deprecated : ECSComponent //deprecated, bonus now is creature
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusState bonusState;
        //this, who taken
        //public Action<ECSEntity, ECSEntity> onTaken;
    }
}
