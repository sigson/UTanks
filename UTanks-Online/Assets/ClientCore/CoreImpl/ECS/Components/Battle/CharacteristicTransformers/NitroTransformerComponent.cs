using Newtonsoft.Json;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Components;
using SecuredSpace.Battle.Tank;

namespace Assets.ClientCore.CoreImpl.ECS.Components.Battle.CharacteristicTransformers
{
    [TypeUid(206099871123258300)]
    public class NitroTransformerComponent : TimerComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }

        public double TransformerTimerAwait;
        public NitroTransformerComponent() { }

        public override void OnAdded(ECSEntity entity)
        {
            base.OnAdded(entity);
            if (entity.GetComponent<EntityManagersComponent>().TryGetValue<TankManager>(out var tankManager))
            {
                tankManager.ExecuteInstruction(() => {
                    tankManager.tankUI.GetComponent<TankUI>().UpdateSupplyInfo(entity);
                    if (ClientInitService.instance.CheckEntityIsPlayer(entity))
                    {
                        UIService.instance.battleUIHandler.Supplies["garage\\supplies\\nitroboost"].GetComponent<BattleSupplyElement>().SetupTimer(Convert.ToSingle(TimeRemaining / 1000));
                    }
                }, "Error access tank ui");
            }
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
