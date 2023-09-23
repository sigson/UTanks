using SecuredSpace.Battle.Tank;
using SecuredSpace.Battle.Tank.Turret;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient.ECS.Components.Battle.Tank;
using UTanksClient.Extensions;

namespace SecuredSpace.Important.Aim
{
    public class RaycastAIM : MonoBehaviour
    {
        private float verticalUpAngle = 10;
        public float VerticalUpAngle {
            get {
                return verticalUpAngle;
            }
            set {
                verticalUpAngle = value;
                InitializeAIM();
            } 
        }
        private float verticalDownAngle = 15;
        public float VerticalDownAngle
        {
            get
            {
                return verticalDownAngle;
            }
            set
            {
                verticalDownAngle = value;
                InitializeAIM();
            }
        }
        public float HorizontalAngle = 0;
        public float pixelPerRayForChecking = 20;
        public GameObject parentTankManager;
        public float rayPerAngle = 10;
        public double Distance = 0;
        public bool CloseRangeWeapon;
        public bool MultitargetsWeapon;
        public int layerMask;
        public Vector3 upCross;
        public GameObject TestModePlayersList;
        public Material LeftMaterial;
        public Material RightMaterial;

        public void Start()
        {
            layerMask = LayerMask.GetMask("Default", "TankBounds", "Tank");
            InitializeAIM();
        }

        public void InitializeAIM()
        {
            var muzzlePoints = parentTankManager.GetComponent<TankManager>().MuzzlePoints;
            for(int i = 0; i < muzzlePoints.Count; i++)
            {
                var muzPoint = muzzlePoints[i];
                var rootAimRails = muzPoint.GetComponent<MuzzlePoint>().rootAimRailsObject;
                var topAimRail = muzPoint.GetComponent<MuzzlePoint>().topAimRail;
                var bottomAimRail = muzPoint.GetComponent<MuzzlePoint>().bottomAimRail;
                if (topAimRail != null)
                {
                    topAimRail.transform.localPosition = new Vector3(0, 0, -5f);
                    rootAimRails.transform.localRotation = Quaternion.Euler(VerticalUpAngle, 0, 0);
                    var cachePosition = topAimRail.transform.position;
                    rootAimRails.transform.localRotation = Quaternion.identity;
                    topAimRail.transform.position = cachePosition;
                }
                if (bottomAimRail != null)
                {
                    bottomAimRail.transform.localPosition = new Vector3(0, 0, -5f);
                    rootAimRails.transform.localRotation = Quaternion.Euler(VerticalDownAngle * -1f, 0, 0);
                    var cachePosition = bottomAimRail.transform.position;
                    rootAimRails.transform.localRotation = Quaternion.identity;
                    bottomAimRail.transform.position = cachePosition;
                }
            }
        }
        public List<(TankManager, Vector3)> FindPossibleTargetsFUCK(TankManager shooter)
        {
            List<Vector3> directions = new List<Vector3>();
            for (int i = 0; i < pixelPerRayForChecking / 2; i++)
            {
                for (int i2 = 0; i2 < rayPerAngle + 1; i2++)
                {
                    directions.Add(Vector3.Lerp(shooter.MuzzlePoint.transform.forward * -1, Quaternion.AngleAxis(i, Vector3.forward) * shooter.MuzzlePoint.transform.forward * -1, (i2) / rayPerAngle));
                    directions.Add(Vector3.Lerp(shooter.MuzzlePoint.transform.forward * -1, Quaternion.AngleAxis(i * -1, Vector3.forward) * shooter.MuzzlePoint.transform.forward * -1, (rayPerAngle - i2) / rayPerAngle));
                }

            }

            //directions.Add()
            //while()
            RaycastHit hitInfo;
            directions.ForEach((direction) => Physics.Raycast(shooter.MuzzlePoint.transform.position, direction, out hitInfo, Mathf.Infinity));
            directions.ForEach((direction) => Debug.DrawRay(shooter.MuzzlePoint.transform.position, direction * 100f, Color.white, 5f));
            return new List<(TankManager, Vector3)>();
        }

        public List<(TankManager, Vector3)> FindPossibleTargetsEEEROCK(TankManager shooter)
        {
            List<(TankManager, Vector3)> targets = new List<(TankManager, Vector3)>();

            Dictionary<long, TankManager> battleTanksDB = null;

            if (shooter.TestingMode)
            {
                battleTanksDB = new Dictionary<long, TankManager>();
                for (int i = 0; i < TestModePlayersList.transform.childCount; i++)
                    battleTanksDB.Add(TestModePlayersList.transform.GetChild(i).gameObject.GetComponent<TankManager>().ManagerEntityId, TestModePlayersList.transform.GetChild(i).gameObject.GetComponent<TankManager>());
            }
            else
            {
                //battleTanksDB = ClientInitService.instance.battleManager.BattleTanksDB;
            }

            foreach (var tankManager in battleTanksDB.Values)
            {
                if (tankManager.Ghost || tankManager.checkGhost)
                    continue;
                if (tankManager.HullAngleColliders.Count < 8)
                {
                    tankManager.TestRebuildHullAngleColliders = true;
                }
                if (!shooter.TestingMode && (tankManager.ManagerEntityId == shooter.ManagerEntityId || tankManager.ManagerEntity.HasComponent(TankDeadStateComponent.Id) ||tankManager.ManagerEntity.HasComponent(TankNewStateComponent.Id)))
                    continue;
                Dictionary<string, Vector3> colliderPointsAngle = new Dictionary<string, Vector3>();
                float minimalVertical = 3600f;
                foreach (var colliderPoint in tankManager.HullAngleColliders)
                {
                    var targetAngle = TargetAngle(shooter.MuzzlePoint, colliderPoint.Value);
                    colliderPointsAngle.Add(colliderPoint.Key, targetAngle);
                    if (minimalVertical > targetAngle.z)
                    {
                        minimalVertical = targetAngle.z;
                    }
                    //Ray ray = new Ray()
                    //{
                    //    direction = colliderPoint,
                    //    origin = 
                    //};
                    //RaycastHit hit;

                    //if (Physics.Linecast(shooter.MuzzlePoint.transform.position, colliderPoint.transform.position, 13))
                    //{

                    //}
                }

                if (minimalVertical > VerticalUpAngle)
                    return targets;
                var hullCenterAngle = TargetAngle(shooter.MuzzlePoint, tankManager.Hull);
                var sides = GetMaxFromSide(hullCenterAngle, colliderPointsAngle, ref tankManager.HullAngleColliders);
                var leftside = false;
                var rightside = false;
                if (sides.Item1 == "" || sides.Item2 == "")
                    continue;
                //Debug.DrawLine(shooter.MuzzlePoint.transform.position, tankManager.HullAngleColliders[sides.Item1].transform.position, Color.blue, 1f);
                //Debug.DrawLine(shooter.MuzzlePoint.transform.position, tankManager.HullAngleColliders[sides.Item2].transform.position, Color.red, 1f);
                //if (colliderPointsAngle[sides.Item1].x < colliderPointsAngle[sides.Item2].x)
                if (Mathf.Abs(colliderPointsAngle[sides.Item1].x) > Mathf.Abs(colliderPointsAngle[sides.Item2].x))
                    leftside = true;
                else
                    rightside = true;

                List<Vector3> WPoints = new List<Vector3>();
                var shooterMuzzlePos = shooter.MuzzlePoint.transform.position;
        //        WPoints.IfThenElse(
        //() => V,
        //e => e.OrderBy(w => w.Id),
        //e => e.OrderByDescending(w => w.Id));

                if (sides.Item1 != "" && sides.Item2 != "")
                {
                    float sumOfAngles = 0;
                    float sideAngle = 0;
                    var forwardNormalized = tankManager.MuzzlePoint.transform.forward;
                    forwardNormalized = new Vector3(forwardNormalized.x, 0f, forwardNormalized.z);
                    var rightNormalized = tankManager.MuzzlePoint.transform.right;
                    rightNormalized = new Vector3(rightNormalized.x, 0f, rightNormalized.z);
                    var muzzleHorPosition = tankManager.HullAngleColliderHeader.transform.InverseTransformPoint(new Vector3(shooter.MuzzlePoint.transform.position.x, shooter.MuzzlePoint.transform.position.y, shooter.MuzzlePoint.transform.position.z));
                    //forwardNormalized += rightNormalized;
                    if (leftside)
                    {
                        sumOfAngles = 180 - Mathf.Abs(colliderPointsAngle[sides.Item1].x) + 180 - Mathf.Abs(colliderPointsAngle[sides.Item2].x) + 180 - Mathf.Abs(hullCenterAngle.x);
                        sideAngle = 180 - Mathf.Abs(colliderPointsAngle[sides.Item1].x);
                        var localPointOfAngleCalcMuzzleShotPoint = Vector3.Lerp(tankManager.HullAngleColliders[sides.Item1].transform.localPosition, tankManager.HullAngleColliders[sides.Item2].transform.localPosition, sideAngle / sumOfAngles);
                        var distanceFloat = Vector3.Distance(tankManager.HullAngleColliders["LeftBackwardTop"].transform.localPosition, tankManager.HullAngleColliders["RightForwardTop"].transform.localPosition) / pixelPerRayForChecking;


                        List<Vector3> Points = new List<Vector3>();
                        Vector3 newSpot = Vector3.MoveTowards(localPointOfAngleCalcMuzzleShotPoint, muzzleHorPosition, distanceFloat);
                        //Vector3 newSpot = localPointOfAngleCalcMuzzleShotPoint + (forwardNormalized.normalized  * distanceFloat);
                        Points.Add(newSpot);
                        while (Points.Count < pixelPerRayForChecking / 2)
                        {
                            Points.Add(Vector3.MoveTowards(Points[Points.Count - 1], muzzleHorPosition, distanceFloat));
                        }
                        Vector3 newSpotY = Vector3.MoveTowards(localPointOfAngleCalcMuzzleShotPoint, muzzleHorPosition * -1, distanceFloat);
                        Points.Add(newSpotY);
                        while (Points.Count < pixelPerRayForChecking / 2)
                        {
                            Points.Add(Vector3.MoveTowards(Points[Points.Count - 1], muzzleHorPosition * -1, distanceFloat));
                        }

                        foreach (var point in Points)
                        {
                            WPoints.Add(tankManager.HullAngleColliderHeader.transform.TransformPoint(point));
                        }
                        //next for half all pixel per ray rays + generate rays for vertical drill
                        //Vector3 newSpot2 = newSpot + ((tankManager.MuzzlePoint.transform.forward.normalized * -1) * distanceFloat); //next half for forward
                    }
                    else
                    {
                        sumOfAngles = 180 - Mathf.Abs(colliderPointsAngle[sides.Item2].x) + 180 - Mathf.Abs(colliderPointsAngle[sides.Item1].x) + 180 - Mathf.Abs(hullCenterAngle.x);
                        sideAngle = 180 - Mathf.Abs(colliderPointsAngle[sides.Item2].x);
                        var localPointOfAngleCalcMuzzleShotPoint = Vector3.Lerp(tankManager.HullAngleColliders[sides.Item2].transform.localPosition, tankManager.HullAngleColliders[sides.Item1].transform.localPosition, sideAngle / sumOfAngles);

                        var distanceFloat = Vector3.Distance(tankManager.HullAngleColliders["LeftBackwardTop"].transform.localPosition, tankManager.HullAngleColliders["RightForwardTop"].transform.localPosition) / pixelPerRayForChecking;
                        List<Vector3> Points = new List<Vector3>();



                        Vector3 newSpot = Vector3.MoveTowards(localPointOfAngleCalcMuzzleShotPoint, muzzleHorPosition, distanceFloat);
                        //Vector3 newSpot = localPointOfAngleCalcMuzzleShotPoint + (forwardNormalized.normalized  * distanceFloat);
                        Points.Add(newSpot);
                        while (Points.Count < pixelPerRayForChecking / 2)
                        {
                            Points.Add(Vector3.MoveTowards(Points[Points.Count - 1], muzzleHorPosition, distanceFloat));
                        }
                        Vector3 newSpotY = Vector3.MoveTowards(localPointOfAngleCalcMuzzleShotPoint, muzzleHorPosition * -1, distanceFloat);
                        Points.Add(newSpotY);
                        while (Points.Count < pixelPerRayForChecking / 2)
                        {
                            Points.Add(Vector3.MoveTowards(Points[Points.Count - 1], muzzleHorPosition * -1, distanceFloat));
                        }

                        foreach (var point in Points)
                        {
                            WPoints.Add(tankManager.HullAngleColliderHeader.transform.TransformPoint(point));
                        }

                        //Vector3 newSpot = localPointOfAngleCalcMuzzleShotPoint + (forwardNormalized.normalized * distanceFloat);
                        //Points.Add(newSpot);
                        //while (Points.Count < pixelPerRayForChecking / 2)
                        //{
                        //    Points.Add(Points[Points.Count - 1] + (forwardNormalized.normalized * distanceFloat));
                        //}
                        //Vector3 newSpotY = localPointOfAngleCalcMuzzleShotPoint + (forwardNormalized.normalized * -1 * distanceFloat);
                        //Points.Add(newSpotY);
                        //while (Points.Count < pixelPerRayForChecking / 2)
                        //{
                        //    Points.Add(Points[Points.Count - 1] + (forwardNormalized.normalized * -1 * distanceFloat));
                        //}

                    }

                    //MessageBoxProvider.ShowInfo("shot", "");
                }
                WPoints = WPoints.OrderBy(x => Vector3.Distance(x, shooterMuzzlePos)).ToList();
                bool shot = false;
                WPoints.ForEach((WPoint) => {
                    RaycastHit hitInfo;
                    if (!shot && Physics.Linecast(shooter.MuzzlePoint.transform.position, WPoint, out hitInfo, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        IManagableAnchor manager = hitInfo.collider.gameObject.GetComponent<IManagableAnchor>();
                        if (manager != null && manager.ownerManager<TankManager>().ManagerEntityId == tankManager.ManagerEntityId)
                        {
                            if(MultitargetsWeapon)
                            {
                                Ray ray = new Ray(shooter.MuzzlePoint.transform.position, (WPoint - shooter.MuzzlePoint.transform.position).normalized);
                                var collidersInfo = Physics.RaycastAll(ray, 549263.7f, layerMask, QueryTriggerInteraction.Ignore).ToList().OrderBy(x => x.distance).ToList();
                                HashSet<TankManager> tankManagersSet = new HashSet<TankManager>();
                                foreach(var colliderHit in collidersInfo)
                                {
                                    if(colliderHit.collider.gameObject.tag == "Ground")
                                    {
                                        targets.Add((null, colliderHit.point));
                                        break;
                                    }
                                    else
                                    {
                                        IManagableAnchor tManager = colliderHit.collider.gameObject.GetComponent<IManagableAnchor>();
                                        if(tManager != null)
                                        {
                                            if (!tankManagersSet.Contains(tManager.ownerManager<TankManager>()))
                                            {
                                                tankManagersSet.Add(tManager.ownerManager<TankManager>());
                                                targets.Add((tManager.ownerManager<TankManager>(), colliderHit.point));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                targets.Add((tankManager, hitInfo.point));
                            shot = true;
                        }

                    }
                });
                WPoints.ForEach((WPoint) => Debug.DrawLine(shooter.MuzzlePoint.transform.position, WPoint, Color.white, 0.5f));
            }


            return targets;
        }

        public Vector3 TargetAngle(GameObject MuzzleObject, GameObject TargetObject)
        {
            var cacheMuzzleRotation = MuzzleObject.transform.localRotation;
            var cacheMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var cacheMuzzlePosition = MuzzleObject.transform.localPosition;
            var cacheForward = MuzzleObject.transform.forward;
            var cacheRight = MuzzleObject.transform.right;
            var cacheUp = MuzzleObject.transform.up;
            MuzzleObject.transform.LookAt(TargetObject.transform);
            var lookMuzzleRotation = MuzzleObject.transform.localRotation;
            var lookMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var lookMuzzlePosition = MuzzleObject.transform.localPosition;
            var lookMuzzleForward = MuzzleObject.transform.forward;
            var lookMuzzleRight = MuzzleObject.transform.right;
            cacheRight.y = 0;
            lookMuzzleRight.y = 0;
            cacheForward.y = 0;
            lookMuzzleForward.y = 0;
            var lookMuzzleUp = MuzzleObject.transform.up;
            MuzzleObject.transform.localRotation = cacheMuzzleRotation;
            MuzzleObject.transform.localPosition = cacheMuzzlePosition;


            //var anglesHorizontal = Vector3.Angle(cacheForward, lookMuzzleForward);// + (cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f));
            //var anglesHorizontal2 = Vector3.Angle(cacheRight, lookMuzzleRight);
            //var anglesVertical = Vector3.Angle(cacheUp, lookMuzzleUp);
            //var middler = Mathf.Abs(anglesHorizontal2 - anglesHorizontal) / 2;
            //var middledAnglesHorizontal2 = (anglesHorizontal > anglesHorizontal2 ? anglesHorizontal2 + middler : anglesHorizontal2 - middler);
            //var middledAnglesHorizontal = (anglesHorizontal2 > anglesHorizontal ? anglesHorizontal + middler : anglesHorizontal - middler);


            var anglesHorizontal = Vector3.Angle(cacheForward, lookMuzzleForward);
            var anglesHorizontal2 = Vector3.Angle(cacheRight, lookMuzzleRight);
            var anglesVertical = Vector3.Angle(cacheUp, lookMuzzleUp);
            var middler = Mathf.Abs(Mathf.Abs(anglesHorizontal2) - Mathf.Abs(anglesHorizontal)) / 2;

            float absAnglesHorizontal = Mathf.Abs(anglesHorizontal);
            float absAnglesHorizontal2 = Mathf.Abs(anglesHorizontal2);

            float middledAnglesHorizontal = 0;
            float middledAnglesHorizontal2 = 0;

            var middledABSAnglesHorizontal2 = (absAnglesHorizontal > absAnglesHorizontal2 ? absAnglesHorizontal2 + middler : absAnglesHorizontal2 - middler);
            var middledABSAnglesHorizontal = (absAnglesHorizontal2 > absAnglesHorizontal ? absAnglesHorizontal + middler : absAnglesHorizontal - middler);

            if (anglesHorizontal >= 0)
                middledAnglesHorizontal = middledABSAnglesHorizontal;
            else
                middledAnglesHorizontal = middledABSAnglesHorizontal * -1;

            if (anglesHorizontal2 >= 0)
                middledAnglesHorizontal2 = middledABSAnglesHorizontal2;
            else
                middledAnglesHorizontal2 = middledABSAnglesHorizontal2 * -1;

            var forwardCross = Vector3.Cross(cacheForward, lookMuzzleForward);
            var rightCross = Vector3.Cross(cacheRight, lookMuzzleRight);
            upCross = Vector3.Cross(cacheUp, lookMuzzleUp);

            middledAnglesHorizontal = (forwardCross.y >= 0 ? middledAnglesHorizontal : middledAnglesHorizontal * -1);
            middledAnglesHorizontal2 = (rightCross.y >= 0 ? middledAnglesHorizontal2 : middledAnglesHorizontal2 * -1);
            //middledAnglesHorizontal = (upCross.y >= 0 ? middledAnglesHorizontal : middledAnglesHorizontal * -1);

            return new Vector3(middledAnglesHorizontal, middledAnglesHorizontal2, anglesVertical);
        }

        public (string, string) GetMaxFromSide(Vector3 hullCenterAngle, Dictionary<string, Vector3> colliderPointsAngle, ref Dictionary<string, GameObject> colliderPoints)//side -1 - left, 1 - right
        {
            string leftSide = "";
            string rightSide = "";
            Vector3 MaxLeft = new Vector3(180, 180, 180);
            Vector3 MaxRight = new Vector3(180, 180, 180);
            int nullCounter = 0;
            foreach (var colliderPointAngle in colliderPointsAngle)
            {
                if (Mathf.Abs(colliderPointAngle.Value.x) < 80 || Mathf.Abs(colliderPointAngle.Value.y) < 80)
                    nullCounter++;
                if ((colliderPointAngle.Value.x < 0 && colliderPointAngle.Value.y < 0) || (colliderPointAngle.Value.x < 0 && colliderPointAngle.Value.y > 0))
                {
                    if (Mathf.Abs(MaxLeft.x) > Mathf.Abs(colliderPointAngle.Value.x))
                    {
                        leftSide = colliderPointAngle.Key;
                        MaxLeft = colliderPointAngle.Value;
                    }
                    //colliderPoints[colliderPointAngle.Key].GetComponent<MeshRenderer>().material = LeftMaterial;
                }
                if ((colliderPointAngle.Value.x > 0 && colliderPointAngle.Value.y > 0) || (colliderPointAngle.Value.x > 0 && colliderPointAngle.Value.y < 0))
                {
                    if (Mathf.Abs(MaxRight.x) > Mathf.Abs(colliderPointAngle.Value.x))
                    {
                        rightSide = colliderPointAngle.Key;
                        MaxRight = colliderPointAngle.Value;
                    }
                    //colliderPoints[colliderPointAngle.Key].GetComponent<MeshRenderer>().material = RightMaterial;
                }
            }
            if (nullCounter == colliderPointsAngle.Count)
                return ("", "");
            return (rightSide, leftSide);
        }


        public List<(TankManager, Vector3)> FindPossibleTargets(TankManager shooter)
        {
            List<Vector3> directions = new List<Vector3>();
            List<(Vector3, Vector3)> rawDirections = new List<(Vector3, Vector3)>();
            var shooterMuzzlePos = shooter.MuzzlePoint.transform.position;
            var muzzPointScript = shooter.MuzzlePoint.GetComponent<MuzzlePoint>();
            var topAimRail = muzzPointScript.topAimRail;
            var bottomAimRail = muzzPointScript.bottomAimRail;
            var targets = new Dictionary<long, (TankManager, Vector3)>();
            var rays = rayPerAngle * (VerticalUpAngle + VerticalDownAngle);
            for (int i2 = 0; i2 < rays; i2++)
            {
                var preDirection = topAimRail.transform.position + (i2 / rays) * (bottomAimRail.transform.position - topAimRail.transform.position);
                rawDirections.Add((preDirection, (preDirection - shooterMuzzlePos).normalized));
            }
            rawDirections.OrderBy(x => Vector3.Distance(x.Item1, shooterMuzzlePos)).ToList().ForEach(x => directions.Add(x.Item2));
            //directions.Add()
            //while()
            bool shot = false;
            directions.ForEach((direction) => {
                RaycastHit hitInfo = new RaycastHit();
                if ((!shot || MultitargetsWeapon) && Physics.Raycast(shooterMuzzlePos, direction, out hitInfo, Const.MaxUnityFloatValue, layerMask, QueryTriggerInteraction.Ignore))
                {
                    IManagableAnchor manager = hitInfo.collider.gameObject.GetComponent<IManagableAnchor>();
                    if (manager != null)
                    {
                        if (MultitargetsWeapon)
                        {
                            Ray ray = new Ray(shooter.MuzzlePoint.transform.position, (hitInfo.point - shooter.MuzzlePoint.transform.position).normalized);
                            var collidersInfo = Physics.RaycastAll(ray, 549263.7f, layerMask, QueryTriggerInteraction.Ignore).ToList().OrderBy(x => x.distance).ToList();
                            HashSet<TankManager> tankManagersSet = new HashSet<TankManager>();
                            int countTargets = 0;
                            foreach (var colliderHit in collidersInfo)
                            {
                                if (colliderHit.collider.gameObject.tag == "Ground")
                                {
                                    if((targets.ContainsKey(0) && targets.Count - 1 < countTargets) || !targets.ContainsKey(0))
                                        targets[0] = ((null, colliderHit.point));
                                    break;
                                }
                                else
                                {
                                    IManagableAnchor tManager = colliderHit.collider.gameObject.GetComponent<IManagableAnchor>();
                                    if (tManager != null)
                                    {
                                        if (!tankManagersSet.Contains(tManager.ownerManager<TankManager>()))
                                        {
                                            tankManagersSet.Add(tManager.ownerManager<TankManager>());
                                            targets[tManager.ownerManager<TankManager>().ManagerEntityId] = ((tManager.ownerManager<TankManager>(), colliderHit.point));
                                            countTargets++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(!targets.ContainsKey(manager.ownerManager<TankManager>().ManagerEntityId))
                                targets[manager.ownerManager<TankManager>().ManagerEntityId] = ((manager.ownerManager<TankManager>(), hitInfo.point));
                        }
                        shot = true;
                    }

                }
            });
            #if DEBUG
            directions.ForEach((direction) => Debug.DrawRay(shooter.MuzzlePoint.transform.position, direction * 100f, Color.white, 5f));
            #endif
            (TankManager, Vector3) groundpoint;
            if (targets.TryGetValue(0, out groundpoint))
            {
                targets.Remove(0);
                var rettTargets = targets.Values.ToList();
                rettTargets.Add(groundpoint);
                return rettTargets;
            }
            var retTargets = targets.Values.ToList();
            return retTargets;
        }




        public Vector3 TargetAngle22(GameObject MuzzleObject, GameObject TargetObject)
        {
            var cacheMuzzleRotation = MuzzleObject.transform.localRotation;
            var cacheMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var cacheMuzzlePosition = MuzzleObject.transform.localPosition;
            var cacheForward = MuzzleObject.transform.forward;
            var cacheRight = MuzzleObject.transform.right;
            var cacheUp = MuzzleObject.transform.up;
            MuzzleObject.transform.LookAt(TargetObject.transform);
            var lookMuzzleRotation = MuzzleObject.transform.localRotation;
            var lookMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var lookMuzzlePosition = MuzzleObject.transform.localPosition;
            var lookMuzzleForward = MuzzleObject.transform.forward;
            var lookMuzzleRight = MuzzleObject.transform.right;
            var lookMuzzleUp = MuzzleObject.transform.up;
            MuzzleObject.transform.localRotation = cacheMuzzleRotation;
            MuzzleObject.transform.localPosition = cacheMuzzlePosition;
            var preAnglesHorizontal = Vector3.SignedAngle(cacheForward, lookMuzzleForward, Vector3.forward);
            float anglesHorizontal = 0;
            if (preAnglesHorizontal >= 0)
            {
                anglesHorizontal = preAnglesHorizontal + ((cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f)));
            }
            else
            {
                anglesHorizontal = preAnglesHorizontal + ((cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f)) * -1);
            }
            //var anglesHorizontal234235 = Vector3.SignedAngle(cacheForward, lookMuzzleForward, Vector3.forward) + (cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f));
            var anglesHorizontal2 = Vector3.SignedAngle(cacheRight, lookMuzzleRight, Vector3.right);
            var anglesVertical = Vector3.SignedAngle(cacheUp, lookMuzzleUp, Vector3.up);
            var middler = (Mathf.Abs(anglesHorizontal2) - Mathf.Abs(anglesHorizontal)) / 2;

            float middledAnglesHorizontal = 0;
            float middledAnglesHorizontal2 = 0;
            if (anglesHorizontal2 >= 0)
                middledAnglesHorizontal2 = (Mathf.Abs(anglesHorizontal) > Mathf.Abs(anglesHorizontal2) ? anglesHorizontal2 + middler : anglesHorizontal2 - middler);
            else
                middledAnglesHorizontal2 = (Mathf.Abs(anglesHorizontal) > Mathf.Abs(anglesHorizontal2) ? anglesHorizontal2 - middler : anglesHorizontal2 + middler);

            if (anglesHorizontal >= 0)
                middledAnglesHorizontal = (Mathf.Abs(anglesHorizontal2) > Mathf.Abs(anglesHorizontal) ? anglesHorizontal + middler : anglesHorizontal - middler);
            else
                middledAnglesHorizontal = (Mathf.Abs(anglesHorizontal2) > Mathf.Abs(anglesHorizontal) ? anglesHorizontal - middler : anglesHorizontal + middler);

            return new Vector3(middledAnglesHorizontal, middledAnglesHorizontal2, anglesVertical);
        }

        public void FF()
        {
            //var t = Vector3.Angle(cacheMuzzleEulerRotation, lookMuzzleEulerRotation);
            //var t2 = new Vector3(DegreeMinus(cacheMuzzleEulerRotation.x, lookMuzzleEulerRotation.x), DegreeMinus(cacheMuzzleEulerRotation.y, lookMuzzleEulerRotation.y), DegreeMinus(cacheMuzzleEulerRotation.z, lookMuzzleEulerRotation.z));




            //var preAnglesHorizontal = Vector3.SignedAngle(cacheForward, lookMuzzleForward, Vector3.forward);
            //float XXXXanglesHorizontal = 0;
            //if (preAnglesHorizontal >= 0)
            //{
            //    XXXXanglesHorizontal = preAnglesHorizontal + ((cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f)));
            //}
            //else
            //{
            //    XXXXanglesHorizontal = preAnglesHorizontal + ((cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f)) * -1);
            //}
            ////var anglesHorizontal234235 = Vector3.SignedAngle(cacheForward, lookMuzzleForward, Vector3.forward) + (cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f));
            //var XXXXanglesHorizontal2 = Vector3.SignedAngle(cacheRight, lookMuzzleRight, Vector3.right);
            //var XXXXanglesVertical = Vector3.SignedAngle(cacheUp, lookMuzzleUp, Vector3.up);

            //middledAnglesHorizontal = (XXXXanglesHorizontal >= 0 ? middledAnglesHorizontal : middledAnglesHorizontal * -1);
            //middledAnglesHorizontal2 = (XXXXanglesHorizontal2 >= 0 ? middledAnglesHorizontal2 : middledAnglesHorizontal2 * -1);
            //anglesVertical = (XXXXanglesVertical >= 0 ? anglesVertical : anglesVertical * -1);
        }

        public Vector3 TargetAngle12(GameObject MuzzleObject, GameObject TargetObject)
        {
            var cacheMuzzleRotation = MuzzleObject.transform.localRotation;
            var cacheMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var cacheMuzzlePosition = MuzzleObject.transform.localPosition;
            var cacheForward = MuzzleObject.transform.forward;
            var cacheRight = MuzzleObject.transform.right;
            var cacheUp = MuzzleObject.transform.up;
            MuzzleObject.transform.LookAt(TargetObject.transform);
            var lookMuzzleRotation = MuzzleObject.transform.localRotation;
            var lookMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var lookMuzzlePosition = MuzzleObject.transform.localPosition;
            var lookMuzzleForward = MuzzleObject.transform.forward;
            var lookMuzzleRight = MuzzleObject.transform.right;
            cacheRight.y = 0;
            lookMuzzleRight.y = 0;
            cacheForward.y = 0;
            lookMuzzleForward.y = 0;
            var lookMuzzleUp = MuzzleObject.transform.up;
            MuzzleObject.transform.localRotation = cacheMuzzleRotation;
            MuzzleObject.transform.localPosition = cacheMuzzlePosition;


            var anglesHorizontal = Vector3.Angle(cacheForward, lookMuzzleForward);// + (cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f));
            var anglesHorizontal2 = Vector3.Angle(cacheRight, lookMuzzleRight);
            var anglesVertical = Vector3.Angle(cacheUp, lookMuzzleUp);
            var middler = Mathf.Abs(anglesHorizontal2 - anglesHorizontal) / 2;
            var middledAnglesHorizontal2 = (anglesHorizontal > anglesHorizontal2 ? anglesHorizontal2 + middler : anglesHorizontal2 - middler);
            var middledAnglesHorizontal = (anglesHorizontal2 > anglesHorizontal ? anglesHorizontal + middler : anglesHorizontal - middler);








            return new Vector3(middledAnglesHorizontal, middledAnglesHorizontal2, anglesVertical);
        }


        public Vector3 TargetAngle333(GameObject MuzzleObject, GameObject TargetObject)
        {
            var cacheMuzzleRotation = MuzzleObject.transform.localRotation;
            var cacheMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var cacheMuzzlePosition = MuzzleObject.transform.localPosition;
            var cacheForward = MuzzleObject.transform.forward;
            var cacheRight = MuzzleObject.transform.right;
            var cacheUp = MuzzleObject.transform.up;
            MuzzleObject.transform.LookAt(TargetObject.transform);
            var lookMuzzleRotation = MuzzleObject.transform.localRotation;
            var lookMuzzleEulerRotation = MuzzleObject.transform.eulerAngles;
            var lookMuzzlePosition = MuzzleObject.transform.localPosition;
            var lookMuzzleForward = MuzzleObject.transform.forward;
            var lookMuzzleRight = MuzzleObject.transform.right;
            cacheRight.y = 0;
            lookMuzzleRight.y = 0;
            cacheForward.y = 0;
            lookMuzzleForward.y = 0;
            var lookMuzzleUp = MuzzleObject.transform.up;
            MuzzleObject.transform.localRotation = cacheMuzzleRotation;
            MuzzleObject.transform.localPosition = cacheMuzzlePosition;


            //var anglesHorizontal = Vector3.Angle(cacheForward, lookMuzzleForward);// + (cacheMuzzleEulerRotation.z <= 180 ? cacheMuzzleEulerRotation.z : 180f - (cacheMuzzleEulerRotation.z - 180f));
            //var anglesHorizontal2 = Vector3.Angle(cacheRight, lookMuzzleRight);
            //var anglesVertical = Vector3.Angle(cacheUp, lookMuzzleUp);
            //var middler = Mathf.Abs(anglesHorizontal2 - anglesHorizontal) / 2;
            //var middledAnglesHorizontal2 = (anglesHorizontal > anglesHorizontal2 ? anglesHorizontal2 + middler : anglesHorizontal2 - middler);
            //var middledAnglesHorizontal = (anglesHorizontal2 > anglesHorizontal ? anglesHorizontal + middler : anglesHorizontal - middler);


            var anglesHorizontal = Vector3.SignedAngle(cacheForward, lookMuzzleForward, Vector3.forward);
            var anglesHorizontal2 = Vector3.SignedAngle(cacheRight, lookMuzzleRight, Vector3.right);
            var anglesVertical = Vector3.SignedAngle(cacheUp, lookMuzzleUp, Vector3.up);
            var middler = Mathf.Abs(Mathf.Abs(anglesHorizontal2) - Mathf.Abs(anglesHorizontal)) / 2;

            float absAnglesHorizontal = Mathf.Abs(anglesHorizontal);
            float absAnglesHorizontal2 = Mathf.Abs(anglesHorizontal2);

            float middledAnglesHorizontal = 0;
            float middledAnglesHorizontal2 = 0;

            var middledABSAnglesHorizontal2 = (absAnglesHorizontal > absAnglesHorizontal2 ? absAnglesHorizontal2 + middler : absAnglesHorizontal2 - middler);
            var middledABSAnglesHorizontal = (absAnglesHorizontal2 > absAnglesHorizontal ? absAnglesHorizontal + middler : absAnglesHorizontal - middler);

            if (anglesHorizontal >= 0)
                middledAnglesHorizontal = middledABSAnglesHorizontal;
            else
                middledAnglesHorizontal = middledABSAnglesHorizontal * -1;

            if (anglesHorizontal2 >= 0)
                middledAnglesHorizontal2 = middledABSAnglesHorizontal2;
            else
                middledAnglesHorizontal2 = middledABSAnglesHorizontal2 * -1;

            Vector3.Cross(cacheRight, lookMuzzleRight);

            return new Vector3(middledAnglesHorizontal, middledAnglesHorizontal2, anglesVertical);
        }

        public float DegreePlus(float a, float b)
        {
            if (a + b > 360)
            {
                return 360 - a + b;
            }
            return a + b;
        }

        public float DegreeMinus(float a, float b)
        {
            if (a < b)
            {
                return 360 + a - b;
            }
            return a - b;
        }
    }

}