using Assets.ClientCore.CoreImpl.ECS.ClientComponents.Battle;
using SecuredSpace.AudioManager.Settings;
using SecuredSpace.ClientControl.Services;
using SecuredSpace.Effects;
using SecuredSpace.Important.Aim;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Energy;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Battle.TankEvents.Shooting;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class SmokyManager : ITurret
    {
        public float verticalAngle;
        public GameObject shotSprite;
        public GameObject explosiveSprite;
        public override void Initialize(TankManager tankManager, ECSEntity player)
        {
            base.Initialize(tankManager, player);
            var raycastAim = tankManager.Turret.GetComponent<RaycastAIM>();
            var weaponComponent = player.GetComponent<SmokyDamageComponent>();
            raycastAim.VerticalUpAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalSectorsTargeting"]["vAngleUp"].ToString(), CultureInfo.InvariantCulture);
            raycastAim.VerticalDownAngle = float.Parse(tankManager.turretGameplayObj.Deserialized["verticalSectorsTargeting"]["vAngleDown"].ToString(), CultureInfo.InvariantCulture);
            shotSprite = Instantiate(ResourcesService.instance.GetPrefab("billboardSprite"), tankManager.Turret.transform);
            tankManager.turretVisualController.ChildTemp.Add(shotSprite);
            shotSprite.transform.position = tankManager.MuzzlePoint.transform.position;
            float distanceToCenter = Vector3.Distance(shotSprite.transform.position, tankManager.Turret.transform.position);
            shotSprite.transform.position += (tankManager.Turret.transform.position - shotSprite.transform.position).normalized * distanceToCenter * 0.1f;
            shotSprite.transform.forward = tankManager.MuzzlePoint.transform.forward * -1;
            explosiveSprite = Instantiate(ResourcesService.instance.GetPrefab("billboardSprite"), tankManager.Turret.transform);
            tankManager.turretVisualController.ChildTemp.Add(explosiveSprite);
            //shotSprite.SetActive(false);
            var shotSpriteBillboard = shotSprite.GetComponent<CameraBillboard>();
            shotSpriteBillboard.freezeZ = true;
            shotSpriteBillboard.freezeX = true;
            shotSpriteBillboard.eulerRotation = Quaternion.Euler(270, 0, 0).eulerAngles;
            var textureShot = tankManager.TurretResources.GetElement<Texture2D>("shot");
            shotSprite.GetComponent<AnimationScript>().frames.Add(SpriteExtensions.Duplicate(Sprite.Create(textureShot, new Rect(0, 0, textureShot.width, textureShot.height), new Vector2()), SpriteExtensions.TransformType.Rotate90Clockwise));
            shotSprite.GetComponent<AnimationScript>().loop = false;
            shotSprite.GetComponent<AnimationScript>().noneSpriteAfterPlay = true;
            explosiveSprite.GetComponent<AnimationScript>().SpriteScaler = 2f;
            shotSprite.GetComponent<AnimationScript>().speed = float.Parse(tankManager.turretGameplayObj.Deserialized["shotAnimationTime"]["shotAnimationTime"].ToString(), CultureInfo.InvariantCulture);
            explosiveSprite.SetActive(false);
            //explosiveSprite.transform.localScale = new Vector3(2, 2, 2);

            tankManager.TurretResources.GetElement<GameObject>("shot_anim").GetComponent<AnimationScript>().CopyToAnimation(explosiveSprite.GetComponent<AnimationScript>());

            weaponAudio.audioStorage.Add(tankManager.TurretResources.GetElement<AudioSourceSetting>("smoky_shot").UpdateSettings("smoky_shot", false, 1f));
            weaponAudio.Build();
        }

        public override void TurretEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretEvent(player, updatingEvent);
        }

        public override void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {
            base.TurretInfluenceEvent(player, updatingEvent);
            if (updatingEvent.GetId() == ShotEvent.Id)
            {
                try
                {
                    var smokyDamage = player.GetComponent<SmokyDamageComponent>();
                    //wtf is going on, not working code of findtarget
                    var shotEvent = updatingEvent as ShotEvent;
                    
                    this.explosiveSprite.SetActive(true);
                    this.explosiveSprite.transform.SetParent(parentTankManager.battleManager.MapManager.EffectSpace.transform);
                    var hitKP = shotEvent.hitList.ElementAt(0);
                    var hipoint = hitKP.Value.ConvertToUnityVector3();
                    this.explosiveSprite.transform.position = hipoint;
                    this.explosiveSprite.GetComponent<AnimationScript>().Play();
                    if(hitKP.Key != -1)
                    {
                        var tankInfluence = ManagerScope.entityManager.EntityStorage[hitKP.Key].GetComponent<EntityManagersComponent>().Get<TankManager>();
                        tankInfluence.Hull.GetComponent<Rigidbody>().AddForceAtPosition((hipoint - parentTankManager.MuzzlePoint.transform.position).normalized * smokyDamage.impactProperty * Const.impactScaler, hipoint);
                    }
                    parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * smokyDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);
                    if(weaponAudio != null)
                        weaponAudio.audioManager.PlayBlock(new List<string> { "smoky_shot" });
                }
                catch
                {
                    ULogger.Log("Error shot");
                }
            }
        }

        public override void RemoveTurret(ECSEntity player)
        {
            base.DestroyTurret(player);
            Destroy(shotSprite);
            Destroy(explosiveSprite);
        }

        public override void RebuildTank(ECSEntity player)
        {
            base.RebuildTank(player);
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
            if(parentTankManager.TestingMode)
                return;
            if (ShootTurn > 0 && (!parentTankManager.Ghost && !parentTankManager.checkGhost))
            {
                var playerEntity = ManagerScope.entityManager.EntityStorage[parentTankManager.ManagerEntityId];
                //(!playerEntity.HasComponent(WeaponCooldownComponent.Id) ||
                if ((playerEntity.GetComponent<WeaponEnergyComponent>().MaxEnergy == playerEntity.GetComponent<WeaponEnergyComponent>().Energy) && !playerEntity.HasComponent(WeaponTechCooldownComponent.Id))
                {
                    //ULogger.Log(playerEntity.GetComponent<WeaponEnergyComponent>().Energy);
                    var smokyDamage = playerEntity.GetComponent<SmokyDamageComponent>();
                    Vector3 pointOfExplosive = CheckMuzzleVisible();
                    bool checkMuzzle = pointOfExplosive == Vector3.zero;

                    bool sended = false;

                    if(checkMuzzle)
                    {
                        var list = this.parentTankManager.raycastAIM.FindPossibleTargets(parentTankManager);
                        if (list.Count > 0)
                        {
                            ClientNetworkService.instance.Socket.emit<RawShotEvent>(new RawShotEvent()
                            {
                                hitDistanceList = new Dictionary<long, float>() {
                                    {list[0].Item1.ManagerEntityId, 0 }
                                },
                                hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { list[0].Item1.ManagerEntityId, new UTanksClient.ECS.Types.Battle.Vector3S(list[0].Item2) } },
                                hitLocalDistanceList = new Dictionary<long, float>() { },
                                MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S((list[0].Item2 - parentTankManager.MuzzlePoint.transform.position).normalized),
                                StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(parentTankManager.MuzzlePoint.transform.position),
                                StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                            });
                            sended = true;
                            pointOfExplosive = list[0].Item2;
                            list[0].Item1.Hull.GetComponent<Rigidbody>().AddForceAtPosition((list[0].Item2 - parentTankManager.MuzzlePoint.transform.position).normalized * smokyDamage.impactProperty * Const.impactScaler, list[0].Item2);
                        }
                        else
                        {
                            RaycastHit hit;
                            Ray ray = new Ray(parentTankManager.MuzzlePoint.transform.position, parentTankManager.MuzzlePoint.transform.forward * -1);

                            if (Physics.Raycast(ray, out hit, float.MaxValue, this.parentTankManager.raycastAIM.layerMask, QueryTriggerInteraction.Ignore))
                            {
                                if (hit.collider != null)
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
                                }
                                else
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
                                {-1, int.MaxValue }
                            },
                            hitList = new Dictionary<long, UTanksClient.ECS.Types.Battle.Vector3S>() { { -1, new UTanksClient.ECS.Types.Battle.Vector3S(float.MaxValue, float.MaxValue, float.MaxValue) } },
                            hitLocalDistanceList = new Dictionary<long, float>() { },
                            MoveDirectionNormalized = new UTanksClient.ECS.Types.Battle.Vector3S(),
                            StartGlobalPosition = new UTanksClient.ECS.Types.Battle.Vector3S(),
                            StartGlobalRotation = new UTanksClient.ECS.Types.Battle.QuaternionS()
                        });
                    }

                    playerEntity.AddComponent(new WeaponTechCooldownComponent(0.4f).SetGlobalComponentGroup());
                    if (pointOfExplosive != Vector3.zero)
                    {
                        this.explosiveSprite.SetActive(true);
                        this.explosiveSprite.transform.SetParent(parentTankManager.battleManager.MapManager.EffectSpace.transform);
                        this.explosiveSprite.transform.position = pointOfExplosive;
                        this.explosiveSprite.GetComponent<AnimationScript>().Play();

                    }
                    parentTankManager.Hull.GetComponent<Rigidbody>().AddForceAtPosition(parentTankManager.Turret.transform.forward * smokyDamage.kickbackProperty * Const.impactScaler, this.parentTankManager.MuzzlePoint.transform.position);
                    if(weaponAudio != null)
                        weaponAudio.audioManager.PlayBlock(new List<string> { "smoky_shot" });

                    this.shotSprite.GetComponent<AnimationScript>().Play();
                }

            }
        }

        public void Shoot()
        {

        }

        public override ITurret AppendManagerToObject(GameObject turretObject)
        {
            return turretObject.AddComponent<SmokyManager>();
        }
    }
}