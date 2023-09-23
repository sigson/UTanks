using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.Battle.Drop;
using System;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace UTanksClient.ECS.Components.Battle.Bonus
{
    [TypeUid(8101904939955946870)]
    public class BonusComponent : ECSComponent
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
        //this, who taken
        //public Action<ECSEntity, ECSEntity> onTaken;

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            ClientInitService.instance.ExecuteInstruction((object Obj) =>
            {
                if(!this.componentManagers.ContainsKey(typeof(DropManager)))
                    this.componentManagers.Add(typeof(DropManager), new DropManager());
                DropManager.UpdateDrop(this);
            }, null);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            ClientInitService.instance.ExecuteInstruction((object Obj) =>
            {
                //if (!this.componentManagers.ContainsKey(typeof(DropManager)))
                //    this.componentManagers.Add(typeof(DropManager), new DropManager());
                DropManager.UpdateDrop(this);
            }, null);

        }
    }
}