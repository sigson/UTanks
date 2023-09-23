using Assets.ClientCore.CoreImpl.ECS.Types.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle
{
    [TypeUid(175153277831607600)]
    public class BattleSimpleInfoComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public BattleSimpleInfo battleSimpleInfo = new BattleSimpleInfo();

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
        }
    }
}
