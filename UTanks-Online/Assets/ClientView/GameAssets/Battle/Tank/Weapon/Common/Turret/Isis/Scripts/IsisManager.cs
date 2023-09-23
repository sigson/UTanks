using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using Assets.ClientCore.CoreImpl.ECS.Components.Battle;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.Important.Aim;
using SecuredSpace.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class IsisManager : ITurret
    {
        public float verticalAngle;
        public float horizontalAngle;
        public float distance;
        public GameObject damageEndSprite;
        public GameObject damageShaftLine;
        public GameObject damageStartSprite;
        public GameObject healEndSprite;
        public GameObject healShaftLine;
        public GameObject healStartSprite;
        public GameObject idleSparkSprite;
        public IsisDamageComponent isisComponent;
        private bool Waited = false;

        public void OnEnable()
        {
            StopCoroutine(this.LocalWaiter());
            StartCoroutine(this.LocalWaiter());
        }
        public override void Initialize(TankManager tankManager, ECSEntity player)
        {
            base.Initialize(tankManager, player);
            var aim = tankManager.closeDistanceAIM;
            isisComponent = player.GetComponent<IsisDamageComponent>();
            aim.verticalAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["conicTargeting"]["halfConeAngle"].ToString(), CultureInfo.InvariantCulture);
            aim.horizontalAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["conicTargeting"]["halfConeAngle"].ToString(), CultureInfo.InvariantCulture);
            //aim.distance = isisComponent.maxShotDistanceProperty;
            aim.AOETargeting = false;
            aim.MultiplyTargetsMode = false;
            aim.RebuildAIM();
            horizontalAngle = aim.horizontalAngle;
            distance = aim.distance;

            weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("audio_damage_start").UpdateSettings("audio_damage_start", false, 1f));
            weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("audio_damage_loop").UpdateSettings("audio_damage_loop", true, 1f));
            //weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("healing_loop").Update(true));
           // weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("healing_start"));
            //weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("idle_loop").Update(true));
            //weaponAudio.audioStorage.Add(parentTankManager.TurretResources.GetElement<AudioSourceSetting>("idle_start"));
            weaponAudio.Build();

            //turretRotaion.SpeedUp = isisComponent.turretSpeedRotationProperty;

            #region SpriteDefinition
            damageEndSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("damage_end"), tankManager.MuzzlePoint.transform);
            damageEndSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["MainDamage"];
            damageEndSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            damageEndSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            damageShaftLine = Instantiate(tankManager.TurretResources.GetElement<GameObject>("damage_shaft"), tankManager.MuzzlePoint.transform);
            damageShaftLine.GetComponent<LineLight>().LightColor = tankManager.TurretResources.ItemColors["MainDamage"];

            damageStartSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("damage_start"), tankManager.MuzzlePoint.transform);
            damageStartSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["MainDamage"];
            damageStartSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            damageStartSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            healEndSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("heal_end"), tankManager.MuzzlePoint.transform);
            healEndSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["MainHeal"];
            healEndSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            healEndSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            healShaftLine = Instantiate(tankManager.TurretResources.GetElement<GameObject>("heal_shaft"), tankManager.MuzzlePoint.transform);
            healShaftLine.GetComponent<LineLight>().LightColor = tankManager.TurretResources.ItemColors["MainHeal"];

            healStartSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("heal_start"), tankManager.MuzzlePoint.transform);
            healStartSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["MainHeal"];
            healStartSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            healStartSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            idleSparkSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("idle_spark"), tankManager.MuzzlePoint.transform);
            idleSparkSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["MainIdle"];
            idleSparkSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            idleSparkSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;

            damageShaftLine.GetComponent<LineRenderer>().enabled = false;
            damageShaftLine.GetComponent<LineRenderer>().useWorldSpace = false;

            damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
            healEndSprite.GetComponent<SpriteRenderer>().enabled = false;

            healShaftLine.GetComponent<LineRenderer>().enabled = false;
            healShaftLine.GetComponent<LineRenderer>().useWorldSpace = false;

            healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
            idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
            #endregion
            
        }

        IEnumerator LocalWaiter()
        {
            while(true)
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
                if (cacheTargetList == null)
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                if(Waited)
                {
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                    Waited = false;
                }
                if (CheckMuzzleVisible() == Vector3.zero)
                    targetList = cacheTargetList;
                foreach (var target in targetList)
                {
                    RaycastHit raycastHit;
                    Physics.Linecast(parentTankManager.MuzzlePoint.transform.position, target.Item1.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Friction"));
                    if (target.Item1.playerCommandEntityId != parentTankManager.playerCommandEntityId || parentTankManager.battleManager.NowBattleEntity.HasComponent(DMComponent.Id))
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            damageStartSprite.GetComponent<AnimationScript>().Play();
                            damageStartSprite.GetComponent<SpriteRenderer>().enabled = true;
                            damageStartSprite.transform.GetChild(0).gameObject.SetActive(true);

                            damageShaftLine.GetComponent<AnimationScript>().Play();
                            damageShaftLine.GetComponent<LineRenderer>().enabled = true;

                            damageEndSprite.GetComponent<AnimationScript>().Play();
                            damageEndSprite.GetComponent<SpriteRenderer>().enabled = true;
                            damageEndSprite.transform.GetChild(0).gameObject.SetActive(true);

                            var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                            if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                            {
                                weaponAudio.audioManager.StopAll();
                                weaponAudio.audioManager.ResetAll();
                                weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" });
                                //weaponAudio.
                                //weaponAudio.SetScheduledEndTime(AudioSettings. + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                            }
                                

                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                        damageShaftLine.GetComponent<LineRenderer>().SetPosition(1, damageShaftLine.transform.InverseTransformPoint(raycastHit.point));
                        damageShaftLine.GetComponent<LineLight>().SetLightToLine(parentTankManager.MuzzlePoint.transform.position, raycastHit.point, false);
                        damageEndSprite.transform.localPosition = parentTankManager.MuzzlePoint.transform.InverseTransformPoint(raycastHit.point);//use shaft because scale
                    }
                    else
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            healStartSprite.GetComponent<AnimationScript>().Play();
                            healStartSprite.GetComponent<SpriteRenderer>().enabled = true;
                            healStartSprite.transform.GetChild(0).gameObject.SetActive(true);

                            healShaftLine.GetComponent<AnimationScript>().Play();
                            healShaftLine.GetComponent<LineRenderer>().enabled = true;
                            
                            healEndSprite.GetComponent<AnimationScript>().Play();
                            healEndSprite.GetComponent<SpriteRenderer>().enabled = true;
                            healEndSprite.transform.GetChild(0).gameObject.SetActive(true);

                            var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                            if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                            {
                                weaponAudio.audioManager.StopAll();
                                weaponAudio.audioManager.ResetAll();
                                weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" }); //healing
                                //weaponAudio.SetScheduledEndTime(AudioSettings.dspTime + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                            }
                                

                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                        healShaftLine.GetComponent<LineRenderer>().SetPosition(1, healShaftLine.transform.InverseTransformPoint(raycastHit.point));
                        healShaftLine.GetComponent<LineLight>().SetLightToLine(parentTankManager.MuzzlePoint.transform.position, raycastHit.point, false);
                        healEndSprite.transform.localPosition = parentTankManager.MuzzlePoint.transform.InverseTransformPoint(raycastHit.point);
                    }
                }

                if(parentTankManager.ManagerEntity.HasComponent(WeaponTechCooldownComponent.Id))
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
                            if (entity.HasComponent<WeaponTechCooldownComponent>())
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
                        idleSparkSprite.GetComponent<AnimationScript>().Play();
                        idleSparkSprite.GetComponent<SpriteRenderer>().enabled = true;
                        idleSparkSprite.transform.GetChild(0).gameObject.SetActive(true);

                        var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                        if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                        {
                            weaponAudio.audioManager.StopAll();
                            weaponAudio.audioManager.ResetAll();
                            weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" }); //idle
                            //////weaponAudio.time = 0;
                            //////weaponAudio.clip = parentTankManager.TurretResources.GetElement<AudioClip>("damage_sound");//idle
                            //////weaponAudio.loop = true;
                            //////weaponAudio.Play();
                            //////weaponAudio.time = (weaponAudio.clip.length * 0.2f);
                            //weaponAudio.SetScheduledEndTime(AudioSettings.dspTime + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                        }
                            

                        parentTankManager.ManagerEntity.AddComponent(new StreamWeaponIdleComponent().SetGlobalComponentGroup());
                    }
                    if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                    {
                        parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponWorkingComponent.Id);
                        damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                        damageStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        damageStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                        damageShaftLine.GetComponent<LineRenderer>().enabled = false;
                        damageShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                        damageShaftLine.GetComponent<LineLight>().HideAllLights();

                        damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                        damageEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        damageEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                        healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                        healStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        healStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                        healShaftLine.GetComponent<LineRenderer>().enabled = false;
                        healShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                        healShaftLine.GetComponent<LineLight>().HideAllLights();

                        healEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                        healEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        healEndSprite.transform.GetChild(0).gameObject.SetActive(false);
                    }
                    
                }
                else
                {
                    if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                    {
                        idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
                        idleSparkSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        idleSparkSprite.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                pressed = true;
                return;
            }
            if(pressed)
            {
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponWorkingComponent>();
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponIdleComponent>();
                
                if(parentTankManager.ManagerEntity.HasComponent(WeaponTechCooldownComponent.Id))
                {
                    var weaponTechCooldown = parentTankManager.ManagerEntity.GetComponent<WeaponTechCooldownComponent>();
                    weaponTechCooldown.techObjects.Clear();
                }

                damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                damageStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                damageStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                damageShaftLine.GetComponent<LineRenderer>().enabled = false;
                damageShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                damageShaftLine.GetComponent<LineLight>().HideAllLights();

                damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                damageEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                damageEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                healStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                healStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                healShaftLine.GetComponent<LineRenderer>().enabled = false;
                healShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                healShaftLine.GetComponent<LineLight>().HideAllLights();

                healEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                healEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                healEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
                idleSparkSprite.GetComponent<AnimationScript>().StopAnimation = true;
                idleSparkSprite.transform.GetChild(0).gameObject.SetActive(false);

                weaponAudio.audioManager.FadeAll();
                //weaponAudio.audioManager.StopAll();
                //weaponAudio.audioManager.SoftStop("audio_damage_start");
            }
            pressed = false;
        }

        bool ShootControl = false;
        public override void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretInfluenceEvent(player, updatingEvent);
            if(updatingEvent.GetId() == StartShootingEvent.Id)
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
                List<(TankManager, Vector3)> targetList = new List<(TankManager, Vector3)>();
                if (cacheTargetList == null)
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                if (Waited)
                {
                    cacheTargetList = this.parentTankManager.closeDistanceAIM.FindPossibleTargets(parentTankManager);
                    Waited = false;
                }
                if(CheckMuzzleVisible() == Vector3.zero)
                    targetList = cacheTargetList;
                foreach (var target in targetList)
                {
                    RaycastHit raycastHit;
                    Physics.Linecast(parentTankManager.MuzzlePoint.transform.position, target.Item1.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Friction"));
                    if (target.Item1.playerCommandEntityId != parentTankManager.playerCommandEntityId || parentTankManager.battleManager.NowBattleEntity.HasComponent(DMComponent.Id))
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            damageStartSprite.GetComponent<AnimationScript>().Play();
                            damageStartSprite.GetComponent<SpriteRenderer>().enabled = true;
                            damageStartSprite.transform.GetChild(0).gameObject.SetActive(true);

                            damageShaftLine.GetComponent<AnimationScript>().Play();
                            damageShaftLine.GetComponent<LineRenderer>().enabled = true;

                            damageEndSprite.GetComponent<AnimationScript>().Play();
                            damageEndSprite.GetComponent<SpriteRenderer>().enabled = true;
                            damageEndSprite.transform.GetChild(0).gameObject.SetActive(true);


                            var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                            if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                            {
                                weaponAudio.audioManager.StopAll();
                                weaponAudio.audioManager.ResetAll();
                                weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" });
                                //weaponAudio.SetScheduledEndTime(AudioSettings.dspTime + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                            }

                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                        damageShaftLine.GetComponent<LineRenderer>().SetPosition(1, damageShaftLine.transform.InverseTransformPoint(raycastHit.point));
                        damageShaftLine.GetComponent<LineLight>().SetLightToLine(parentTankManager.MuzzlePoint.transform.position, raycastHit.point, false);
                        damageEndSprite.transform.localPosition = parentTankManager.MuzzlePoint.transform.InverseTransformPoint(raycastHit.point);//use shaft because scale
                    }
                    else
                    {
                        if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                        {
                            healStartSprite.GetComponent<AnimationScript>().Play();
                            healStartSprite.GetComponent<SpriteRenderer>().enabled = true;
                            healStartSprite.transform.GetChild(0).gameObject.SetActive(true);

                            healShaftLine.GetComponent<AnimationScript>().Play();
                            healShaftLine.GetComponent<LineRenderer>().enabled = true;

                            healEndSprite.GetComponent<AnimationScript>().Play();
                            healEndSprite.GetComponent<SpriteRenderer>().enabled = true;
                            healEndSprite.transform.GetChild(0).gameObject.SetActive(true);

                            var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                            if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                            {
                                weaponAudio.audioManager.StopAll();
                                weaponAudio.audioManager.ResetAll();
                                weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" });
                                //weaponAudio.SetScheduledEndTime(AudioSettings.dspTime + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                            }
                                

                            parentTankManager.ManagerEntity.AddComponent(new StreamWeaponWorkingComponent().SetGlobalComponentGroup());
                        }
                        if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                        {
                            parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponIdleComponent.Id);
                        }
                        healShaftLine.GetComponent<LineRenderer>().SetPosition(1, healShaftLine.transform.InverseTransformPoint(raycastHit.point));
                        healShaftLine.GetComponent<LineLight>().SetLightToLine(parentTankManager.MuzzlePoint.transform.position, raycastHit.point, false);
                        healEndSprite.transform.localPosition = parentTankManager.MuzzlePoint.transform.InverseTransformPoint(raycastHit.point);
                    }
                }

                if (targetList.Count == 0)
                {
                    if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                    {
                        idleSparkSprite.GetComponent<AnimationScript>().Play();
                        idleSparkSprite.GetComponent<SpriteRenderer>().enabled = true;
                        idleSparkSprite.transform.GetChild(0).gameObject.SetActive(true);

                        var playedAudio = weaponAudio.audioManager.GetNowPlayingAudioName();
                        if (playedAudio.Length == 0 || (playedAudio.Length > 0 && playedAudio[0].IndexOf("damage") == -1))
                        {
                            weaponAudio.audioManager.StopAll();
                            weaponAudio.audioManager.ResetAll();
                            weaponAudio.audioManager.PlayBlock(new List<string> { "audio_damage_start", "audio_damage_loop" });
                            //weaponAudio.SetScheduledEndTime(AudioSettings.dspTime + weaponAudio.clip.length - (weaponAudio.clip.length * 0.1f));
                        }
                        

                        parentTankManager.ManagerEntity.AddComponent(new StreamWeaponIdleComponent().SetGlobalComponentGroup());
                    }
                    if (parentTankManager.ManagerEntity.HasComponent(StreamWeaponWorkingComponent.Id))
                    {
                        parentTankManager.ManagerEntity.RemoveComponent(StreamWeaponWorkingComponent.Id);
                        damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                        damageStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        damageStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                        damageShaftLine.GetComponent<LineRenderer>().enabled = false;
                        damageShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                        damageShaftLine.GetComponent<LineLight>().HideAllLights();

                        damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                        damageEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        damageEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                        healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                        healStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        healStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                        healShaftLine.GetComponent<LineRenderer>().enabled = false;
                        healShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                        healShaftLine.GetComponent<LineLight>().HideAllLights();

                        healEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                        healEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        healEndSprite.transform.GetChild(0).gameObject.SetActive(false);
                    }

                }
                else
                {
                    if (!parentTankManager.ManagerEntity.HasComponent(StreamWeaponIdleComponent.Id))
                    {
                        idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
                        idleSparkSprite.GetComponent<AnimationScript>().StopAnimation = true;
                        idleSparkSprite.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                pressed = true;
                return;
            }
            if (pressed)
            {
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponWorkingComponent>();
                parentTankManager.ManagerEntity.RemoveComponentIfPresent<StreamWeaponIdleComponent>();

                damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                damageStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                damageStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                damageShaftLine.GetComponent<LineRenderer>().enabled = false;
                damageShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                damageShaftLine.GetComponent<LineLight>().HideAllLights();

                damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                damageEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                damageEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
                healStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
                healStartSprite.transform.GetChild(0).gameObject.SetActive(false);

                healShaftLine.GetComponent<LineRenderer>().enabled = false;
                healShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
                healShaftLine.GetComponent<LineLight>().HideAllLights();

                healEndSprite.GetComponent<SpriteRenderer>().enabled = false;
                healEndSprite.GetComponent<AnimationScript>().StopAnimation = true;
                healEndSprite.transform.GetChild(0).gameObject.SetActive(false);

                idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
                idleSparkSprite.GetComponent<AnimationScript>().StopAnimation = true;
                idleSparkSprite.transform.GetChild(0).gameObject.SetActive(false);


                //weaponAudio.audioManager.StopAll();
                //weaponAudio.audioManager.SoftStopAll();
                weaponAudio.audioManager.FadeAll();

                //weaponAudio.loop = false;
                ////weaponAudio.Stop();
                //var weaponEndStart = weaponAudio.clip.length - (weaponAudio.clip.length * 0.05f);
                //weaponAudio.Play();
                //weaponAudio.time = (weaponEndStart);

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

            damageStartSprite.GetComponent<SpriteRenderer>().enabled = false;
            damageStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
            damageShaftLine.GetComponent<LineRenderer>().enabled = false;
            damageShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
            damageEndSprite.GetComponent<SpriteRenderer>().enabled = false;
            damageEndSprite.GetComponent<AnimationScript>().StopAnimation = true;

            healStartSprite.GetComponent<SpriteRenderer>().enabled = false;
            healStartSprite.GetComponent<AnimationScript>().StopAnimation = true;
            healShaftLine.GetComponent<LineRenderer>().enabled = false;
            healShaftLine.GetComponent<AnimationScript>().StopAnimation = true;
            healEndSprite.GetComponent<SpriteRenderer>().enabled = false;
            healEndSprite.GetComponent<AnimationScript>().StopAnimation = true;

            idleSparkSprite.GetComponent<SpriteRenderer>().enabled = false;
            idleSparkSprite.GetComponent<AnimationScript>().StopAnimation = true;
            pressed = false;
        }

        public override ITurret AppendManagerToObject(GameObject turretObject)
        {
            return turretObject.AddComponent<IsisManager>();
        }
    }
}