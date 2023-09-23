using SecuredSpace.Battle.Tank.Hull;
using SecuredSpace.Battle.Tank.Turret;
using SecuredSpace.Important.Raven;
using SecuredSpace.ClientControl.Log;
using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.Extensions;
using UTanksClient.Network.NetworkEvents.FastGameEvents;

namespace SecuredSpace.Important.TPhysics
{
    public class TankChassisManager : MonoBehaviour
    {
        public ChassisSystem MainChassisSystem;
        public ChassisSystem.ChassisNode chassisNode;
        public TankJumpComponent tankJump;
        public GameTankSettingsComponent settingsComponent;
        public ChassisSystem.ChassisInitNode chassisInit;
        public BoxCollider MainBoundsCollider;
        //public BoxCollider SideDamperCollider;
        public ChassisSmootherComponent chassisSmoother;
        public SpeedConfigComponent speedConfig;
        public Entity entity;
        public ChassisConfigComponent chassisConfigComponent;

        public HullManager parentHullManager;
        public RawMovementEvent preparedMovementEvent = new RawMovementEvent();
        public TurretRotaion turretRotation;

        public bool isTankOnGround
        {
            get
            {
                try
                {
                    return chassisNode.track.LeftTrack.numContacts + chassisNode.track.RightTrack.numContacts > 0;
                }
                catch
                {
                    UTanksClient.Core.Logging.ULogger.Log("Error: chassis not initialized");
                }
                return false;
            }
        }

        public bool Inited;


        public float TurnSpeed = 10f;
        public float Acceleration = 10f;
        public float Speed = 10f;
        public float MaxRayLength = 100f;
        public float NominalRayLength = 100f;
        public int NumRaysPerTrack = 100;
        public float SuspensionRayOffsetY = 10f;
        public float TrackSeparation = 1f;
        public float Damping = 0f;
        public float Weight = 1000f;
        public float rigidbodyDrag = 0f;
        public float rigidbodyAngularDrag = 0.3f;
        public float ReverseAcceleration = 10f;
        public float ReverseTurnAcceleration = 10f;
        public float SideAcceleration = 30f;
        public float TurnAcceleration = 50f;
        public float SideSprindDamperDelta = 0.00003f;
        public float FrontSpringDamperDelta = 0.000007f;
        [Space(10)]
        public float maxSpeedSmoothStart = 10f;
        public float maxSpeedSmoothFinal = 20f;
        public float maxTurnSpeedSmootherStart = 30f;
        public float maxTurnSpeedSmootherFinal = 40f;
        [Space(10)]
        public float SpringScaler = 2f;
        public float SpringCoef = 0f;
        public float SideDamperScaler = 2f;
        public float SwingCoef = 1f;
        public bool TankMovable = false;
        public float TorqueAcceleration;
        public float StartTorqueStrength;
        public float SideTorqueDamper;
        public bool StopTank = false;
        public LayerMask currentLayerMask;

        private void Awake()
        {
            currentLayerMask = LayerMask.GetMask("Default", "TankBounds");
        }

        private void OnEnable()
        {
            ApplyPhysics();
        }

        public void HotApplyPhysics()
        {
            chassisNode.speedConfig.ReverseAcceleration = ReverseAcceleration;
            chassisNode.speedConfig.ReverseTurnAcceleration = ReverseTurnAcceleration;
            chassisNode.speedConfig.SideAcceleration = SideAcceleration;
            chassisNode.speedConfig.TurnAcceleration = TurnAcceleration;
            chassisNode.speed.TurnSpeed = TurnSpeed;
            chassisNode.speed.Acceleration = Acceleration;
            chassisNode.speed.Speed = Speed;
            MainChassisSystem.SpringScaler = SpringScaler;
            MainChassisSystem.SpringCoefManual = SpringCoef;
            MainChassisSystem.SideDamperScaler = SideDamperScaler;
            MainChassisSystem.SideSprindDamperDelta = SideSprindDamperDelta;
            MainChassisSystem.FrontSpringDamperDelta = FrontSpringDamperDelta;
            MainChassisSystem.SwingCoef = SwingCoef;
        }

        public void ApplyPhysics()
        {
            //currentLayerMask = LayerMask.GetMask("Default", "TankBounds");
            MainChassisSystem = new ChassisSystem
            {
                SpringScaler = SpringScaler,
                SpringCoefManual = SpringCoef,
                SideDamperScaler = SideDamperScaler,
                SideSprindDamperDelta = SideSprindDamperDelta,
                FrontSpringDamperDelta = FrontSpringDamperDelta,
                SwingCoef = SwingCoef,
                chassisManager = this,
                LogComponent = this.GetComponent<LogComponent>()
            };
            entity = new Entity();
            var rigidbodyUC = this.GetComponent<Rigidbody>();
            rigidbodyUC.angularDrag = rigidbodyAngularDrag;
            rigidbodyUC.drag = rigidbodyDrag;
            RigidbodyComponent rigidbodyComponent = new RigidbodyComponent(rigidbodyUC);
            chassisInit = new ChassisSystem.ChassisInitNode();
            chassisInit.rigidbody = rigidbodyComponent;
            SpeedComponent speedComponent = new SpeedComponent
            {
                TurnSpeed = TurnSpeed,
                Acceleration = Acceleration,
                Speed = Speed
            };
            chassisInit.speed = speedComponent;
            TankCollidersComponent tankColliders = new TankCollidersComponent
            {
                BoundsCollider = MainBoundsCollider,
                //SideDamperCollider = SideDamperCollider
            };
            chassisInit.tankColliders = tankColliders;
            chassisConfigComponent = new ChassisConfigComponent
            {
                MaxRayLength = MaxRayLength,
                NominalRayLength = NominalRayLength,
                NumRaysPerTrack = NumRaysPerTrack,
                ReverseBackTurn = false,
                SuspensionRayOffsetY = SuspensionRayOffsetY,
                TrackSeparation = TrackSeparation,
                trackLayerMask = currentLayerMask
            };

            chassisInit.chassisConfig = chassisConfigComponent;

            DampingComponent dampingComponent = new DampingComponent { Damping = Damping };

            chassisInit.damping = dampingComponent;

            chassisInit.weight = new WeightComponent { Weight = Weight };

            chassisSmoother = new ChassisSmootherComponent
            {
                maxSpeedSmoother = new SimpleValueSmoother(maxSpeedSmoothStart, maxSpeedSmoothFinal, 0f, 0f),
                maxTurnSpeedSmoother = new SimpleValueSmoother(maxTurnSpeedSmootherStart, maxTurnSpeedSmootherFinal, 0f, 0f)
            };

            chassisInit.Entity = entity;

            MainChassisSystem.InitTankChassis(chassisInit);

            speedConfig = new SpeedConfigComponent
            {
                ReverseAcceleration = ReverseAcceleration,
                ReverseTurnAcceleration = ReverseTurnAcceleration,
                SideAcceleration = SideAcceleration,
                TurnAcceleration = TurnAcceleration
            };

            settingsComponent = new GameTankSettingsComponent
            {
                DamageInfoEnabled = false,
                HealthFeedbackEnabled = false,
                MovementControlsInverted = false,
                SelfTargetHitFeedbackEnabled = false
            };



            chassisNode = new ChassisSystem.ChassisNode
            {
                Entity = entity,
                chassis = (ChassisComponent)entity.storage[1],
                effectiveSpeed = (EffectiveSpeedComponent)entity.storage[2],
                track = (TrackComponent)entity.storage[0],
                speed = speedComponent,
                chassisConfig = chassisConfigComponent,
                chassisSmoother = chassisSmoother,
                rigidbody = rigidbodyComponent,
                tankColliders = tankColliders,
                cameraVisibleTrigger = null,
                speedConfig = speedConfig,
                tankGroup = null
            };

            tankJump = new TankJumpComponent { };

            Inited = true;
        }

        // Update is called once per frame
        
        void FixedUpdate()
        {
            if (Inited && !StopTank)
            {
                MainChassisSystem.FixedUpdate(chassisNode, tankJump, settingsComponent, TankMovable);
                NetworkPositionSynchronization(chassisNode);
                NetworkPositionStatementsManager(chassisNode);
                //MainChassisSystem.UpdateSelfInput(chassisNode, true, false);
            }
        }


        private RawMovementEvent lastSynchroniztionMovementEvent;
        private bool lastSyncPacketApplied = false;
        public void SynchronizePosition(RawMovementEvent rawMovementEvent)
        {
            lastSynchroniztionMovementEvent = rawMovementEvent;
            lastSyncPacketApplied = false;
        }

        private void NetworkPositionStatementsManager(ChassisSystem.ChassisNode chassisNode)
        {
            if(!this.TankMovable && lastSynchroniztionMovementEvent.PlayerEntityId != 0)
            {
                if (!lastSyncPacketApplied)
                {
                    this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, lastSynchroniztionMovementEvent.position.ConvertToUnityVector3Constant007Scaling(), 0.7f);
                    this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, lastSynchroniztionMovementEvent.rotation.ToQuaternion(), 0.7f);
                    this.turretRotation.transform.localRotation = lastSynchroniztionMovementEvent.turretRotation.ToQuaternion();
                    //chassisNode.rigidbody.Rigidbody.velocity = lastSynchroniztionMovementEvent.velocity.ConvertToUnityVector3();
                    //chassisNode.rigidbody.Rigidbody.angularVelocity = lastSynchroniztionMovementEvent.angularVelocity.ConvertToUnityVector3();
                    lastSyncPacketApplied = true;
                }
                chassisNode.chassis.MoveAxis = lastSynchroniztionMovementEvent.TankMoveControl;
                chassisNode.chassis.TurnAxis = lastSynchroniztionMovementEvent.TankTurnControl;
                chassisNode.chassis.EffectiveMoveAxis = lastSynchroniztionMovementEvent.TankMoveControl;
                chassisNode.chassis.EffectiveTurnAxis = lastSynchroniztionMovementEvent.TankTurnControl;
                
                turretRotation.ExternalManagement(lastSynchroniztionMovementEvent.WeaponRotation, lastSynchroniztionMovementEvent.WeaponRotationControl);
            }
        }

        private bool PreparedToSend = false;
        private float EffectiveMoveLastSend;
        private float EffectiveMoveHistorycal;
        private int EffectiveMoveHistorycalDirection; //-1 - down; 0 - stay; 1 - up 
        private float EffectiveTurnLastSend;
        private float EffectiveTurnHistorycal;
        private int EffectiveTurnHistorycalDirection; //-1 - down; 0 - stay; 1 - up 
        private Vector3 HullSpeedLastSend;
        private float HullSpeedHistorycal;
        private int HullSpeedHistorycalDirection; //-1 - down; 0 - stay; 1 - up
        private float TurretSpeedLastSend;
        private float TurretSpeedHistorycal;
        private int TurretSpeedHistorycalDirection; //-1 - down; 0 - stay; 1 - up 
        void NetworkPositionSynchronization(ChassisSystem.ChassisNode chassisNode)
        {
            if(TankMovable)
            {
                if ((chassisNode.track.LeftTrack.numContacts > 0 || chassisNode.track.RightTrack.numContacts > 0))
                {
                    #region EffectiveMoveHistorycal
                    if (EffectiveMoveHistorycal == chassisNode.chassis.EffectiveMoveAxis)
                    {
                        EffectiveMoveHistorycal = chassisNode.chassis.EffectiveMoveAxis;
                        EffectiveMoveHistorycalDirection = 0;
                    }
                    if (EffectiveMoveHistorycal > chassisNode.chassis.EffectiveMoveAxis)
                    {
                        EffectiveMoveHistorycal = chassisNode.chassis.EffectiveMoveAxis;
                        EffectiveMoveHistorycalDirection = 1;
                    }
                    if (EffectiveMoveHistorycal < chassisNode.chassis.EffectiveMoveAxis)
                    {
                        EffectiveMoveHistorycal = chassisNode.chassis.EffectiveMoveAxis;
                        EffectiveMoveHistorycalDirection = -1;
                    }
                    if (EffectiveMoveLastSend != EffectiveMoveHistorycal)
                    {
                        EffectiveMoveLastSend = EffectiveMoveHistorycal;
                        PreparedToSend = true;
                    }
                    #endregion

                    #region EffectiveTurnHistorycal
                    if (EffectiveTurnHistorycal == chassisNode.chassis.EffectiveTurnAxis)
                    {
                        EffectiveTurnHistorycal = chassisNode.chassis.EffectiveTurnAxis;
                        EffectiveTurnHistorycalDirection = 0;
                    }
                    if (EffectiveTurnHistorycal > chassisNode.chassis.EffectiveTurnAxis)
                    {
                        EffectiveTurnHistorycal = chassisNode.chassis.EffectiveTurnAxis;
                        EffectiveTurnHistorycalDirection = 1;
                    }
                    if (EffectiveTurnHistorycal < chassisNode.chassis.EffectiveTurnAxis)
                    {
                        EffectiveTurnHistorycal = chassisNode.chassis.EffectiveTurnAxis;
                        EffectiveTurnHistorycalDirection = -1;
                    }
                    if (EffectiveTurnLastSend != EffectiveTurnHistorycal)
                    {
                        EffectiveTurnLastSend = EffectiveTurnHistorycal;
                        PreparedToSend = true;
                    }
                    #endregion

                    #region HullSpeedHistorycal
                    int newHullSpeedDirectonValue = 0;
                    if (HullSpeedHistorycal == chassisNode.rigidbody.Rigidbody.velocity.magnitude)
                    {
                        HullSpeedHistorycal = chassisNode.rigidbody.Rigidbody.velocity.magnitude;
                        newHullSpeedDirectonValue = 0;
                    }
                    if (HullSpeedHistorycal > chassisNode.rigidbody.Rigidbody.velocity.magnitude)
                    {
                        HullSpeedHistorycal = chassisNode.rigidbody.Rigidbody.velocity.magnitude;
                        newHullSpeedDirectonValue = 1;
                    }
                    if (HullSpeedHistorycal < chassisNode.rigidbody.Rigidbody.velocity.magnitude)
                    {
                        HullSpeedHistorycal = chassisNode.rigidbody.Rigidbody.velocity.magnitude;
                        newHullSpeedDirectonValue = -1;
                    }
                    if (HullSpeedHistorycalDirection != newHullSpeedDirectonValue)
                    {
                        HullSpeedLastSend = chassisNode.rigidbody.Rigidbody.velocity;
                        //PreparedToSend = true;
                    }
                    HullSpeedHistorycalDirection = newHullSpeedDirectonValue;
                    #endregion

                    #region TurretTurnHistorycal
                    if (turretRotation != null)
                    {
                        int newTurretSpeedDirectonValue = 0;
                        float turretControl = (turretRotation.LeftTurn * -1 + turretRotation.RightTurn);
                        if (TurretSpeedHistorycal == turretControl)
                        {
                            TurretSpeedHistorycal = turretControl;
                            newTurretSpeedDirectonValue = 0;
                        }
                        if (TurretSpeedHistorycal > turretControl)
                        {
                            TurretSpeedHistorycal = turretControl;
                            newTurretSpeedDirectonValue = 1;
                        }
                        if (TurretSpeedHistorycal < turretControl)
                        {
                            TurretSpeedHistorycal = turretControl;
                            newTurretSpeedDirectonValue = -1;
                        }
                        if (TurretSpeedHistorycalDirection != newTurretSpeedDirectonValue)
                        {
                            TurretSpeedLastSend = turretControl;
                            PreparedToSend = true;
                        }
                        TurretSpeedHistorycalDirection = newTurretSpeedDirectonValue;
                    }
                    #endregion



                    //if (BUCKET[BUCKET.Count-1] != EffectiveMoveHistorycalDirection)
                    //    BUCKET.Add(EffectiveMoveHistorycalDirection);
                }

                #region PrepareToSend
                preparedMovementEvent.position = UTanksClient.ECS.Types.Battle.Vector3S.ConvertToVector3SUnScaling(this.transform.localPosition, Const.ResizeResourceConst);
                preparedMovementEvent.rotation = new UTanksClient.ECS.Types.Battle.QuaternionS(this.transform.localRotation);
                preparedMovementEvent.PlayerEntityId = parentHullManager.parentTankManager.ManagerEntityId;
                preparedMovementEvent.TankMoveControl = EffectiveMoveLastSend;
                preparedMovementEvent.TankTurnControl = EffectiveTurnLastSend;
                preparedMovementEvent.velocity = new UTanksClient.ECS.Types.Battle.Vector3S(HullSpeedLastSend);
                preparedMovementEvent.WeaponRotation = turretRotation.nowSpeed;
                preparedMovementEvent.WeaponRotationControl = TurretSpeedLastSend;
                preparedMovementEvent.turretRotation = new UTanksClient.ECS.Types.Battle.QuaternionS(parentHullManager.parentTankManager.Turret.transform.localRotation);
                #endregion


                if (PreparedToSend)
                {
                    TaskEx.RunAsync(() => {
                        ClientNetworkService.instance.Socket.emit(preparedMovementEvent);
                    });
                    PreparedToSend = false;
                }
            }
            
        }
    
    }
}