using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Tank
{
    [TypeUid(206052512006924320)]
    public class TankCooldownStorageComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public Dictionary<string, long> CooldownStorage = new Dictionary<string, long>();
        public TankCooldownStorageComponent() { }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);

        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            this.TimerStop();

        }
    }
}
