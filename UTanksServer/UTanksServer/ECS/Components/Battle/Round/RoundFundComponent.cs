using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.Extensions;

namespace UTanksServer.ECS.Components.Battle.Round
{
    [TypeUid(226975893288601060)]
    public class RoundFundComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        long StartDateTime = DateTime.Now.Ticks;
        [NonSerialized]
        [JsonIgnore]
        long LastSectionDateTime = DateTime.Now.Ticks;
        [NonSerialized]
        [JsonIgnore]
        float SectionActionUpdates = 0;
        [NonSerialized]
        [JsonIgnore]
        public float SectionCoef = 0;
        [NonSerialized]
        [JsonIgnore]
        public float TargetSectionCoef = 0;
        public RoundFundComponent() { }

        public RoundFundComponent(MapComponent mapComponent)
        {
            //FundAction
            TargetSectionCoef = GlobalGameDataConfig.SelectableMap.selectableMaps.FundTimeSection / GlobalGameDataConfig.SelectableMap.selectableMaps.FundActionPerTimeSection * mapComponent.map.FundScaling;
            UpdateFundActions();
            //TimeActionAmount += (int)(ActionCoef * i);
            //for (int i = 1; i < GlobalGameDataConfig.SelectableMap.selectableMaps.FundActionPerTimeSection+1; i++)
            //{
                
            //}
        }

        public RoundFundComponent(float actionCoef)
        {
            //FundAction
            TargetSectionCoef = actionCoef;
            UpdateFundActions();
            //TimeActionAmount += (int)(ActionCoef * i);
            //for (int i = 1; i < GlobalGameDataConfig.SelectableMap.selectableMaps.FundActionPerTimeSection+1; i++)
            //{

            //}
        }

        public void UpdateFundActions()
        {
            lock(locker)
            {
                //TimeActionAmount -= Math.Abs(FundAction[1] - FundAction[0]);
                var nowTime = DateTime.Now.Ticks;
                if (nowTime - LastSectionDateTime == 0)
                {
                    SectionCoef = GlobalGameDataConfig.SelectableMap.selectableMaps.FundMaxPositiveCompensation;
                }
                else
                {
                    SectionCoef = DateTimeExtensions.TicksToSeconds(nowTime - LastSectionDateTime) / TargetSectionCoef;
                    SectionCoef = SectionCoef > GlobalGameDataConfig.SelectableMap.selectableMaps.FundMaxPositiveCompensation ? GlobalGameDataConfig.SelectableMap.selectableMaps.FundMaxPositiveCompensation : SectionCoef;
                }
                LastSectionDateTime = nowTime;
            }
        }

        
        [JsonIgnore]
        public float FundCoef => SectionCoef == 0f? 0f : SectionCoef;
        [NonSerialized]
        private float realFund;
        public float Fund {
            get {
                return (int)realFund;
            }
            set {
                realFund += value;
            }
        }
    }
}
