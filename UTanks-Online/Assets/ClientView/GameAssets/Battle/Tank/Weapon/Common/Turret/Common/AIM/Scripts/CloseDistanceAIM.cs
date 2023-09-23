using SecuredSpace.Battle.Tank;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.Components.Battle.Tank;

namespace SecuredSpace.Important.Aim
{
    public class CloseDistanceAIM : MonoBehaviour
    {
        public float verticalAngle = 30;
        public float horizontalAngle = 30;
        public float distance = 80;
        public float separatorFactor = 10;
        public float zColliderSize = 0f;
        public bool MultiplyTargetsMode = false;
        public bool AOETargeting = false;
        public bool SuperClosestCompensation = true;
        public float SuperClosestCompensatorHorizontal = 4;
        public float SuperClosestCompensatorVertical = 2;
        public float SuperClosestThreshold = 0.15f;
        public TankManager parentTankManager;
        public GameObject AimColliderStorage;
        public List<Collider> Colliders = new List<Collider>(); //mainly box collider
        public Dictionary<Collider, int> ColliderEntered = new Dictionary<Collider, int>();
        //public List<Collider> watchlist;

        public void OnEnable()
        {
            //RebuildAIM();
            StopCoroutine(CleanColliders());
            StartCoroutine(CleanColliders());
        }

        IEnumerator CleanColliders()
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);
                yield return new WaitForEndOfFrame();
                List<Collider> colliderToRemove = new List<Collider>();
                ColliderEntered.ForEach(x => {
                    if (!x.Key.gameObject.activeInHierarchy)
                        colliderToRemove.Add(x.Key);
                });
                colliderToRemove.ForEach(x => ColliderEntered.Remove(x));
            }
        }

        public void RebuildAIM()
        {
            if(AOETargeting)
                this.gameObject.layer = LayerMask.NameToLayer("TankBounds");
            for(int i = 0; i < separatorFactor; i++)
            {
                var nowDistance = distance / 10f / separatorFactor * i;
                var zColliderSizeLocal = zColliderSize == 0 ? nowDistance / i : zColliderSize;

                BoxCollider collider;
                if (Colliders.Count > i)
                    collider = Colliders[i] as BoxCollider;
                else
                {
                    collider = this.gameObject.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    Colliders.Add(collider);
                }
                float boundWidth = 0;
                //if (!SuperClosestCompensation)
                    boundWidth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * horizontalAngle), 2) - Mathf.Pow(nowDistance, 2));
                //else
                //{
                //    if (i / separatorFactor <= SuperClosestThreshold)
                //    {
                //        boundWidth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * horizontalAngle), 2) - Mathf.Pow(nowDistance, 2)) * SuperClosestCompensatorHorizontal;
                //    }
                //    else
                //    {
                //        boundWidth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * horizontalAngle), 2) - Mathf.Pow(nowDistance, 2));
                //    }
                //}

                float boundHeigth = 0;
                if (!SuperClosestCompensation)
                    boundHeigth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * verticalAngle), 2) - Mathf.Pow(nowDistance, 2));
                else
                {
                    if(i/separatorFactor <= SuperClosestThreshold)
                    {
                        boundHeigth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * verticalAngle), 2) - Mathf.Pow(nowDistance, 2)) * SuperClosestCompensatorVertical;
                    }
                    else
                    {
                        boundHeigth = Mathf.Sqrt(Mathf.Pow(nowDistance / Mathf.Cos(Mathf.PI / 180 * verticalAngle), 2) - Mathf.Pow(nowDistance, 2));
                    }
                }

                collider.center = new Vector3(0, 0, nowDistance * -1);
                collider.size = new Vector3(boundWidth, boundHeigth, zColliderSizeLocal);
            }
        }

        public List<(TankManager, Vector3)> FindPossibleTargets(TankManager shooter)
        {
            List<(TankManager, Vector3)> targets = new List<(TankManager, Vector3)>();
            HashSet<TankManager> tankManagers = new HashSet<TankManager>();
            if (ColliderEntered.Count == 0)
                return targets;
            foreach (var collider in ColliderEntered)
                try { tankManagers.Add(collider.Key.GetComponent<IManagableAnchor>().ownerManager<TankManager>()); } catch { }
            if(AOETargeting)
            {
                tankManagers.ForEach((tankmanag) => {
                    if (!tankmanag.ManagerEntity.HasComponent(TankDeadStateComponent.Id) && !tankmanag.ManagerEntity.HasComponent(TankNewStateComponent.Id))
                        if (!tankmanag.checkGhost && !tankmanag.Ghost)
                            targets.Add((tankmanag, tankmanag.HullCenterCollider.transform.position));
                });
            }
            if(MultiplyTargetsMode)
            {
                foreach (var tankManager in tankManagers)
                {
                    if (tankManager.ManagerEntity.HasComponent(TankDeadStateComponent.Id) || tankManager.ManagerEntity.HasComponent(TankNewStateComponent.Id))
                        continue;
                    if (ColliderEntered.ContainsKey(tankManager.HullCenterCollider.GetComponent<BoxCollider>()))
                    {
                        RaycastHit raycastHit;
                        if(Physics.Linecast(shooter.MuzzlePoint.transform.position, tankManager.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Default", "AIMLayer"), QueryTriggerInteraction.Ignore))
                        {
                            if(raycastHit.collider == tankManager.HullCenterCollider.GetComponent<BoxCollider>())
                            {
                                if(!tankManager.checkGhost && !tankManager.Ghost)
                                    targets.Add((tankManager, tankManager.HullCenterCollider.transform.position));
                            }
                        }
                    }
                }
            }
            else
            {
                float minimalDistance = -1;
                TankManager minimalTankManager = null;
                foreach (var tankManager in tankManagers)
                {
                    if (tankManager.ManagerEntity.HasComponent(TankDeadStateComponent.Id) || tankManager.ManagerEntity.HasComponent(TankNewStateComponent.Id))
                        continue;
                    if (ColliderEntered.ContainsKey(tankManager.HullCenterCollider.GetComponent<BoxCollider>()))
                    {
                        RaycastHit raycastHit;
                        if (Physics.Linecast(shooter.MuzzlePoint.transform.position, tankManager.HullCenterCollider.transform.position, out raycastHit, LayerMask.GetMask("Default", "AIMLayer"), QueryTriggerInteraction.Ignore))
                        {
                            if (raycastHit.collider == tankManager.HullCenterCollider.GetComponent<BoxCollider>())
                            {
                                if (!tankManager.checkGhost && !tankManager.Ghost)
                                {
                                    if (minimalDistance == -1)
                                    {
                                        minimalDistance = Vector3.Distance(shooter.MuzzlePoint.transform.position, tankManager.HullCenterCollider.transform.position);
                                        minimalTankManager = tankManager;
                                    }
                                    else
                                    {
                                        var checkDistance = Vector3.Distance(shooter.MuzzlePoint.transform.position, tankManager.HullCenterCollider.transform.position);
                                        if (checkDistance < minimalDistance)
                                        {
                                            minimalDistance = checkDistance;
                                            minimalTankManager = tankManager;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (minimalTankManager == null)
                    return targets;
                targets.Add((minimalTankManager, minimalTankManager.HullCenterCollider.transform.position));
            }
            
            return targets;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (ColliderEntered.TryGetValue(other, out _))
                ColliderEntered[other]++;
            else
            {
                ColliderEntered.Add(other, 1);
            }
            //watchlist = ColliderEntered.Keys.ToList();
        }
        public void OnTriggerExit(Collider other)
        {
            int presentedColl;
            if(ColliderEntered.TryGetValue(other, out presentedColl))
            {
                presentedColl--;
                if (presentedColl == 0)
                    ColliderEntered.Remove(other);
                else
                    ColliderEntered[other]--;
            }
            
                
            //watchlist = ColliderEntered.Keys.ToList();
        }
    }
}
