using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Services;

namespace UTanksServer.ECS.Components.Battle.AdditionalLogicComponents
{
    [TypeUid(181300335650133500)]
    public class CreatureContainerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        public List<ConfigObj> creatureContainerComponent = new List<ConfigObj>();
    }
}
