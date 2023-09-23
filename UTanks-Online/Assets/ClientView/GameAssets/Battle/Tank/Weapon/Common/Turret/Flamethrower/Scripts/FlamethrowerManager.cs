using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class FlamethrowerManager : ITurret
    {
        public float verticalAngle;
        public float horizontalAngle;
        public float distance;
        public JetManager jetManager;
        public FlamethrowerDamageComponent flameComponent;
        private bool Waited = false;

        public override ITurret AppendManagerToObject(GameObject turretObject)
        {
            return turretObject.AddComponent<FlamethrowerManager>();
        }

        public void OnEnable()
        {
            StopCoroutine(this.LocalWaiter());
            StartCoroutine(this.LocalWaiter());
        }
        public override void Initialize(TankManager tankManager, ECSEntity player)
        {
            base.Initialize(tankManager, player);
            var aim = tankManager.closeDistanceAIM;
            flameComponent = player.GetComponent<FlamethrowerDamageComponent>();
            aim.verticalAngle = tankManager.turretGameplayObj.GetObject<string>($"conicTargeting\\halfConeAngle").FastFloat();
            aim.horizontalAngle = tankManager.turretGameplayObj.GetObject<string>($"conicTargeting\\halfConeAngle").FastFloat();
            //aim.distance = isisComponent.maxShotDistanceProperty;
            aim.AOETargeting = true;
            aim.MultiplyTargetsMode = false;
            aim.RebuildAIM();
            horizontalAngle = aim.horizontalAngle;
            distance = aim.distance;
            //turretRotaion.SpeedUp = isisComponent.turretSpeedRotationProperty;

            #region SpriteDefinition
            jetManager = Instantiate(tankManager.TurretResources.GetElement<GameObject>("jet"), tankManager.MuzzlePoint.transform).GetComponent<JetManager>();
            jetManager.SaveState();
            jetManager.HideJet();
            var jetLighting = jetManager.GetComponent<ParticlesLight>();
            jetLighting.lightColor = tankManager.TurretResources.ItemColors["Main"];
            jetLighting.UpdateLight();
            jetLighting.GetComponent<IAnchor>().DirectRegisterObject();
            //for (int i = 0; i < jetManager.transform.childCount; i++)
            //{

            //}
            weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("audio_flamethrower_start").UpdateSettings("audio_flamethrower_start", false, 1f));
            weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("audio_flamethrower_loop").UpdateSettings("audio_flamethrower_loop", true, 1f));
            weaponAudio.Build();
            #endregion

        }

        IEnumerator LocalWaiter()
        {
            while (true)
            {
                Waited = true;
                yield return new WaitForSeconds(0.01f);
            }
        }

        public override void RebuildTank(ECSEntity player)
        {
            base.RebuildTank(player);
        }

        public override void TurretEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretEvent(player, updatingEvent);
        }

        private bool pressed = false;
        private List<(TankManager, Vector3)> cacheTargetList;
        public override void InputOperations()
        {
            base.InputOperations();
            var ShootTurn = !ClientInitService.instance.LockInput ? Input.GetAxis("Shoot") : 0f;
            //if ( ShootTurn > 0 && parentTankManager.hullManager.chassisManager.TankMovable && parentTankManager.TestingMode)
            //{

            //    return;
            //}
            //
            //return;
            if (ShootTurn > 0 && parentTankManager.ManagerEntity.GetComponent<WeaponEnergyComponent>().Energy > 0 && (!parentTankManager.Ghost && !parentTankManager.checkGhost))
            {
                List<(TankManager, Vector3)> targetList = new List<(TankManager, Vector3)>();

                var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("flamethrower") == -1))
                {
                    weaponAudio.audioManager.StopAll();
                    weaponAudio.audioManager.ResetAll();
                    weaponAudio.audioManager.PlayBlock(new List<string> { "audio_flamethrower_start", "audio_flamethrower_loop" });
                }
                //for (int i = 0; i < jetManager.transform.childCount; i++)
                //{
                //    jetManager.transform.GetChild(i).gameObject.SetActive(true);
                //}
                if (cacheTargetList == null)
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                if (Waited)
                {
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                    Waited = false;
                }
                if (CheckMuzzleVisible() == Vector3.zero)
                {
                    targetList = cacheTargetList;
                    jetManager.ShowJet();
                }
                else
                {
                    jetManager.HideJet();
                }
                foreach (var target in targetList)
                {
                    RaycastHit raycastHit;
                    Physics.Linecast(parentTankManager.MuzzlePoint.transform.position, target.Item1.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Friction"));
                    if (target.Item1.playerCommandEntityId != parentTankManager.playerCommandEntityId)
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {

                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                        
                    }
                    else
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                    }
                }

                if (parentTankManager.ManagerEntity.HasComponent(WeaponTechCooldownComponent.Id))
                {
                    parentTankManager.ManagerEntity.GetComponent<WeaponTechCooldownComponent>().techObjects = targetList.Cast<object>().ToList();
                }
                else
                {
                    var weaponTechCooldown = new WeaponTechCooldownComponent(0.25f).SetGlobalComponentGroup() as WeaponTechCooldownComponent;

                    weaponTechCooldown.onEnd = (entity, component) =>
                    {
                        try
                        {
                            var typeComponent = component as WeaponTechCooldownComponent;
                            var cacheTech = new List<object>(typeComponent.techObjects);
                            if (cacheTech.Count == 0)
                            {
                                entity.RemoveComponent(typeComponent.GetId());
                                return;
                            }
                            Dictionary<long, Vector3S> hitList = new Dictionary<long, Vector3S>();
                            Dictionary<long, float> HitDistanceList = new Dictionary<long, float>();
                            foreach (var obj in cacheTech)
                            {
                                var typedObj = (ValueTuple<SecuredSpace.Battle.Tank.TankManager, Vector3>)obj;
                                hitList.Add(typedObj.Item1.ManagerEntityId, new Vector3S(typedObj.Item2));
                                HitDistanceList.Add(typedObj.Item1.ManagerEntityId, 0f);
                            }

                            TaskEx.RunAsync(() =>
                            {
                                ClientNetworkService.instance.Socket.emit(new RawHitEvent()
                                {
                                    hitList = hitList,
                                    hitDistanceList = HitDistanceList,
                                    hitLocalDistanceList = new Dictionary<long, float>()
                                });
                            });
                        }
                        finally
                        {
                            var timerComp = component as WeaponTechCooldownComponent;
                            if(entity.HasComponent<WeaponTechCooldownComponent>())
                                entity.RemoveComponent(timerComp.GetId());
                        }
                    };
                    weaponTechCooldown.techObjects = targetList.Cast<object>().ToList();
                    this.parentTankManager.ManagerEntity.AddComponent(weaponTechCooldown);
                }

                if (targetList.Count == 0)
                {
                    if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                    {
                        parentTankManager.ManagerEntity.AddComponent(new StreamWeaponIdleComponent().SetGlobalComponentGroup());
                    }
                    if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                    {
                        parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponWorkingComponent.Id);
                    }

                }
                pressed = true;
                return;
            }
            if (pressed)
            {
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponWorkingComponent>();
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponIdleComponent>();

                if (parentTankManager.ManagerEntity.HasComponent(WeaponTechCooldownComponent.Id))
                {
                    var weaponTechCooldown = parentTankManager.ManagerEntity.GetComponent<WeaponTechCooldownComponent>();
                    weaponTechCooldown.techObjects.Clear();
                }

                jetManager.HideJet();
                weaponAudio.audioManager.FadeAll();
                //for (int i = 0; i < jetManager.transform.childCount; i++)
                //{
                //    jetManager.transform.GetChild(i).gameObject.SetActive(false);
                //}
            }
            pressed = false;
        }

        bool ShootControl = false;
        public override void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretInfluenceEvent(player, updatingEvent);
            if (updatingEvent.GetId() == StartShootingEvent.Id)
            {
                ShootControl = true;
            }
            if (updatingEvent.GetId() == EndShootingEvent.Id)
            {
                ShootControl = false;
            }
        }

        public override void FixedUpd()
        {
            base.FixedUpd();
            if (parentTankManager.ManagerEntityId == ClientNetworkService.instance.PlayerEntityId)
                return;
            if (ShootControl)
            {
                jetManager.ShowJet();

                var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("flamethrower") == -1))
                {
                    weaponAudio.audioManager.StopAll();
                    weaponAudio.audioManager.ResetAll();
                    weaponAudio.audioManager.PlayBlock(new List<string> { "audio_flamethrower_start", "audio_flamethrower_loop" });
                }
                
                //for (int i = 0; i < jetManager.transform.childCount; i++)
                //{
                //    jetManager.transform.GetChild(i).gameObject.SetActive(true);
                //}
                List<(TankManager, Vector3)> targetList = new List<(TankManager, Vector3)>();
                if (cacheTargetList == null)
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                if (Waited)
                {
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                    Waited = false;
                }
                if (CheckMuzzleVisible() == Vector3.zero)
                {
                    targetList = cacheTargetList;
                    jetManager.ShowJet();
                }
                else
                {
                    jetManager.HideJet();
                }
                foreach (var target in targetList)
                {
                    RaycastHit raycastHit;
                    Physics.Linecast(parentTankManager.MuzzlePoint.transform.position, target.Item1.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Friction"));
                    if (target.Item1.playerCommandEntityId != parentTankManager.playerCommandEntityId)
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                    }
                    
                }

                if (targetList.Count == 0)
                {
                    if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                    {
                        parentTankManager.ManagerEntity.AddComponent(new StreamWeaponIdleComponent().SetGlobalComponentGroup());
                    }
                    if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                    {
                        parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponWorkingComponent.Id);
                    }

                }
                pressed = true;
                return;
            }
            if (pressed)
            {
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponWorkingComponent>();
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponIdleComponent>();
                jetManager.HideJet();
                weaponAudio.audioManager.FadeAll();
                //for (int i = 0; i < jetManager.transform.childCount; i++)
                //{
                //    jetManager.transform.GetChild(i).gameObject.SetActive(false);
                //}
            }
            pressed = false;
        }

        public override void DestroyTurret(ECSEntity player)
        {
            base.DestroyTurret(player);
            parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponWorkingComponent>();
            parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponIdleComponent>();

            if (parentTankManager.ManagerEntity.HasComponent(WeaponTechCooldownComponent.Id))
            {
                var weaponTechCooldown = parentTankManager.ManagerEntity.GetComponent<WeaponTechCooldownComponent>();
                weaponTechCooldown.techObjects.Clear();
            }
            jetManager.HideJet();
            pressed = false;
        }
    }
}