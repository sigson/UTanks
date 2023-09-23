using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(211691963638678620)]
    public abstract class TankConstructionComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public int ComponentGrade { get; set; }

        public abstract TankConstructionComponent UpdateComponent(TankConstructionComponent weaponComponent);
    }
}
