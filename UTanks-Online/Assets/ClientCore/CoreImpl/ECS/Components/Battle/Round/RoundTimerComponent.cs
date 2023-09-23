using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components.Battle.Round
{
    [TypeUid(231444874950937060)]
    public class RoundTimerComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public RoundTimerComponent() { }

        public RoundTimerComponent(int minutes)
        {

        }

    }
}
