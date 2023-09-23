using System.Collections;
using System.Collections.Generic;
using Assets.ClientCore.CoreImpl.ECS.Components.Battle.Hull;
using SecuredSpace.Important.TPhysics;
using SecuredSpace.Important.Raven;
using UnityEngine;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.ECSCore;
using SecuredSpace.Settings;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullManager : MonoBehaviour
    {
        public TankManager parentTankManager;
        public TankChassisManager chassisManager;
        public PhysicMaterial BoundMaterial;
        public AudioAnchor hullAudioSource;
        public void Initialize(TankManager tankManager, ECSEntity player)
        {
            var emptyGO = new GameObject();
            parentTankManager = tankManager;
            tankManager.TankBoundsObject.GetComponent<BoxCollider>().center = this.GetComponent<MeshFilter>().mesh.bounds.center;
            tankManager.TankBoundsObject.GetComponent<BoxCollider>().size = this.GetComponent<MeshFilter>().mesh.bounds.size;
            tankManager.TankBoundsObject.GetComponent<BoxCollider>().material = BoundMaterial;
            var hullComponent = player.GetComponent<HullComponent>(HullComponent.Id);

            #region Init aim colliders OLD

            //var tankMesh = tankManager.Hull.GetComponent<MeshFilter>();
            //parentTankManager.HullAngleColliderHeader.transform.localPosition = new Vector3(parentTankManager.HullAngleColliderHeader.transform.localPosition.x, tankMesh.sharedMesh.bounds.size.y / 2, parentTankManager.HullAngleColliderHeader.transform.localPosition.z);


            //parentTankManager.HullAngleColliders["LeftBackwardTop"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftBackwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2, tankMesh.sharedMesh.bounds.size.z / 2);//xyz
            //parentTankManager.HullAngleColliders["LeftBackwardTop"].gameObject.name = "LeftBackwardTop";
            //parentTankManager.HullAngleColliders["LeftBackwardTop"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["RightBackwardTop"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightBackwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2, tankMesh.sharedMesh.bounds.size.z / 2);//-xyz
            //parentTankManager.HullAngleColliders["RightBackwardTop"].gameObject.name = "RightBackwardTop";
            //parentTankManager.HullAngleColliders["RightBackwardTop"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["LeftBackwardDown"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2);//x-yz
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].gameObject.name = "LeftBackwardDown";
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["LeftForwardTop"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftForwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//xy-z
            //parentTankManager.HullAngleColliders["LeftForwardTop"].gameObject.name = "LeftForwardTop";
            //parentTankManager.HullAngleColliders["LeftForwardTop"].layer = LayerMask.NameToLayer("AIMLayer");


            //parentTankManager.HullAngleColliders["RightBackwardDown"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightBackwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2);//-x-yz
            //parentTankManager.HullAngleColliders["RightBackwardDown"].gameObject.name = "RightBackwardDown";
            //parentTankManager.HullAngleColliders["RightBackwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["LeftForwardDown"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftForwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//x-y-z
            //parentTankManager.HullAngleColliders["LeftForwardDown"].gameObject.name = "LeftForwardDown";
            //parentTankManager.HullAngleColliders["LeftForwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["RightForwardDown"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightForwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//-x-y-z
            //parentTankManager.HullAngleColliders["RightForwardDown"].gameObject.name = "RightForwardDown";
            //parentTankManager.HullAngleColliders["RightForwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["RightForwardTop"] = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightForwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//-xy-z
            //parentTankManager.HullAngleColliders["RightForwardTop"].gameObject.name = "RightForwardTop";
            //parentTankManager.HullAngleColliders["RightForwardTop"].layer = LayerMask.NameToLayer("AIMLayer");


            //foreach (var colliderPoint in parentTankManager.HullAngleColliders.Values)
            //{
            //    colliderPoint.AddComponent<BoxCollider>();
            //    var boxCollider = colliderPoint.GetComponent<BoxCollider>();
            //    colliderPoint.AddComponent<TankManagerAnchor>();
            //    colliderPoint.GetComponent<TankManagerAnchor>().parentTankManager = parentTankManager;
            //    boxCollider.isTrigger = true;
            //    boxCollider.size = new Vector3(0.02f, 0.02f, 0.02f);
            //}

            //parentTankManager.HullCenterCollider = Instantiate(new GameObject(), parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullCenterCollider.AddComponent<BoxCollider>();
            ////parentTankManager.HullCenterCollider.GetComponent<BoxCollider>().center = this.GetComponent<MeshFilter>().mesh.bounds.center;
            //parentTankManager.HullCenterCollider.GetComponent<BoxCollider>().size = new Vector3(0.02f, 0.02f, 0.02f);
            //parentTankManager.HullCenterCollider.AddComponent<TankManagerAnchor>();
            //parentTankManager.HullCenterCollider.GetComponent<TankManagerAnchor>().parentTankManager = parentTankManager;
            //parentTankManager.HullCenterCollider.layer = LayerMask.NameToLayer("AIMLayer");

            #endregion

            #region Init aim colliders

            var tankMesh = tankManager.Hull.GetComponent<MeshFilter>();
            parentTankManager.HullAngleColliderHeader.transform.localPosition = new Vector3(parentTankManager.HullAngleColliderHeader.transform.localPosition.x, tankMesh.sharedMesh.bounds.size.y / 2, parentTankManager.HullAngleColliderHeader.transform.localPosition.z);


            parentTankManager.HullAngleColliders["LeftBackwardTop"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            parentTankManager.HullAngleColliders["LeftBackwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 3, tankMesh.sharedMesh.bounds.size.z / 2);//xyz
            parentTankManager.HullAngleColliders["LeftBackwardTop"].gameObject.name = "LeftBackwardTop";
            parentTankManager.HullAngleColliders["LeftBackwardTop"].layer = LayerMask.NameToLayer("AIMLayer");

            parentTankManager.HullAngleColliders["RightBackwardTop"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            parentTankManager.HullAngleColliders["RightBackwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 3, tankMesh.sharedMesh.bounds.size.z / 2);//-xyz
            parentTankManager.HullAngleColliders["RightBackwardTop"].gameObject.name = "RightBackwardTop";
            parentTankManager.HullAngleColliders["RightBackwardTop"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["LeftBackwardDown"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2);//x-yz
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].gameObject.name = "LeftBackwardDown";
            //parentTankManager.HullAngleColliders["LeftBackwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            parentTankManager.HullAngleColliders["LeftForwardTop"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            parentTankManager.HullAngleColliders["LeftForwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 3, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//xy-z
            parentTankManager.HullAngleColliders["LeftForwardTop"].gameObject.name = "LeftForwardTop";
            parentTankManager.HullAngleColliders["LeftForwardTop"].layer = LayerMask.NameToLayer("AIMLayer");


            //parentTankManager.HullAngleColliders["RightBackwardDown"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightBackwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2);//-x-yz
            //parentTankManager.HullAngleColliders["RightBackwardDown"].gameObject.name = "RightBackwardDown";
            //parentTankManager.HullAngleColliders["RightBackwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["LeftForwardDown"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["LeftForwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//x-y-z
            //parentTankManager.HullAngleColliders["LeftForwardDown"].gameObject.name = "LeftForwardDown";
            //parentTankManager.HullAngleColliders["LeftForwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            //parentTankManager.HullAngleColliders["RightForwardDown"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            //parentTankManager.HullAngleColliders["RightForwardDown"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 2 * -1, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//-x-y-z
            //parentTankManager.HullAngleColliders["RightForwardDown"].gameObject.name = "RightForwardDown";
            //parentTankManager.HullAngleColliders["RightForwardDown"].layer = LayerMask.NameToLayer("AIMLayer");

            parentTankManager.HullAngleColliders["RightForwardTop"] = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            parentTankManager.HullAngleColliders["RightForwardTop"].transform.localPosition = new Vector3(tankMesh.sharedMesh.bounds.size.x / 2 * -1, tankMesh.sharedMesh.bounds.size.y / 3, tankMesh.sharedMesh.bounds.size.z / 2 * -1);//-xy-z
            parentTankManager.HullAngleColliders["RightForwardTop"].gameObject.name = "RightForwardTop";
            parentTankManager.HullAngleColliders["RightForwardTop"].layer = LayerMask.NameToLayer("AIMLayer");


            foreach (var colliderPoint in parentTankManager.HullAngleColliders.Values)
            {
                colliderPoint.AddComponent<BoxCollider>();
                var boxCollider = colliderPoint.GetComponent<BoxCollider>();
                colliderPoint.AddComponent<IManagableAnchor>();
                colliderPoint.GetComponent<IManagableAnchor>().ownerManagerSpace = parentTankManager;
                boxCollider.isTrigger = true;
                boxCollider.size = new Vector3(0.02f, 0.02f, 0.02f);
            }

            parentTankManager.HullCenterCollider = Instantiate(emptyGO, parentTankManager.HullAngleColliderHeader.transform);
            parentTankManager.HullCenterCollider.AddComponent<BoxCollider>();
            //parentTankManager.HullCenterCollider.GetComponent<BoxCollider>().center = this.GetComponent<MeshFilter>().mesh.bounds.center;
            parentTankManager.HullCenterCollider.GetComponent<BoxCollider>().size = new Vector3(0.02f, 0.02f, 0.02f);
            parentTankManager.HullCenterCollider.AddComponent<IManagableAnchor>();
            parentTankManager.HullCenterCollider.GetComponent<IManagableAnchor>().ownerManagerSpace = parentTankManager;
            parentTankManager.HullCenterCollider.layer = LayerMask.NameToLayer("AIMLayer");


            Destroy(emptyGO);
            #endregion
        }
        public virtual void RebuildTank(ECSEntity player)
        {
            var hullComponent = player.GetComponent<HullComponent>(HullComponent.Id);
            chassisManager.Speed = hullComponent.speed;
            chassisManager.Acceleration = hullComponent.acceleration;
            chassisManager.TurnSpeed = hullComponent.turnSpeed;
            chassisManager.TurnAcceleration = hullComponent.turnAcceleration;
            chassisManager.Weight = hullComponent.weight;
            chassisManager.Damping = hullComponent.damping;
            chassisManager.ReverseAcceleration = hullComponent.reverseAcceleration;
            chassisManager.ReverseTurnAcceleration = hullComponent.reverseTurnAcceleration;
            chassisManager.SideAcceleration = hullComponent.sideAcceleration;
            chassisManager.ApplyPhysics();
        }

        public virtual void HotRebuildTank(ECSEntity player)
        {
            var hullComponent = player.GetComponent<HullComponent>(HullComponent.Id);
            chassisManager.Speed = hullComponent.speed;
            chassisManager.Acceleration = hullComponent.acceleration;
            chassisManager.TurnSpeed = hullComponent.turnSpeed;
            chassisManager.TurnAcceleration = hullComponent.turnAcceleration;
            chassisManager.Weight = hullComponent.weight;
            chassisManager.Damping = hullComponent.damping;
            chassisManager.ReverseAcceleration = hullComponent.reverseAcceleration;
            chassisManager.ReverseTurnAcceleration = hullComponent.reverseTurnAcceleration;
            chassisManager.SideAcceleration = hullComponent.sideAcceleration;
            chassisManager.HotApplyPhysics();
        }
    }
}
