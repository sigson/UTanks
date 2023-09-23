using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Components.Battle.Round
{
    [TypeUid(226975893288601060)]
    public class RoundFundComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        long StartDateTime = DateTime.Now.Ticks;

        List<int> FundAction = new List<int>();
        int TimeActionAmount = 0;
        float ActionCoef = 0;
        public RoundFundComponent() { }

        private float realFund;
        public float Fund
        {
            get
            {
                return (int)realFund;
            }
            set
            {
                realFund = value;
            }
        }
    }
}
