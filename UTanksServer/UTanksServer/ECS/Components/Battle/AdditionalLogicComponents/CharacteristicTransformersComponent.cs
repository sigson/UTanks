using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.Weapon
{
    [TypeUid(188606579688654300)]
    public class CharacteristicTransformersComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        public WeaponComponent sourceDamageComponent;
        [NonSerialized]
        public WeaponComponent damageComponent;
        [NonSerialized]
        //component instance id, func of transform, recalc after change, args: entity owner, component
        public ConcurrentDictionary<long, ICharacteristicTransformerComponent> characteristicTransformers = new ConcurrentDictionary<long, ICharacteristicTransformerComponent>();
        [NonSerialized]
        //disable some of transformers(turret/hull modules)
        public ConcurrentDictionary<long, bool> disabledTransformers = new ConcurrentDictionary<long, bool>();
    }
}
