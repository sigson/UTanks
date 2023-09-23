using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.Battle.AdditionalLogicComponents
{
    [TypeUid(183511394984287970)]
    public class CharacteristicTransformerContainerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        public List<ICharacteristicTransformerComponent> characteristicTransformerComponent = new List<ICharacteristicTransformerComponent>();
    }
}
