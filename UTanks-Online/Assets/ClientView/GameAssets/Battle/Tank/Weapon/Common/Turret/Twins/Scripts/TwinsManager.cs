using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.Effects;
using SecuredSpace.Important.Aim;
using SecuredSpace.UnityExtend;
using SecuredSpace.UnityExtend.MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class TwinsManager : ITurret
    {
        private bool Waited = false;
        public float verticalAngle;
        public float projectileSpeed = 1;
        public float projectileMaxDistance = 20;
        public GameObject chargeSprite;
        public GameObject railgunLine;
        public TwinsDamageComponent twinsComponent;
        private string projectilePoolName;
        public void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine("LocalWaiter");
        }
        public override void Initialize(TankManager tankManager, ECSEntity player)
        {
            base.Initialize(tankManager, player);
            var raycastAim = tankManager.Turret.GetComponent<RaycastAIM>();
            var twinsComponent = player.GetComponent<TwinsDamageComponent>();
            raycastAim.VerticalUpAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalTargeting"]["angleUp"].ToString(), CultureInfo.InvariantCulture);
            raycastAim.VerticalDownAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalTargeting"]["angleDown"].ToString(), CultureInfo.InvariantCulture);
            projectileSpeed = twinsComponent.bulletSpeedProperty;
            //projectileMaxDistance = twinsComponent.;
            //raycastAim.MultitargetsWeapon = true;
            projectilePoolName = player.instanceId.ToString();
            EasyObjectPool.instance.AddPool(new PoolInfo
            {
                fixedSize = false,
                poolName = projectilePoolName,
                poolSize = 30,
                prefab = tankManager.TurretResources.GetElement<GameObject>("TwinsProjectile")
            });
            weaponAudio.audioStorage.Add(tankManager.TurretResources.GetElement<AudioSourceSetting>("plazma_shot").UpdateSettings("plazma_shot", false, 1f));
            weaponAudio.Build();
            //chargeSprite = Instantiate(tankManager.TurretResources.ScriptAnimations["charge"], tankManager.MuzzlePoint.transform);
            //railgunLine = Instantiate(tankManager.TurretResources.ScriptAnimations["railgun"], tankManager.MuzzlePoint.transform);

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
                if ((playerEntity.GetComponent<WeaponEnergyComponent>().MaxEnergy == playerEntity.GetComponent<WeaponEnergyComponent>().Energy) && !playerEntity.HasComponent(WeaponTechCooldownComponent.Id))
                {
                    //ULogger.Log(playerEntity.GetComponent<WeaponEnergyComponent>().Energy);
                    NextMuzzlePoint();
                    Vector3 pointOfExplosive = CheckMuzzleVisible();
                    bool checkMuzzle = pointOfExplosive == Vector3.zero;
                    var projectDirection = parentTankManager.MuzzlePoint.transform.forward * -1;

                    bool sended = false;
                    if (checkMuzzle)
                    {
                        var list = this.parentTankManager.raycastAIM.FindPossibleTargets(parentTankManager);
                        if (list.Count > 0)
                        {
                            
                            ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                            {
                                hitDistanceList = new Dictionary<long, float>(),
                                hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>(),
                                hitLocalDistanceList = new Dictionary<long, float>() { },
                                MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((list[0].Item2 - parentTankManager.MuzzlePoint.transform.position).normalized),
                                StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                                StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                            });
                            sended = true;
                            //pointOfExplosive = list.Where((x) => x.Item1 == null).ToList()[0].Item2;
                            projectDirection = (list[0].Item2 - parentTankManager.MuzzlePoint.transform.position).normalized;
                        }
                        else
                        {
                            //var hits = Physics.RaycastAll(parentTankManager.MuzzlePoint.transform.position, parentTankManager.MuzzlePoint.transform.forward * -1, 549263.7f, this.parentTankManager.raycastAIM.layerMask, QueryTriggerInteraction.Ignore).ToList().OrderBy(x => x.distance).ToList();
                            //foreach (var hit in hits)
                            //{
                            //    if (hit.collider != null)
                            //    {
                            //        if (hit.collider.gameObject.tag == "Ground")
                            //        {
                            //            ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                            //            {
                            //                hitDistanceList = new Dictionary<long, float>(),
                            //                hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>(),
                            //                hitLocalDistanceList = new Dictionary<long, float>() { },
                            //                MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((hit.point - parentTankManager.MuzzlePoint.transform.position).normalized),
                            //                StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                            //                StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                            //            });
                            //            pointOfExplosive = hit.point;
                            //            sended = true;
                            //            break;
                            //        }
                            //    }
                            //    else
                            //    {

                            //    }
                            //}

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
                            MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.forward * -1),
                            StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                            StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                        });
                    }

                    var twinsDamage = playerEntity.GetComponent<TwinsDamageComponent>();

                    playerEntity.AddComponent(new WeaponTechCooldownComponent(0.2f).SetGlobalComponentGroup());
                    if (pointOfExplosive != Vector3.zero)
                    {
                        //this.railgunLine.SetActive(true);
                        //this.railgunLine.GetComponent<LineRenderer>().SetPosition(0, parentTankManager.MuzzlePoint.transform.position);
                        //this.railgunLine.GetComponent<LineRenderer>().SetPosition(1, pointOfExplosive);
                        //this.railgunLine.GetComponent<AnimationScript>().Play();
                        weaponAudio.audioManager.PlayBlock(new List<string> { "plazma_shot" });
                    }
                    else
                    {
                        var projectile = EasyObjectPool.instance.GetObjectFromPool(projectilePoolName, parentTankManager.MuzzlePoint.transform.position, Quaternion.identity);
                        var projectileScript = projectile.GetComponent<PhysicalProjectile>();
                        projectileScript.Direction = projectDirection;
                        projectileScript.StartPosition = parentTankManager.MuzzlePoint.transform.position;
                        projectileScript.MaxDistance = this.projectileMaxDistance;
                        projectileScript.Speed = this.projectileSpeed;
                        projectileScript.Influence = twinsDamage.impactProperty * Const.impactScaler;
                        projectileScript.Init();
                        projectileScript.bulletAnimations["charge"].gameObject.SetActive(true);
                        projectileScript.bulletAnimations["charge"].Play();
                        projectileScript.OnDistanceExcess = (projectileObj) =>
                        {
                            var projScript = projectileObj.GetComponent<PhysicalProjectile>();
                            projScript.bulletAnimations.ForEach((x) => x.Value.gameObject.SetActive(false));
                            projScript.bulletAnimations["dissolution"].gameObject.SetActive(true);
                            projScript.bulletAnimations["dissolution"].Play();
                            projScript.bulletAnimations["dissolution"].afterPlayAction = (projectObj) =>
                            {
                                EasyObjectPool.instance.ReturnObjectToPool(projScript.gameObject);
                                projScript.bulletAnimations["dissolution"].gameObject.SetActive(false);
                            };
                        };
                        projectileScript.OnTriggerEnterAction = (projScript, collider) =>
                        {
                            if (!collider.isTrigger && collider.tag != "PhysicalProjectile" && LayerMaskEx.PresentedInLayerMask(LayerMask.GetMask("Tank", "TankBounds", "Default", "Bounds"), collider.gameObject.layer))
                            {
                                projScript.bulletAnimations.ForEach((x) => x.Value.gameObject.SetActive(false));
                                projScript.bulletAnimations["explosion"].gameObject.SetActive(true);
                                projScript.bulletAnimations["explosion"].Play();
                                projScript.bulletAnimations["explosion"].afterPlayAction = (projectObj) =>
                                {
                                    EasyObjectPool.instance.ReturnObjectToPool(projScript.gameObject);
                                    projScript.bulletAnimations["explosion"].gameObject.SetActive(false);
                                };
                                var physProjScript = projScript as PhysicalProjectile;
                                physProjScript.thisConstantForce.force = Vector3.zero;
                                physProjScript.thisConstantForce.relativeForce = Vector3.zero;
                                physProjScript.thisRigidbody.velocity = Vector3.zero;
                                TankManager tankManager = null;
                                try { tankManager = collider.GetComponent<IManagableAnchor>().ownerManager<TankManager>(); } catch {
                                    try { tankManager = collider.GetComponent<TankManager>(); } catch { }
                                }
                                
                                if (tankManager != null && !physProjScript.hitedTanks.ContainsKey(tankManager))
                                {
                                    tankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(projScript.Direction * projScript.Influence, projScript.transform.position);
                                    Dictionary<long, Vector3S> hitList = new Dictionary<long, Vector3S>();
                                    Dictionary<long, float> HitDistanceList = new Dictionary<long, float>();
                                    hitList.Add(tankManager.ManagerEntityId, new Vector3S(projScript.transform.position));
                                    HitDistanceList.Add(tankManager.ManagerEntityId, 0f);
                                    physProjScript.hitedTanks.Add(tankManager, 0);
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
                            }
                        };

                        projectileScript.projectileLightColor = parentTankManager.TurretResources.ItemColors["Main"];       
                        projectileScript.UpdateStatements();
                        projectileScript.projectileLight.LightSwitch(ClientInitService.instance.gameSettings.WeaponIllumination);
                        weaponAudio.audioManager.ResetAll();
                        weaponAudio.audioManager.PlayBlock(new List<string> { "plazma_shot" });

                        parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * twinsDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);

                    }
                    //parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * 150000f, this.parentTankManager.MuzzlePoint.transform.position);
                }
            }
        }

        bool ShootControl = false;
        public override void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretInfluenceEvent(player, updatingEvent);
            if (updatingEvent.GetId() == ShotEvent.Id)
            {
                try
                {
                    var shotEvent = updatingEvent as ShotEvent;

                    Vector3 pointOfExplosive = CheckMuzzleVisible();
                    bool checkMuzzle = pointOfExplosive == Vector3.zero;

                    var twinsDamage = player.GetComponent<TwinsDamageComponent>();

                    if (checkMuzzle)
                    {
                        var projectile = EasyObjectPool.instance.GetObjectFromPool(projectilePoolName, shotEvent.StartGlobalPosition.ConvertToUnityVector3(), Quaternion.identity);
                        var projectileScript = projectile.GetComponent<PhysicalProjectile>();
                        projectileScript.Direction = shotEvent.MoveDirectionNormalized.ConvertToUnityVector3();
                        projectileScript.StartPosition = shotEvent.StartGlobalPosition.ConvertToUnityVector3();
                        projectileScript.MaxDistance = this.projectileMaxDistance;
                        projectileScript.Speed = this.projectileSpeed;
                        projectileScript.Influence = twinsDamage.impactProperty * Const.impactScaler;
                        projectileScript.Init();
                        projectileScript.bulletAnimations["charge"].gameObject.SetActive(true);
                        projectileScript.bulletAnimations["charge"].Play();
                        projectileScript.OnDistanceExcess = (projectileObj) =>
                        {
                            var projScript = projectileObj.GetComponent<PhysicalProjectile>();
                            projScript.bulletAnimations.ForEach((x) => x.Value.gameObject.SetActive(false));
                            projScript.bulletAnimations["dissolution"].gameObject.SetActive(true);
                            projScript.bulletAnimations["dissolution"].Play();
                            projScript.bulletAnimations["dissolution"].afterPlayAction = (projectObj) =>
                            {
                                EasyObjectPool.instance.ReturnObjectToPool(projScript.gameObject);
                                projScript.bulletAnimations["dissolution"].gameObject.SetActive(false);
                            };
                        };
                        projectileScript.OnTriggerEnterAction = (projScript, collider) =>
                        {
                            if (!collider.isTrigger && collider.tag != "PhysicalProjectile" && LayerMaskEx.PresentedInLayerMask(LayerMask.GetMask("Tank", "TankBounds", "Default", "Bounds"), collider.gameObject.layer))
                            {
                                projScript.bulletAnimations.ForEach((x) => x.Value.gameObject.SetActive(false));
                                projScript.bulletAnimations["explosion"].gameObject.SetActive(true);
                                projScript.bulletAnimations["explosion"].Play();
                                projScript.bulletAnimations["explosion"].afterPlayAction = (projectObj) =>
                                {
                                    EasyObjectPool.instance.ReturnObjectToPool(projScript.gameObject);
                                    projScript.bulletAnimations["explosion"].gameObject.SetActive(false);
                                };
                                var physProjScript = projScript as PhysicalProjectile;
                                physProjScript.thisConstantForce.force = Vector3.zero;
                                physProjScript.thisConstantForce.relativeForce = Vector3.zero;
                                physProjScript.thisRigidbody.velocity = Vector3.zero;

                                TankManager tankManager = null;
                                try { tankManager = collider.GetComponent<IManagableAnchor>().ownerManager<TankManager>(); }
                                catch
                                {
                                    try { tankManager = collider.GetComponent<TankManager>(); } catch { }
                                }

                                if (tankManager != null && !physProjScript.hitedTanks.ContainsKey(tankManager))
                                {
                                    tankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(projScript.Direction * projScript.Influence, projScript.transform.position);
                                }
                            }
                        };
                        projectileScript.projectileLightColor = parentTankManager.TurretResources.ItemColors["Main"];
                        projectileScript.UpdateStatements();
                        projectileScript.projectileLight.LightSwitch(ClientInitService.instance.gameSettings.WeaponIllumination);

                        parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * twinsDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);

                        weaponAudio.audioManager.ResetAll();
                        weaponAudio.audioManager.PlayBlock(new List<string> { "plazma_shot" });
                    }
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

        public override void RemoveTurret(ECSEntity player)
        {
            base.RemoveTurret(player);
            EasyObjectPool.instance.RemovePool(projectilePoolName);
        }

        public override ITurret AppendManagerToObject(GameObject turretObject)
        {
            return turretObject.AddComponent<TwinsManager>();
        }
    }
}
