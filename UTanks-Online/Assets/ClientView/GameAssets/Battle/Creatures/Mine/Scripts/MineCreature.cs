using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using Assets.ClientCore.CoreImpl.ECS.Events.Battle.TankEvents.Shooting;
using Assets.ClientCore.CoreImpl.Network.NetworkEvents.FastGameEvents;
using SecuredSpace.ClientControl.Managers;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Battle.Tank;
using SecuredSpace.Effects;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Creatures;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.Team;
using UTanksClient.Extensions;
using static UTanksClient.ECS.ECSCore.ComponentsDBComponent;

namespace SecuredSpace.Battle.Creatures
{
    public class MineCreature : ICreature
    {
        public static Dictionary<TankManager, HashSet<MineCreature>> enteredManagers = new Dictionary<TankManager, HashSet<MineCreature>>();
        public AnimationScript MineDispellingAnim;
        public AnimationScript MineExplosiveAnim;
        public Renderer mineRenderer;
        public AudioAnchor audioSource;
        public ILight exploseLight;
        public HashSet<TankManager> enteredColliders = new HashSet<TankManager>();
        private bool sendedTarget = false;

        public static new ICreature Create(ICreatureComponent creatureComponent, BattleManager battleManager)
        {
            //var config = ClientInitService.instance.ItemResourcesDBOld.BattleObjectsResourcesDB[]["Main"];
            var resources = ResourcesService.instance.GameAssets.GetDirectory("battle\\creature\\mine").FillChildContentToItem();
            var battleEntity = ManagerScope.entityManager.EntityStorage[creatureComponent.ownerEntity.GetComponent<BattleOwnerComponent>().BattleInstanceId];
            var color = battleEntity.GetComponent<BattleSimpleInfoComponent>().battleSimpleInfo.Commands.Where(x => x.Value.commandPlayers.ContainsKey(creatureComponent.ownerEntity.instanceId)).ToList()[0].Value.TeamColor;
            battleEntity.GetComponent<EntityManagersComponent>().TryGetValue<MapManager>(out var mapManager);
            var creatureObj = Instantiate(ResourcesService.instance.GetPrefab("battle\\creature\\mine"), mapManager.CreatureSpace.transform);
            var creature = creatureObj.GetComponent<MineCreature>();
            creature.CreatureOwnerId = creatureComponent.ownerEntity.instanceId;
            creatureObj.transform.position = creatureComponent.WorldPositon.Position.ConvertToUnityVector3();
            var rotat = creatureComponent.WorldPositon.Rotation.ConvertToUnityVector3();
            creatureObj.transform.rotation = Quaternion.Euler(rotat.x, rotat.y, rotat.z);
            creature.CreatureInstanceId = creatureComponent.instanceId;
            creature.mineRenderer.material.SetColor("_ColorShade1", ColorEx.ToColor(TeamColors.colors["dm"]));
            creature.isPrefabScript = false;
            creature.MineExplosiveAnim.afterPlayAction = (gameobj) =>
            {
                Destroy(gameobj.transform.parent.gameObject);
            };
            creature.MineDispellingAnim.afterPlayAction = (gameobj) =>
            {
                Destroy(gameobj.transform.parent.gameObject);
            };
            return creature;
        }

        public override void AddManager()
        {
            
        }

        public override void UpdateCreatureState(ICreatureComponent creatureComponent, ComponentState state)
        {
            base.UpdateCreatureState(creatureComponent, state);
            var mineCreature = (creatureComponent as MineCreatureComponent);
            if (state == ComponentState.Removed)
            {
                if (mineCreature.mineState == MineState.Explosed)
                {
                    MineExplosiveAnim.gameObject.SetActive(true);
                    mineRenderer.enabled = false;
                    MineExplosiveAnim.Play();
                    enteredColliders.ForEach(x => x.Hull.GetComponent<Rigidbody>().AddForceAtPosition((this.transform.position - x.transform.position).normalized * 50000f, this.transform.position));
                }
                else
                {
                    MineDispellingAnim.gameObject.SetActive(true);
                    mineRenderer.enabled = false;
                    MineDispellingAnim.Play();
                }
                //this.gameObject.SetActive(false);
            }
        }

        protected override void OnActivateManager()
        {
            
        }

        protected override void OnAwakeManager()
        {
            
        }

        protected override void OnDeactivateManager()
        {
            
        }

        protected override void OnRemoveManager()
        {
            
        }

        protected override void OnStartManager()
        {
            
        }
        protected override void OnTriggerEnterManager(Collider other)
        {
            base.OnTriggerEnterManager(other);
            if (other.tag == "Tank" && !sendedTarget && (ownerTankManager == null || ownerTankManager.ManagerEntityId == ClientNetworkService.instance.PlayerEntityId))
            {
                var parentManager = other.GetComponent<IManagableAnchor>();
                if (parentManager != null && !parentManager.ownerManager<TankManager>().checkGhost && !parentManager.ownerManager<TankManager>().Ghost && !parentManager.ownerManager<TankManager>().deadTank)
                {
                    var tankManager = parentManager.ownerManager<TankManager>();

                    //if(enteredManagers.ContainsKey(tankManager))
                    //    enteredManagers[tankManager].Add(this);
                    //else
                    //    enteredManagers[tankManager] = new HashSet<MineCreature>() { this };
                    if (ownerTankManager == null)
                        ownerTankManager = tankManager.battleManager[CreatureOwnerId].GetComponent<EntityManagersComponent>().Get<TankManager>();
                    if (ownerTankManager.ManagerEntityId != ClientNetworkService.instance.PlayerEntityId)
                        return;
                    enteredColliders.Add(tankManager);
                    if ((ownerTankManager.TeamColor.ToLower() == "dm" && tankManager != ownerTankManager) || ownerTankManager.playerCommandEntityId != tankManager.playerCommandEntityId)
                    {
                        ClientNetworkService.instance.Socket.emit(new RawCreatureActuationEvent()
                        {
                            BattleDBOwnerId = tankManager.battleManager.NowBattleId,
                            CreatureInstanceId = CreatureInstanceId,
                            TargetsId = new List<long>() { tankManager.ManagerEntityId }
                        });
                        sendedTarget = true;
                    }
                }
            }
        }

        protected override void OnTriggerExitManager(Collider other)
        {
            base.OnTriggerExitManager(other);
            if (other.tag == "Tank")
            {
                var parentManager = other.GetComponent<IManagableAnchor>();
                if (parentManager != null)
                    if (enteredColliders.Contains(parentManager.ownerManager<TankManager>()))
                    {
                        enteredColliders.Remove(parentManager.ownerManager<TankManager>());
                        //enteredManagers[parentManager.parentTankManager].Remove(this);
                    }
            }
        }
    }
}