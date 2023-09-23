using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Weapon
{
    [TypeUid(188606579688654300)]
    public class CharacteristicTransformerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public WeaponComponent sourceDamageComponent;
        public WeaponComponent damageComponent;
        ////component instance id, func of transform, recalc after change, args: entity owner, component
        //public ConcurrentDictionary<long, Action<ECSEntity, ECSComponent>> characteristicTransformers = new ConcurrentDictionary<long, Action<ECSEntity, ECSComponent>>();

        ////disable some of transformers(turret/hull modules)
        //public ConcurrentDictionary<long, bool> disabledTransformers = new ConcurrentDictionary<long, bool>();
    }
}
