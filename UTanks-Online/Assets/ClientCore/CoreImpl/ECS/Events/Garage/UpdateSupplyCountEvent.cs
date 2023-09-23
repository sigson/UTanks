using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;

namespace Assets.ClientCore.CoreImpl.ECS.Events.Garage
{
    [TypeUid(229658540996011970)]
    public class UpdateSupplyCountEvent : ECSEvent
    {
        static public new long Id { get; set; }
        public List<Supply> InBattleSupply = new List<Supply>();
        public List<Supply> SyncSupply = new List<Supply>();
        //public Dictionary<string, int> GarageSupply = new Dictionary<string, int>();
        public override void Execute()
        {
            if(ManagerScope.entityManager.EntityStorage.TryGetValue(this.EntityOwnerId, out var playerEntity))
            {
                BattleManager.LoadedBattleClientAction(playerEntity, (battleManagerObj) =>
                {
                    var battleManager = battleManagerObj as BattleManager;
                    var garage = UIService.instance.garageUIHandler;
                    var battle = UIService.instance.battleUIHandler;
                    foreach (var synced in SyncSupply)
                    {
                        if (garage.GarageItemsListRepresentation.ContainsKey(synced.PathName))
                        {
                            garage.GarageItemsDB[synced.PathName].ItemCount = synced.Count;
                            garage.GarageItemsDB[synced.PathName].UpdateData();
                        }
                        if (garage.GarageItemsDB.ContainsKey(synced.PathName))
                        {
                            garage.GarageItemsDB[synced.PathName].ItemCount = synced.Count;
                            garage.GarageItemsDB[synced.PathName].UpdateData();
                        }
                        if (battle.Supplies.ContainsKey(synced.PathName))
                        {
                            battle.Supplies[synced.PathName].GetComponent<BattleSupplyElement>().supplyCount.text = synced.Count.ToString();
                        }
                        else
                        {
                            var entity = playerEntity;
                            entity.GetComponent<UserBattleGarageDBComponent>().battleEquipment.Supplies.Add(synced);
                            battleManager.BattleSupplyUIPrepare(entity, ManagerScope.entityManager.EntityStorage[entity.GetComponent<BattleOwnerComponent>().BattleInstanceId].GetComponent<BattleComponent>());
                            battle.Supplies[synced.PathName].GetComponent<BattleSupplyElement>().supplyCount.text = synced.Count.ToString();

                        }
                    }
                    foreach (var synced in InBattleSupply)
                    {
                        if (battle.Supplies.ContainsKey(synced.PathName))
                        {
                            battle.Supplies[synced.PathName].GetComponent<BattleSupplyElement>().supplyCount.text = synced.Count.ToString();
                        }
                        else
                        {
                            var entity = playerEntity;
                            entity.GetComponent<UserBattleGarageDBComponent>().battleEquipment.Supplies.Add(synced);
                            battleManager.BattleSupplyUIPrepare(entity, ManagerScope.entityManager.EntityStorage[entity.GetComponent<BattleOwnerComponent>().BattleInstanceId].GetComponent<BattleComponent>());
                            battle.Supplies[synced.PathName].GetComponent<BattleSupplyElement>().supplyCount.text = synced.Count.ToString();

                        }
                    }
                });
            }
            
        }
    }
}
