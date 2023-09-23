using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.BattleComponents
{
    [TypeUid(177121168562783230)]
    public class BattleGameplayEntitiesComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //long is entity id
        public List<long> GameplayEntities = new List<long>();
    }
}
