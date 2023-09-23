using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.Important.Aim;
using SecuredSpace.Settings;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class RailgunManager : ITurret
    {
        private bool Waited = false;
        public float verticalAngle;
        public GameObject chargeSprite;
        public GameObject railgunLine;
        public RailgunDamageComponent railgunComponent;
        public void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine("LocalWaiter");
        }
        public override void Initialize(TankManager tankManager, ECSEntity player)
        {
            base.Initialize(tankManager, player);
            var raycastAim = tankManager.Turret.GetComponent<RaycastAIM>();
            var railgunComponent = player.GetComponent<RailgunDamageComponent>();
            raycastAim.VerticalUpAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalSectorsTargeting"]["vAngleUp"].ToString(), CultureInfo.InvariantCulture);
            raycastAim.VerticalDownAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalSectorsTargeting"]["vAngleDown"].ToString(), CultureInfo.InvariantCulture);
            raycastAim.MultitargetsWeapon = true;
            chargeSprite = Instantiate(tankManager.TurretResources.GetElement<GameObject>("charge_anim"), tankManager.MuzzlePoint.transform);
            chargeSprite.transform.GetChild(0).GetComponent<PointLight>().lightColor = tankManager.TurretResources.ItemColors["Main"];
            chargeSprite.transform.GetChild(0).GetComponent<PointLight>().UpdateLight();
            chargeSprite.transform.GetChild(0).GetComponent<IAnchor>().DirectRegisterObject();

            weaponAudio.audioStorage.Add(tankManager.TurretResources.GetElement<AudioSourceSetting>("railgun_sound").UpdateSettings("railgun_sound", false, 1f));
            weaponAudio.Build();

            railgunLine = Instantiate(tankManager.TurretResources.GetElement<GameObject>("railgun_anim"), tankManager.MuzzlePoint.transform);
            railgunLine.GetComponent<LineLight>().LightColor = tankManager.TurretResources.ItemColors["Main"];
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
            if (!pressed && ShootTurn > 0 && parentTankManager.hullManager.chassisManager.TankMovable && parentTankManager.TestingMode)
            {
                var list = this.parentTankManager.raycastAIM.FindPossibleTargets(parentTankManager);
                pressed = true;
                return;
            }
            pressed = false;
            //return;
            if (ShootTurn > 0 && (!parentTankManager.Ghost && !parentTankManager.checkGhost))
            {
                var playerEntity = parentTankManager.ManagerEntity;
                //(!playerEntity.HasComponent(WeaponCooldownComponent.Id) ||
                if ((playerEntity.GetComponent<WeaponEnergyComponent>().MaxEnergy == playerEntity.GetComponent<WeaponEnergyComponent>().Energy) && !playerEntity.HasComponent(WeaponTechCooldownComponent.Id) && !playerEntity.HasComponent(RailgunChargingWeaponComponent.Id))
                {
                    parentTankManager.ManagerEntity.AddComponent(new RailgunChargingWeaponComponent(parentTankManager.ManagerEntity.GetComponent<RailgunDamageComponent>().chargeTimeProperty).SetGlobalComponentGroup());
                    ClientNetworkService.instance.Socket.emit<RawStartShootingEvent>(new RawStartShootingEvent());
                    var chargeAnim = chargeSprite.GetComponent<AnimationScript>();
                    chargeSprite.GetComponent<SpriteRenderer>().enabled = true;
                    playerEntity.AddComponent(new WeaponTechCooldownComponent(2.5f).SetGlobalComponentGroup());
                    chargeSprite.transform.GetChild(0).gameObject.SetActive(true);
                    chargeAnim.afterPlayAction = (go) => go.transform.GetChild(0).gameObject.SetActive(false);
                    chargeAnim.Play();
                    weaponAudio.audioManager.PlayBlock(new List<string> { "railgun_sound" });
                }
            }
        }

        public void Shot()
        {
            var playerEntity = ManagerScope.entityManager.EntityStorage[parentTankManager.ManagerEntityId];
            //(!playerEntity.HasComponent(WeaponCooldownComponent.Id) ||
            if ((playerEntity.GetComponent<WeaponEnergyComponent>().MaxEnergy == playerEntity.GetComponent<WeaponEnergyComponent>().Energy))
            {

                //ULogger.Log(playerEntity.GetComponent<WeaponEnergyComponent>().Energy);
                var railgunDamage = playerEntity.GetComponent<RailgunDamageComponent>();
                Vector3 pointOfExplosive = CheckMuzzleVisible();
                bool checkMuzzle = pointOfExplosive == Vector3.zero;
       
                bool sended = false;
                if(checkMuzzle)
                {
                    var list = this.parentTankManager.raycastAIM.FindPossibleTargets(parentTankManager);
                    if (list.Count > 0)
                    {
                        var xhitDistanceList = new Dictionary<long, float>();
                        var xhitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>();
                        list.ForEach((x) => {
                            if (x.Item1 != null)
                            {
                                xhitDistanceList.Add(x.Item1.ManagerEntityId, 0f);
                                xhitList.Add(x.Item1.ManagerEntityId, new UTanksClient.ECS.Types.Battle.Vector3S(x.Item2));
                                x.Item1.Hull.GetComponent<Rigidbody>().AddForceAtPosition((x.Item2 - parentTankManager.MuzzlePoint.transform.position).normalized * railgunDamage.impactProperty * Const.impactScaler, x.Item2);
                            }
                            
                        });
                        ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                        {
                            hitDistanceList = xhitDistanceList,
                            hitList = xhitList,
                            hitLocalDistanceList = new Dictionary<long, float>() { },
                            MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((list[0].Item2 - parentTankManager.MuzzlePoint.transform.position).normalized),
                            StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                            StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                        });
                        sended = true;
                        pointOfExplosive = list.Where((x) => x.Item1 == null).ToList()[0].Item2;
                    }
                    else
                    {
                        var hits = Physics.RaycastAll(parentTankManager.MuzzlePoint.transform.position, parentTankManager.MuzzlePoint.transform.forward * -1, 549263.7f, this.parentTankManager.raycastAIM.layerMask, QueryTriggerInteraction.Ignore).ToList().OrderBy(x => x.distance).ToList();
                        foreach (var hit in hits)
                        {
                            if (hit.collider != null)
                            {
                                if (hit.collider.gameObject.tag == "Ground")
                                {
                                    ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                                    {
                                        hitDistanceList = new Dictionary<long, float>() {
                                    {-1, hit.distance }
                                },
                                        hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { -1, new UTanksClient.ECS.Types.Battle.Vector3S(hit.point) } },
                                        hitLocalDistanceList = new Dictionary<long, float>() { },
                                        MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((hit.point - parentTankManager.MuzzlePoint.transform.position).normalized),
                                        StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                                        StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                                    });
                                    pointOfExplosive = hit.point;
                                    sended = true;
                                    break;
                                }
                            }
                            else
                            {
                                //ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                                //{
                                //    hitDistanceList = new Dictionary<long, float>() {
                                //        {-1, hit.distance }
                                //    },
                                //    hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { -1, new UTanksClient.ECS.Types.Battle.Vector3S(hit.point) } },
                                //    hitLocalDistanceList = new Dictionary<long, float>() { },
                                //    MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((hit.point - parentTankManager.MuzzlePoint.transform.position).normalized),
                                //    StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                                //    StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                                //});
                                //pointOfExplosive = hit.point;
                                //sended = true;
                            }
                        }

                    }
                }
                else
                {
                    ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                    {
                        hitDistanceList = new Dictionary<long, float>() {
                                    {-1, Vector3.Distance(parentTankManager.MuzzlePoint.transform.position, pointOfExplosive) }
                                },
                        hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { -1, new UTanksClient.ECS.Types.Battle.Vector3S(pointOfExplosive) } },
                        hitLocalDistanceList = new Dictionary<long, float>() { },
                        MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((pointOfExplosive - parentTankManager.MuzzlePoint.transform.position).normalized),
                        StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                        StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                    });
                    sended = true;
                }
                

                if (!sended)
                {
                    ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                    {
                        hitDistanceList = new Dictionary<long, float>() {
                                {-1, Const.MaxUnityFloatValue }
                            },
                        hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { -1, new UTanksClient.ECS.Types.Battle.Vector3S(Const.MaxUnityFloatValue, Const.MaxUnityFloatValue, Const.MaxUnityFloatValue) } },
                        hitLocalDistanceList = new Dictionary<long, float>() { },
                        MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S(),
                        StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(),
                        StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                    });
                }

                playerEntity.AddComponent(new WeaponTechCooldownComponent(0.4f).SetGlobalComponentGroup());
                if (pointOfExplosive != Vector3.zero)
                {
                    this.railgunLine.SetActive(true);
                    if (ClientInitService.instance.gameSettings.WeaponIllumination)
                    {
                        var lightLine = railgunLine.GetComponent<LineLight>();
                        lightLine.SetLightToLine(parentTankManager.MuzzlePoint.transform.position, pointOfExplosive);
                    }
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(0, parentTankManager.MuzzlePoint.transform.position);
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(1, pointOfExplosive);
                    this.railgunLine.GetComponent<AnimationScript>().Play();
                }
                else
                {
                    this.railgunLine.SetActive(true);
                    if(ClientInitService.instance.gameSettings.WeaponIllumination)
                    {
                        var lightLine = railgunLine.GetComponent<LineLight>();
                        lightLine.SetLightToLine(parentTankManager.MuzzlePoint.transform.position, parentTankManager.MuzzlePoint.transform.position + parentTankManager.MuzzlePoint.transform.forward * 549263.7f * -1f);
                    }
                    
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(0, parentTankManager.MuzzlePoint.transform.position);
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(1, parentTankManager.MuzzlePoint.transform.position + parentTankManager.MuzzlePoint.transform.forward * 549263.7f * -1f);
                    this.railgunLine.GetComponent<AnimationScript>().Play();
                }
                parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * railgunDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);
                //this.shotSprite.GetComponent<AnimationScript>().Play();
            }
        }

        bool ShootControl = false;
        public override void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretInfluenceEvent(player, updatingEvent);
            if (updatingEvent.GetId() == StartShootingEvent.Id)
            {
                try
                {
                    this.chargeSprite.SetActive(true);
                    this.chargeSprite.GetComponent<SpriteRenderer>().enabled = true;
                    var chargeAnim = this.chargeSprite.GetComponent<AnimationScript>();
                    chargeSprite.transform.GetChild(0).gameObject.SetActive(true);
                    chargeAnim.afterPlayAction = (go) => go.transform.GetChild(0).gameObject.SetActive(false);
                    chargeAnim.Play();
                    weaponAudio.audioManager.PlayBlock(new List<string> { "railgun_sound" });
                }
                catch
                {
                    ULogger.Log("Error shot");
                }
            }
            if (updatingEvent.GetId() == ShotEvent.Id)
            {
                try
                {
                    var shotEvent = updatingEvent as ShotEvent;

                    Vector3 pointOfExplosive = CheckMuzzleVisible();
                    bool checkMuzzle = pointOfExplosive == Vector3.zero;
                    var railgunDamage = player.GetComponent<RailgunDamageComponent>();

                    if (checkMuzzle)
                    {
                        var hits = Physics.RaycastAll(parentTankManager.MuzzlePoint.transform.position, shotEvent.MoveDirectionNormalized.ConvertToUnityVector3(), 549263.7f, this.parentTankManager.raycastAIM.layerMask, QueryTriggerInteraction.Ignore).ToList().OrderBy(x => x.distance).ToList();
                        foreach (var hit in hits)
                        {
                            if (hit.collider != null)
                            {
                                if (hit.collider.gameObject.tag == "Ground")
                                {
                                    pointOfExplosive = hit.point;
                                    break;
                                }
                            }
                        }
                        if (hits.Count == 0)
                        {
                            pointOfExplosive = parentTankManager.MuzzlePoint.transform.position + parentTankManager.MuzzlePoint.transform.forward * 549263.7f * -1f;
                        }
                        foreach (var tankinfl in shotEvent.hitList)
                        {
                            if (tankinfl.Key == -1)
                                continue;
                            var tankInfluence = ManagerScope.entityManager.EntityStorage[tankinfl.Key].GetComponent<EntityManagersComponent>().Get<TankManager>();
                            tankInfluence.Hull.GetComponent<Rigidbody>().AddForceAtPosition((shotEvent.hitList[tankinfl.Key].ConvertToUnityVector3() - parentTankManager.MuzzlePoint.transform.position).normalized * railgunDamage.impactProperty * Const.impactScaler, shotEvent.hitList[tankinfl.Key].ConvertToUnityVector3());
                        }
                    }
                    
                    this.railgunLine.SetActive(true);
                    if (ClientInitService.instance.gameSettings.WeaponIllumination)
                    {
                        var lightLine = railgunLine.GetComponent<LineLight>();
                        lightLine.SetLightToLine(parentTankManager.MuzzlePoint.transform.position, pointOfExplosive);
                    }
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(0, parentTankManager.MuzzlePoint.transform.position);
                    this.railgunLine.GetComponent<LineRenderer>().SetPosition(1, pointOfExplosive);
                    this.railgunLine.GetComponent<AnimationScript>().Play();
                    parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * railgunDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);
                }
                catch
                {
                    ULogger.Log("Error shot");
                }
            }
        }

        public override void FixedUpd()
        {
            base.FixedUpd();
            
        }

        public override void DestroyTurret(ECSEntity player)
        {
            base.DestroyTurret(player);

        }
        public override ITurret AppendManagerToObject(GameObject turretObject)
        {
            return turretObject.AddComponent<RailgunManager>();
        }
    }
}
