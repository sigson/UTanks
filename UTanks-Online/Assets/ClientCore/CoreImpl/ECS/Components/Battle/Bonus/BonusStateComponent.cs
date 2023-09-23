using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle.Bonus
{
    [TypeUid(210663413977887550)]
    public class BonusStateComponent : ECSComponent //deprecated
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BonusState bonusState;
        //this, who taken
        //public Action<ECSEntity, ECSEntity> onTaken;
    }
}
