using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(214260570342458180)]
    public class EffectsAggregatorComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        ////component instance id, func of transform, args: entity attacker, entity attacked, random salt, hit event
        //public ConcurrentDictionary<long, Action<ECSEntity, ECSEntity, float, ECSEvent>> effectsAggregator = new ConcurrentDictionary<long, Action<ECSEntity, ECSEntity, float, ECSEvent>>();

        ////disable some of effects(turret/hull modules)
        //public ConcurrentDictionary<long, bool> disabledEffects = new ConcurrentDictionary<long, bool>();
    }
}
