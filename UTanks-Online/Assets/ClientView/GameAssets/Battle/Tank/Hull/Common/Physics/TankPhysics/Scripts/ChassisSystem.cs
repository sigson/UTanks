using SecuredSpace.Important.TPhysics;
using SecuredSpace.ClientControl.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Types.Battle;

namespace SecuredSpace.Important.Raven
{
    public class ChassisSystem
    {
        private const float SELF_TANK_UPDATE_PERIOD = 0f;
        private const float REMOTE_TANK_UPDATE_PERIOD = 0.05f;
        private const float REMOTE_INVISBILE_TANK_UPDATE_PERIOD = 0.1f;
        private const float REMOTE_RANDOM_TANK_UPDATE_PERIOD = 0.05f;
        private static readonly string RIGHT_AXIS = "MoveRight";
        private static readonly string LEFT_AXIS = "MoveLeft";
        private static readonly string FORWARD_AXIS = "MoveForward";
        private static readonly string BACKWARD_AXIS = "MoveBackward";
        private const float MIN_ACCELERATION = 4f;
        private const float SQRT1_2 = 0.7071068f;
        private const float FULL_FORCE_ANGLE = 0.7853982f; //(float)Math.PI / 4f;
        private const float ZERO_FORCE_ANGLE = 1.047198f; //(float)Math.PI / 3f;
        private const float FULL_SLOPE_ANGLE = 0.7853982f; //(float)Math.PI / 4f;
        private const float MAX_SLOPE_ANGLE = 1.047198f; //(float)Math.PI / 3f;
        private static readonly float FULL_SLOPE_COS_ANGLE = Mathf.Cos(0.7853982f); //(float)Math.PI / 4f
        private static readonly float MAX_SLOPE_COS_ANGLE = Mathf.Cos(1.047198f); //(float)Math.PI / 3f
        public TankChassisManager chassisManager;
        public float SpringScaler = 2f;
        public float SpringCoefManual = 0f;
        public float SideDamperScaler = 2f;
        public float InertiaTorqueScaler = 1f;
        public LogComponent LogComponent;
        public float SideSprindDamperDelta = 0.00003f;
        public float FrontSpringDamperDelta = 0.000007f;
        public float SwingCoef = 1f;

        public Vector3 inertiaVelocity = new Vector3();

        private void AddSurfaceVelocitiesFromRay(ChassisNode node, SuspensionRay ray, Vector3 contactsMidpoint, ref Vector3 surfaceVelocity, ref Vector3 angularSurfaceVelocity)
        {
            if (ray.hasCollision)
            {
                surfaceVelocity += ray.surfaceVelocity;
                Vector3 lhs = ray.rayHit.point - contactsMidpoint;
                float sqrMagnitude = lhs.sqrMagnitude;
                if (sqrMagnitude > 0.0001f)
                {
                    angularSurfaceVelocity += Vector3.Cross(lhs, ray.surfaceVelocity) / sqrMagnitude;
                }
            }
        }

        private void AdjustSuspensionSpringCoeff(ChassisConfigComponent chassisConfig, ChassisComponent chassis, Rigidbody rigidbody)
        {
            float num = Physics.gravity.magnitude * rigidbody.mass;
            chassis.SpringCoeff = num / ((SpringScaler * chassisConfig.NumRaysPerTrack) * (chassisConfig.MaxRayLength - chassisConfig.NominalRayLength));
            if (SpringCoefManual != 0f)
                chassis.SpringCoeff = SpringCoefManual;
        }

        private void ApplyForceFromRay(SuspensionRay ray, Rigidbody rigidbody, Vector3 bodyForwardAxis, float forcePerRay)
        {
            if (ray.hasCollision)
            {
                float num = Mathf.Abs(Mathf.Acos(ray.rayHit.normal.normalized.y));
                if (num < 1.047198f)
                {
                    float num2 = forcePerRay;
                    if (num > 1.047198f)
                    {
                        num2 *= (1.047198f - num) / 0.2617994f;
                    }
                    Vector3 force = bodyForwardAxis * num2 * (1 * (Math.Abs(rigidbody.velocity.x) > 0.5 || Math.Abs(rigidbody.velocity.z) > 0.5f ? SwingCoef : 1));
                    rigidbody.AddForceAtPositionSafe(force, ray.GetGlobalOrigin());
                }
            }
        }

        private void ApplyMovementForces(ChassisNode node, float dt)
        {
            TrackComponent track = node.track;
            if ((track.LeftTrack.numContacts + track.RightTrack.numContacts) > 0)
            {
                Vector3 vector;
                Vector3 vector2;
                float num2;
                Rigidbody rigidbody = node.rigidbody.Rigidbody;
                ChassisConfigComponent chassisConfig = node.chassisConfig;
                this.CalculateNetSurfaceVelocities(node, out vector, out vector2);
                //LogComponent.Write("CalculateNetSurfaceVelocities", new LogObj { Vector3 = vector });
                float num = this.CalculateSlopeCoefficient(rigidbody.transform.up.y);
                var calculatedVelocity = this.CalculateRigidBodyVelocity(rigidbody, vector, (node.speedConfig.SideAcceleration * num) * dt, out num2);
                rigidbody.SetVelocitySafe(calculatedVelocity);
                this.CalculateSpringInertia(rigidbody, node, calculatedVelocity);
                if ((track.LeftTrack.numContacts > 0) || (track.RightTrack.numContacts > 0))
                {
                    Vector3 vector4 = this.CalculateRelativeAngularVelocity(node, dt, num * 1.2f, vector2, num2);
                    Vector3 normalized = rigidbody.transform.InverseTransformDirection(rigidbody.angularVelocity).normalized;
                    if ((Mathf.Abs(node.chassis.TurnAxis) > 0f) && (Mathf.Sign(normalized.y) != Mathf.Sign(node.chassis.TurnAxis)))
                    {
                        float y = Mathf.Lerp(0f, normalized.y, (0.2f * dt) * 60f);
                        vector2 -= rigidbody.transform.TransformDirection(new Vector3(0f, y, 0f));
                    }
                    rigidbody.SetAngularVelocitySafe(vector2 + vector4);
                }
            }
        }

        //public void ApplySideDamper(ChassisNode node, float dt)//выпилить
        //{
        //    if (node.chassis.MoveAxis != 0)
        //        node.tankColliders.SideDamperCollider.center = new Vector3(node.chassis.TurnAxis / SideDamperScaler, node.tankColliders.SideDamperCollider.center.y, node.tankColliders.SideDamperCollider.center.z);
        //    else
        //        node.tankColliders.SideDamperCollider.center = new Vector3(0, node.tankColliders.SideDamperCollider.center.y, node.tankColliders.SideDamperCollider.center.z);
        //}

        public void ApplyStaticFriction(TrackComponent tracks, Rigidbody rigidbody) //убирает влияние силы притяжения (видно при попытке вхобраться на подьем
        {
            if ((tracks.RightTrack.numContacts >= (tracks.RightTrack.rays.Length >> 1)) || (tracks.LeftTrack.numContacts >= (tracks.LeftTrack.rays.Length >> 1)))
            {
                Vector3 up = rigidbody.transform.up;
                float num = Vector3.Dot(Physics.gravity, up);
                float num2 = 0.7071068f * Physics.gravity.magnitude;
                if ((num < -num2) || (num > num2))
                {
                    Vector3 force = ((up * num) - Physics.gravity) * rigidbody.mass;
                    rigidbody.AddForceSafe(force);
                }
            }
        }

        private float CalculateForcePerRay(ChassisNode node, float dt, float forwardRelativeSpeed)
        {
            ChassisConfigComponent chassisConfig = node.chassisConfig;
            ChassisComponent chassis = node.chassis;
            float maxSpeed = node.effectiveSpeed.MaxSpeed;
            TrackComponent track = node.track;
            Rigidbody rigidbody = node.rigidbody.Rigidbody;
            float acceleration = node.speed.Acceleration;
            float num3 = 0f;
            if (chassis.EffectiveMoveAxis == 0f)
            {
                num3 = (-MathUtil.Sign(forwardRelativeSpeed) * acceleration) * dt;
                if (MathUtil.Sign(forwardRelativeSpeed) != MathUtil.Sign(forwardRelativeSpeed + num3))
                {
                    num3 = -forwardRelativeSpeed;
                }
            }
            else
            {
                if (this.IsReversedMove(chassis.EffectiveMoveAxis, forwardRelativeSpeed))
                {
                    acceleration = node.speedConfig.ReverseAcceleration;
                }
                num3 = (chassis.EffectiveMoveAxis * acceleration) * dt;
            }
            float f = Mathf.Clamp(forwardRelativeSpeed + num3, -maxSpeed, maxSpeed);
            float num5 = f - forwardRelativeSpeed;
            float num6 = 1f;
            float num7 = (maxSpeed <= 0f) ? num6 : Mathf.Clamp01(1f - Mathf.Abs((float)(forwardRelativeSpeed / maxSpeed)));
            if ((num7 < num6) && ((chassis.EffectiveMoveAxis * MathUtil.Sign(forwardRelativeSpeed)) > 0f))
            {
                num5 *= num7 / num6;
            }
            float num8 = num5 / dt;
            if ((Mathf.Abs(num8) < 4f) && (Mathf.Abs(f) > (0.5f * maxSpeed)))
            {
                num8 = MathUtil.SignEpsilon(num8, 0.1f) * 4f;
            }
            int num10 = track.LeftTrack.numContacts + track.RightTrack.numContacts;
            int num11 = 2 * chassisConfig.NumRaysPerTrack;
            float num12 = ((num8 * rigidbody.mass) * (num10 + (0.42f * (num11 - track.LeftTrack.numContacts)))) / ((float)num11);
            return ((num10 <= 0) ? num12 : (num12 / ((float)num10)));
        }

        private void CalculateNetSurfaceVelocities(ChassisNode node, out Vector3 surfaceVelocity, out Vector3 angularSurfaceVelocity)
        {
            ChassisConfigComponent chassisConfig = node.chassisConfig;
            TrackComponent track = node.track;
            Vector3 contactsMidpoint = this.GetContactsMidpoint(chassisConfig, track);
            surfaceVelocity = Vector3.zero;
            angularSurfaceVelocity = Vector3.zero;
            for (int i = 0; i < chassisConfig.NumRaysPerTrack; i++)
            {
                this.AddSurfaceVelocitiesFromRay(node, track.LeftTrack.rays[i], contactsMidpoint, ref surfaceVelocity, ref angularSurfaceVelocity);
                this.AddSurfaceVelocitiesFromRay(node, track.RightTrack.rays[i], contactsMidpoint, ref surfaceVelocity, ref angularSurfaceVelocity);
            }
            float num2 = track.LeftTrack.numContacts + track.RightTrack.numContacts;
            surfaceVelocity = (num2 <= 0f) ? surfaceVelocity : (surfaceVelocity / num2);
            angularSurfaceVelocity = (num2 <= 0f) ? angularSurfaceVelocity : (angularSurfaceVelocity / num2);
        }

        private Vector3 CalculateRelativeAngularVelocity(ChassisNode node, float dt, float slopeCoeff, Vector3 surfaceAngularVelocity, float forwardRelativeSpeed)
        {
            ChassisConfigComponent chassisConfig = node.chassisConfig;
            TrackComponent track = node.track;
            Rigidbody rigidbody = node.rigidbody.Rigidbody;
            float maxTurnSpeed = node.effectiveSpeed.MaxTurnSpeed * 0.01745329f;
            Vector3 up = rigidbody.transform.up;
            Vector3 forward = rigidbody.transform.forward;
            Vector3 lhs = rigidbody.angularVelocity - surfaceAngularVelocity;
            float relativeTurnSpeed = Vector3.Dot(lhs, up);
            float forcePerRay = this.CalculateForcePerRay(node, dt, forwardRelativeSpeed);
            for (int i = 0; i < chassisConfig.NumRaysPerTrack; i++)
            {
                this.ApplyForceFromRay(track.LeftTrack.rays[i], rigidbody, forward, forcePerRay);
                this.ApplyForceFromRay(track.RightTrack.rays[i], rigidbody, forward, forcePerRay);
            }
            float num5 = Vector3.Dot(lhs, up);
            return (lhs + ((this.RecalculateRelativeTurnSpeed(node, dt, maxTurnSpeed, relativeTurnSpeed, slopeCoeff) - num5) * up));
        }

        private Vector3 CalculateRigidBodyVelocity(Rigidbody rigidbody, Vector3 surfaceVelocity, float sideSpeedDelta, out float forwardRelativeSpeed)
        {
            Vector3 right = rigidbody.transform.right;
            Vector3 lhs = rigidbody.velocity - surfaceVelocity;
            //forwardRelativeSpeed = 0f;
            forwardRelativeSpeed = Vector3.Dot(lhs, rigidbody.transform.forward);
            //LogComponent.Write("surfaceVelocity", new LogObj { Float = forwardRelativeSpeed });
            lhs += this.CalculateSideVelocityDelta(lhs, right, sideSpeedDelta) * right;
            //if (lhs.magnitude > 0.32f)
            //    lhs -= this.CalculateSideVelocityDelta(lhs, rigidbody.transform.forward, sideSpeedDelta * 10) * rigidbody.transform.forward;
            return (surfaceVelocity + lhs);
        }

        private void CalculateMoveInertia(Rigidbody rigidbody, ChassisNode node, Vector3 velocity)
        {
            float moveInertiaCoef = 0.1f;
            float t = rigidbody.mass / 1000 * Time.fixedDeltaTime * moveInertiaCoef;
            rigidbody.velocity = new Vector3(Mathf.Lerp(inertiaVelocity.x, rigidbody.velocity.x, t), rigidbody.velocity.y, Mathf.Lerp(inertiaVelocity.z, rigidbody.velocity.z, t));
        }

        private void CalculateSpringInertia(Rigidbody rigidbody, ChassisNode node, Vector3 velocity)
        {
            var rigidEuler = rigidbody.transform.rotation.eulerAngles;
            var NullRotateVelocity = Quaternion.Euler((rigidEuler.x >= 0 ? 360 - rigidEuler.x : 360 + rigidEuler.x), (rigidEuler.y >= 0 ? 360 - rigidEuler.y : 360 + rigidEuler.y), (rigidEuler.z >= 0 ? 360 - rigidEuler.z : 360 + rigidEuler.z)) * velocity;

            //LogComponent.Write("rigidEuler", new LogObj { Vector3 = rigidbody.transform.rotation.eulerAngles });
            var MoveAngle = DegreeDistinct(rigidbody.transform.rotation.eulerAngles.y, RigidbodyDirtyMoveAngle(rigidbody));//Quaternion.Angle(Quaternion.Euler(0f, rigidbody.transform.rotation.eulerAngles.y, 0f), Quaternion.Euler(0f, RigidbodyDirtyMoveAngle(rigidbody), 0f));//RigidbodyDirtyMoveAngle(rigidbody) - rigidbody.transform.rotation.eulerAngles.y;//rigidbody.transform.rotation.eulerAngles.y -
                                                                                                                           //LogComponent.Write("DirtyMoveAngle", new LogObj { Float = RigidbodyDirtyMoveAngle(rigidbody) });
                                                                                                                           //LogComponent.Write("MoveAngle", new LogObj {Float = MoveAngle });
            float rightPercentage = 0f;
            float forwardPercentage = 0f;

            if (MoveAngle >= 0 && MoveAngle <= 90)
            {
                rightPercentage = (((MoveAngle)) / 90);
                forwardPercentage = (1 - ((MoveAngle)) / 90) * -1;
                //LogComponent.Write("MoveDirect", new LogObj { String = "LeftBackward", Float = MoveAngle });
            }
            else if (MoveAngle > 90 && MoveAngle <= 180)
            {
                rightPercentage = (1 - (MoveAngle - 90) / 90);
                forwardPercentage = ((MoveAngle - 90) / 90);
                //LogComponent.Write("MoveDirect", new LogObj { String = "LeftForward", Float = MoveAngle });

            }
            else if (MoveAngle > 180 && MoveAngle <= 270)
            {

                rightPercentage = (MoveAngle - 180) / 90 * -1;
                forwardPercentage = 1 - (MoveAngle - 180) / 90;
                //LogComponent.Write("MoveDirect", new LogObj { String = "RightForward", Float = MoveAngle });
            }
            else if (MoveAngle > 270 && MoveAngle <= 360)
            {
                rightPercentage = (1 - (MoveAngle - 270) / 90) * -1;
                forwardPercentage = ((MoveAngle - 270) / 90) * -1;
                //LogComponent.Write("MoveDirect", new LogObj { String = "RightBackward", Float = MoveAngle });
            }

            //LogComponent.Write("rightPercentage", new LogObj { Float = rightPercentage });
            //LogComponent.Write("forwardPercentage", new LogObj { Float = forwardPercentage });

            //rigidbody.angularVelocity = new Vector3(0f, rigidbody.angularVelocity.y, rigidbody.angularVelocity.z);

            rigidbody.angularVelocity = rigidbody.transform.forward * rightPercentage * rigidbody.velocity.magnitude * rigidbody.mass * SideSprindDamperDelta * ((float)(node.track.LeftTrack.numContacts + node.track.RightTrack.numContacts) / (node.chassisConfig.NumRaysPerTrack * 2f)) + rigidbody.angularVelocity; //workmodel
                                                                                                                                                                                           //rigidbody.angularVelocity = rigidbody.transform.right * forwardPercentage * rigidbody.velocity.magnitude * rigidbody.mass * FrontSpringDamperDelta * -1 + rigidbody.angularVelocity; //workmodel
                                                                                                                                                                                           //rigidbody.angularVelocity = rigidbody.transform.right * forwardPercentage * rigidbody.velocity.magnitude * rigidbody.mass * FrontSpringDamperDelta * Math.Abs(node.chassis.MoveAxis) + rigidbody.angularVelocity; //workmodel //компенсация обычного ускорения должна производиться автоматически, однако старт эффект и постостановочный эффект должны доводиться до кондиции инерцией движения





            ////if(MoveAngle - rigidbody.transform.rotation.eulerAngles.y <= 180)
            ////{
            ////    rightPercentage = MoveAngle / 180;
            ////}
            ////else
            ////{
            ////    rightPercentage = ((MoveAngle - 180) / 180) * -1;
            ////}
            ////LogComponent.Write("rightPercentage", new LogObj { Float = rightPercentage });
            ////float forwardPercentage = (rightPercentage >= 0? 1-rightPercentage:-1-rightPercentage);
            ////LogComponent.Write("forwardPercentage", new LogObj { Float = forwardPercentage });
            //var multiDirection = new Vector3(rigidbody.transform.forward * MoveAngle * rigidbody.transform.up * rigidbody.transform.right * MoveAngle);
            //rigidbody.angularVelocity = rigidbody.transform.forward * rightPercentage * rigidbody.velocity.magnitude * rigidbody.mass * 0.5f + rigidbody.angularVelocity; //workmodel
            //rigidbody.angularVelocity = rigidbody.transform.forward * node.chassis.TurnAxis * 0.5f + rigidbody.angularVelocity;
            //rigidbody.angularVelocity = rigidbody.transform.right * node.chassis.MoveAxis * 0.2f * -1f + rigidbody.angularVelocity;
            ////if (MoveAngle - rigidEuler.y >= 178 || rigidEuler.y - MoveAngle >= 179)
            ////{
            ////    //rigidbody.AddTorque(rigidbody.transform.right * rigidbody.velocity.magnitude * 4000f * -1);//rigidbody.velocity.magnitude * MoveAngle
            ////    //                                                                                           //rigidbody.AddTorque(rigidbody.transform.forward * rigidbody.velocity.magnitude * rightPercentage * 500f);
            ////    //rigidbody.AddTorque(rigidbody.transform.right * rigidbody.velocity.magnitude * Math.Abs(node.chassis.MoveAxis) * 4000f);
            ////}
            ////else
            ////{
            ////    //rigidbody.AddTorque(rigidbody.transform.right * rigidbody.velocity.magnitude * 4000f);//rigidbody.velocity.magnitude * MoveAngle
            ////    //                                                                                      //rigidbody.AddTorque(rigidbody.transform.forward * rigidbody.velocity.magnitude * rightPercentage * 500f);
            ////    //rigidbody.AddTorque(rigidbody.transform.right * rigidbody.velocity.magnitude * Math.Abs(node.chassis.MoveAxis) * 4000f * -1);
            ////}
            //LogComponent.Write("Torque", new LogObj { Vector3 = rigidbody.transform.forward * rigidbody.velocity.magnitude * forwardPercentage * node.chassis.MoveAxis * 500f });
        }

        private float RigidbodyDirtyMoveAngle(Rigidbody rigidbody)
        {
            if (rigidbody.velocity.z >= 0)
            {
                if (rigidbody.velocity.x >= 0)
                    return (float)Math.Atan(Math.Abs(rigidbody.velocity.x) / Math.Abs(rigidbody.velocity.z)) * Mathf.Rad2Deg;
                else
                    return 270 + (float)Math.Atan(Math.Abs(rigidbody.velocity.z) / Math.Abs(rigidbody.velocity.x)) * Mathf.Rad2Deg;

                //var result = Math.atan(num1 / num2) * 180 / Math.PI;
                //var result2 = (3.14159262689154419 - 1.570796313445772097 - Math.atan(num1 / num2)) * 180 / Math.PI;
            }
            else
            {
                if (rigidbody.velocity.x >= 0)
                    return 90 + (float)Math.Atan(Math.Abs(rigidbody.velocity.z) / Math.Abs(rigidbody.velocity.x)) * Mathf.Rad2Deg;
                else
                    return 180 + (float)Math.Atan(Math.Abs(rigidbody.velocity.x) / Math.Abs(rigidbody.velocity.z)) * Mathf.Rad2Deg;
            }

        }

        private float DegreeDistinct(float AngleA, float AngleB)
        {
            if (AngleA < AngleB)
            {
                return 360 - (AngleB - AngleA);
            }
            else
            {
                return (AngleA - AngleB);
            }
            return 0f;
        }
        private float CalculateSideVelocityDelta(Vector3 relativeVelocity, Vector3 xAxis, float sideSpeedDelta)
        {
            float num = Vector3.Dot(relativeVelocity, xAxis);
            float num2 = num;
            if (num2 < 0f)
            {
                num2 = (sideSpeedDelta <= -num2) ? (num2 + sideSpeedDelta) : 0f;
            }
            else if (num2 > 0f)
            {
                num2 = (sideSpeedDelta <= num2) ? (num2 - sideSpeedDelta) : 0f;
            }
            return (num2 - num);
        }

        private float CalculateSlopeCoefficient(float upAxisY)
        {
            float num = 1f;
            if (upAxisY < FULL_SLOPE_COS_ANGLE)
            {
                num = (upAxisY >= MAX_SLOPE_COS_ANGLE) ? ((1.047198f - Mathf.Acos(upAxisY)) / 0.2617994f) : 0f;
            }
            return num;
        }

        private float CalculateTurnCoefficient(TrackComponent trackComponent)
        {
            float num = 1f;
            if ((trackComponent.LeftTrack.numContacts == 0) || (trackComponent.RightTrack.numContacts == 0))
            {
                num = 0.5f;
            }
            return num;
        }

        private void CreateTracks(ChassisInitNode node, ChassisComponent chassis)
        {
            Entity entity = node.Entity;
            ChassisConfigComponent chassisConfig = node.chassisConfig;
            Rigidbody rigidbody = node.rigidbody.Rigidbody;
            BoxCollider boundsCollider = node.tankColliders.BoundsCollider;
            float trackLength = boundsCollider.size.z * 0.8f;
            float num2 = boundsCollider.size.x - chassisConfig.TrackSeparation;
            Vector3 vector3 = boundsCollider.center - new Vector3(0f, boundsCollider.size.y / 2f, 0f);
            Vector3 trackCenterPosition = vector3 + new Vector3(-0.5f * num2, chassisConfig.NominalRayLength, 0f);
            Vector3 vector6 = vector3 + new Vector3(0.5f * num2, chassisConfig.NominalRayLength, 0f);
            float damping = node.damping.Damping;
            TrackComponent component = new TrackComponent
            {
                LeftTrack = new Track(rigidbody, chassisConfig.NumRaysPerTrack, trackCenterPosition, trackLength, chassisConfig, chassis, -1, damping),
                RightTrack = new Track(rigidbody, chassisConfig.NumRaysPerTrack, vector6, trackLength, chassisConfig, chassis, 1, damping)
            };
            //int layerMask = 0;//LayerMasks.VISIBLE_FOR_CHASSIS_SEMI_ACTIVE;
            //var layerMask = LayerMask.GetMask("Default", "TankBounds");
            component.LeftTrack.SetRayсastLayerMask(node.chassisConfig.trackLayerMask);
            component.RightTrack.SetRayсastLayerMask(node.chassisConfig.trackLayerMask);
            entity.AddComponent(component);
        }

        public void FixedUpdate(ChassisNode chassisNode, TankJumpComponent tankJump, GameTankSettingsComponent gameTankSettings, bool TankMovable)
        {
            if (!(tankJump != null) || !tankJump.isNearBegin())
            {
                bool inputEnabled = true;//inputState.IsPresent();
                if (TankMovable)//chassisNode.Entity.HasComponent<SelfTankComponent>()
                {
                    this.UpdateSelfInput(ref chassisNode, inputEnabled, gameTankSettings.MovementControlsInverted);
                    //chassisManager.preparedMovementEvent.velocity = new Vector3S(chassisNode.rigidbody.Rigidbody.velocity);
                    //chassisManager.preparedMovementEvent.position = (new Vector3S(chassisManager.transform.localPosition)).ConvertToVector3SConstant007UnScaling();
                    //chassisManager.preparedMovementEvent.rotation = new QuaternionS(chassisManager.transform.localRotation);
                }
                this.UpdateInput(chassisNode, inputEnabled);
                this.UpdateTrackMoveAnimation(chassisNode);
                ChassisSmootherComponent chassisSmoother = chassisNode.chassisSmoother;
                chassisSmoother.maxSpeedSmoother.SetTargetValue(chassisNode.speed.Speed);
                float num = chassisSmoother.maxSpeedSmoother.Update(Time.fixedDeltaTime);
                chassisNode.effectiveSpeed.MaxSpeed = num;
                Rigidbody rigidbody = chassisNode.rigidbody.Rigidbody;
                if (rigidbody)
                {
                    float x = rigidbody.velocity.x;
                    float z = rigidbody.velocity.z;
                    float t = (!tankJump.OnFly) ? 1f : tankJump.GetSlowdownLerp();
                    if (((x * x) + (z * z)) > (num * num))
                    {
                        float num5 = Mathf.Lerp(1f, num / ((float)Math.Sqrt((double)((x * x) + (z * z)))), t);
                        Vector3 velocity = new Vector3(rigidbody.velocity.x * num5, rigidbody.velocity.y, rigidbody.velocity.z * num5);
                        rigidbody.SetVelocitySafe(velocity);
                    }
                    chassisSmoother.maxTurnSpeedSmoother.SetTargetValue(chassisNode.speed.TurnSpeed);
                    chassisNode.effectiveSpeed.MaxTurnSpeed = chassisSmoother.maxTurnSpeedSmoother.Update(Time.fixedDeltaTime);
                    this.AdjustSuspensionSpringCoeff(chassisNode.chassisConfig, chassisNode.chassis, chassisNode.rigidbody.Rigidbody);
                    float updatePeriod = 0f;
                    if (!(tankJump != null))
                    {
                        updatePeriod = chassisNode.cameraVisibleTrigger.IsVisible ? 0.05f : 0.1f;
                        updatePeriod += UnityEngine.Random.value * 0.05f;
                    }
                    if (this.UpdateSuspensionContacts(chassisNode.track, Time.fixedDeltaTime, updatePeriod) && tankJump != null)
                    {
                        tankJump.FinishAndSlowdown();
                    }
                    this.ApplyMovementForces(chassisNode, Time.fixedDeltaTime);
                    this.ApplyStaticFriction(chassisNode.track, chassisNode.rigidbody.Rigidbody);
                    var contactFactor = (float)(chassisNode.track.LeftTrack.numContacts + chassisNode.track.RightTrack.numContacts) / (chassisNode.chassisConfig.NumRaysPerTrack * 2f);
                    rigidbody.drag = chassisNode.rigidbody.cacheRigidbodyDrag * contactFactor;
                    rigidbody.angularDrag = chassisNode.rigidbody.cacheRigidbodyAngularDrag * contactFactor;
                    //this.ApplySideDamper(chassisNode, Time.fixedDeltaTime);
                }
            }
        }

        public Vector2 TrackTextureOffset = Vector2.zero;

        private void UpdateTrackMoveAnimation(ChassisNode chassisNode)
        {
            if(chassisNode.chassis.EffectiveMoveAxis != 0f)
            {
            }
            if (this.chassisManager.parentHullManager != null)
            {
                chassisManager.parentHullManager.parentTankManager.hullVisualController.MoveAnimation(chassisNode.chassis.EffectiveMoveAxis, 0);
            }
            else //for test purposes
            {
                this.chassisManager.GetComponent<MeshRenderer>().materials[1].SetTextureOffset("_Lightmap", TrackTextureOffset);
                this.chassisManager.GetComponent<MeshRenderer>().materials[1].SetTextureOffset("_Details", TrackTextureOffset);
            }
        }
        private Vector3 GetContactsMidpoint(ChassisConfigComponent chassisConfig, TrackComponent tracks)
        {
            Vector3 vector = new Vector3();
            for (int i = 0; i < chassisConfig.NumRaysPerTrack; i++)
            {
                SuspensionRay ray = tracks.LeftTrack.rays[i];
                if (ray.hasCollision)
                {
                    vector += ray.rayHit.point;
                }
                ray = tracks.RightTrack.rays[i];
                if (ray.hasCollision)
                {
                    vector += ray.rayHit.point;
                }
            }
            int num2 = tracks.LeftTrack.numContacts + tracks.RightTrack.numContacts;
            return ((num2 != 0) ? (vector / ((float)num2)) : Vector3.zero);
        }

        public void InitTankChassis(ChassisInitNode node)
        {
            ChassisComponent chassis = new ChassisComponent();
            this.CreateTracks(node, chassis);
            node.Entity.AddComponent(chassis);
            node.Entity.AddComponent<EffectiveSpeedComponent>();
            ChassisSmootherComponent component = new ChassisSmootherComponent();
            component.maxSpeedSmoother.Reset(node.speed.Speed);
            component.maxTurnSpeedSmoother.Reset(node.speed.TurnSpeed);
            node.Entity.AddComponent(component);
            node.rigidbody.Rigidbody.mass = node.weight.Weight;
        }

        private bool IsReversedMove(float moveDirection, float relativeMovementSpeed) =>
            (moveDirection * relativeMovementSpeed) < 0f;

        private bool IsReversedTurn(float turnDirection, float relativeTurnSpeed) =>
            (turnDirection * relativeTurnSpeed) < 0f;

        private float RecalculateRelativeTurnSpeed(ChassisNode node, float dt, float maxTurnSpeed, float relativeTurnSpeed, float slopeCoeff)
        {
            ChassisComponent chassis = node.chassis;
            ChassisConfigComponent chassisConfig = node.chassisConfig;
            float num = node.speedConfig.TurnAcceleration * 0.01745329f;
            float num2 = this.CalculateTurnCoefficient(node.track);
            float num3 = 0f;
            if (chassis.EffectiveTurnAxis == 0f)
            {
                num3 = ((-MathUtil.Sign(relativeTurnSpeed) * num) * slopeCoeff) * dt;
                if (MathUtil.Sign(relativeTurnSpeed) != MathUtil.Sign(relativeTurnSpeed + num3))
                {
                    num3 = -relativeTurnSpeed;
                }
            }
            else
            {
                if (this.IsReversedTurn(chassis.EffectiveTurnAxis, relativeTurnSpeed))
                {
                    num = node.speedConfig.ReverseTurnAcceleration * 0.01745329f;
                }
                num3 = ((chassis.EffectiveTurnAxis * num) * slopeCoeff) * dt;
                if ((chassis.EffectiveMoveAxis == -1f) && chassisConfig.ReverseBackTurn)
                {
                    num3 = -num3;
                }
            }
            return Mathf.Clamp((float)(relativeTurnSpeed + num3), (float)(-maxTurnSpeed * num2), (float)(maxTurnSpeed * num2));
        }

        public void ResetTankChassis(SingleNode<ChassisSmootherComponent> smoother, SingleNode<SpeedComponent> speed)
        {
            smoother.component.maxSpeedSmoother.Reset(speed.component.Speed);
            smoother.component.maxTurnSpeedSmoother.Reset(speed.component.TurnSpeed);
        }

        public void SetTankCollisionLayerMask(TankActiveStateNode node)
        {
            int layerMask = LayerMasks.VISIBLE_FOR_CHASSIS_ACTIVE;
            node.track.LeftTrack.SetRayсastLayerMask(layerMask);
            node.track.RightTrack.SetRayсastLayerMask(layerMask);
        }

        private void UpdateInput(ChassisNode tank, bool inputEnabled)
        {
            ChassisComponent chassis = tank.chassis;
            bool flag = true;//tank.Entity.HasComponent<TankMovableComponent>();
            chassis.EffectiveMoveAxis = !flag ? 0f : chassis.MoveAxis;
            chassis.EffectiveTurnAxis = !flag ? 0f : chassis.TurnAxis;
        }

        public ChassisNode UpdateSelfInput(ref ChassisNode tank, bool inputEnabled, bool inverse)
        {
            ChassisComponent chassis = tank.chassis;
            //float x = !inputEnabled ? 0f : (InputManager.GetUnityAxis(RIGHT_AXIS) - InputManager.GetUnityAxis(LEFT_AXIS));
            //float y = !inputEnabled ? 0f : (InputManager.GetUnityAxis(FORWARD_AXIS) - InputManager.GetUnityAxis(BACKWARD_AXIS));
            float x = !ClientInitService.instance.LockInput && inputEnabled ? (Input.GetAxis("Right") - Input.GetAxis("Left")) : 0f;
            float y = !ClientInitService.instance.LockInput && inputEnabled ? (Input.GetAxis("Backward") - Input.GetAxis("Forward")) : 0f;
            if (inverse && (y < 0f))
            {
                x *= -1f;
            }
            Vector2 vector = new Vector2(chassis.TurnAxis, chassis.MoveAxis);
            Vector2 vector2 = new Vector2(x, y);
            if (vector2 != vector)
            {
                chassis.TurnAxis = x;
                chassis.MoveAxis = y;
                bool flag = true;// tank.Entity.HasComponent<TankMovableComponent>();
                chassis.EffectiveMoveAxis = !flag ? 0f : chassis.MoveAxis;
                chassis.EffectiveTurnAxis = !flag ? 0f : chassis.TurnAxis;
                //base.ScheduleEvent<ChassisControlChangedEvent>(tank);
            }
            return tank;
        }

        private bool UpdateSuspensionContacts(TrackComponent trackComponent, float dt, float updatePeriod)
        {
            bool flag2 = trackComponent.RightTrack.UpdateSuspensionContacts(dt, updatePeriod);
            return (trackComponent.LeftTrack.UpdateSuspensionContacts(dt, updatePeriod) && flag2);
        }

        //public static Tanks.Battle.ClientCore.Impl.InputManager InputManager { get; set; }

        public class ChassisInitNode : Node
        {
            public RigidbodyComponent rigidbody;
            public ChassisConfigComponent chassisConfig;
            public TankCollidersComponent tankColliders;
            public SpeedComponent speed;
            public WeightComponent weight;
            public DampingComponent damping;
        }

        //[Not(typeof(TankDeadStateComponent))]
        public class ChassisNode : Node
        {
            public TankGroupComponent tankGroup;
            public RigidbodyComponent rigidbody;
            public ChassisConfigComponent chassisConfig;
            public ChassisComponent chassis;
            public TankCollidersComponent tankColliders;
            public SpeedComponent speed;
            public EffectiveSpeedComponent effectiveSpeed;
            public TrackComponent track;
            public ChassisSmootherComponent chassisSmoother;
            public SpeedConfigComponent speedConfig;
            public CameraVisibleTriggerComponent cameraVisibleTrigger;
        }

        public class TankActiveStateNode : Node
        {
            public TrackComponent track;
            public TankActiveStateComponent tankActiveState;
        }
    }
}
