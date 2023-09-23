using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(211367938871949540)]
    public class ICreatureComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WorldPoint WorldPositon;
        private long savedCreatureID = 0;
        public long CreatureComponentID
        {
            get
            {
                if (savedCreatureID == 0)
                    return this.GetId();
                else
                    return savedCreatureID;
            }
            set
            {
                savedCreatureID = value;
            }
        }
    }
}
