using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle.Tank;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.ECSCore;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle.CharacteristicTransformers
{
    [TypeUid(214811788313734370)]
    public class ArmorTransformerComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public double TransformerTimerAwait;
        public ArmorTransformerComponent() { }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);

            if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                    if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                    {
                        UIService.instance.battleUIHandler.Supplies["garage\\supplies\\doublearmor"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle(TimeRemaining / 1000));
                    }
                }, "Error access tank ui");
            }

            //ClientInitService.instance.battleManager.ExecuteInstruction((object Obj) =>
            //{
            //    try
            //    {
                    
            //        ClientInitService.instance.battleManager.BattleTanksDB[entity.instanceId].
            //        if(ClientInitService.instance.CheckEntityIsPlayer(entity))
            //        {
            //            UIService.instance.battleUIHandler.Supplies["DoubleArmor"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle(TimeRemaining/1000));
            //        }
            //    }
            //    catch { ULogger.Log("Error access tank ui"); }
                
            //}, null);
            //this.TimerStart(TransformerTimerAwait, entity, false, false);
        }

        public override void OnRemoving(ECSEntity entity)
        {
            base.OnRemoving(entity);
            if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                }, "Error access tank ui");
            }
        }
        public bool stableTransformer { get; set; } = false;
    }
}
