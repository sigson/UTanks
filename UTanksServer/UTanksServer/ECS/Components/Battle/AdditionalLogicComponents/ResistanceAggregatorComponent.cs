using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(165935233787829660)]
    public class ResistanceAggregatorComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        //long key - for component from whom we store resistance method, component who changed(isisDamage, SmokyDamage), effect component, component whom changing(health, weapon), value of changing(its mainly float).  From that who component typed - references working logic 
        public ConcurrentDictionary<long, Action<ECSComponent, ECSComponent, ECSComponent, float>> resistanceAggregator = new ConcurrentDictionary<long, Action<ECSComponent, ECSComponent, ECSComponent, float>>();
        [NonSerialized]
        public ConcurrentDictionary<long, bool> disabledResistance = new ConcurrentDictionary<long, bool>();
    }
}
