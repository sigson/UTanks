using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.Bonus;

namespace UTanksServer.ECS.Components.Battle.Bonus
{
    [TypeUid(8101904939955946870)]
    public class BonusComponent : ICreatureComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public string BonusConfig;
        public string BonusEra;
        public string BonusVersion;
        public BonusType bonusType;
        public BonusState bonusState;//spawn bonus state
        public long DateTimeDropped;
        public Vector3S position;
        [NonSerialized]
        public CharacteristicTransformerContainerComponent CharacteristicTransformerContainer = new CharacteristicTransformerContainerComponent();
        //this, who taken
        //public Action<ECSEntity, ECSEntity> onTaken;

        public BonusComponent() { }
        public BonusComponent(float despawnSecTime)
        {
            timerAwait = despawnSecTime * 1000;
            onEnd = (entity, selfDestructComponent) =>
            {
                if ((this.bonusState != Types.Battle.Bonus.BonusState.Taken || this.bonusState != Types.Battle.Bonus.BonusState.Taking))
                {
                    this.bonusState = Types.Battle.Bonus.BonusState.Despawned;
                    var battleDropStorage = ownerEntity.GetComponent<BattleDropStorageComponent>();
                    battleDropStorage.RemoveComponent(this.instanceId, this.ownerEntity);
                    battleDropStorage.MarkAsChanged();
                }
            };
        }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            this.TimerStart(timerAwait, entity, false, false);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            this.TimerStop();
        }
    }
}