using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.ECS.Components.Battle.AdditionalLogicComponents
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
    
        public virtual void OnCreatureActuation(List<ECSEntity> actuationTargets, BattleCreatureStorageComponent creatureStorage)
        {

        }

        public override void OnAdded(ECSEntity entity)
        {
            
        }

        public override void OnRemoving(ECSEntity entity)
        {
            
        }
    }
}
