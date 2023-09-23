using SecuredSpace.Important.Aim;
using SecuredSpace.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.Weapon;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class ITurret : MonoBehaviour
    {
        public TankManager parentTankManager;
        public TurretRotaion turretRotaion;
        public AudioAnchor weaponAudio;
        public virtual void Initialize(TankManager tankManager, ECSEntity player)
        {
            parentTankManager = tankManager;
            turretRotaion = tankManager.Turret.GetComponent<TurretRotaion>();
            turretRotaion.maxTorque = (player.GetComponents(
                IsisDamageComponent.Id, 
                ThunderDamageComponent.Id,
                SmokyDamageComponent.Id,
                RailgunDamageComponent.Id,
                FlamethrowerDamageComponent.Id,
                FreezeDamageComponent.Id,
                GaussDamageComponent.Id,
                HammerDamageComponent.Id,
                RicochetDamageComponent.Id,
                ShaftDamageComponent.Id,
                TwinsDamageComponent.Id,
                VulcanDamageComponent.Id
                )[0] as WeaponComponent).turretSpeedRotationProperty * 5f;
            turretRotaion.minTorque = turretRotaion.maxTorque * 0.75f;
        }

        public virtual void TurretEvent(ECSEntity player, ECSEvent updatingEvent)
        {

        }

        public virtual void TurretInfluenceEvent(ECSEntity player, ECSEvent updatingEvent)
        {

        }

        public virtual void RebuildTank(ECSEntity player)
        {
            //Type weaponComponentType = typeof(WeaponComponent);
            //var WeaponComponents = playerEntity.entityComponents.Components.Where(x => x.GetTypeFast().BaseType == weaponComponentType).ToList();
        }

        public virtual void HotRebuildTank(ECSEntity player)
        {
            //Type weaponComponentType = typeof(WeaponComponent);
            //var WeaponComponents = playerEntity.entityComponents.Components.Where(x => x.GetTypeFast().BaseType == weaponComponentType).ToList();
        }

        public virtual void DestroyTurret(ECSEntity player)
        {
            
        }

        public virtual void RemoveTurret(ECSEntity player)
        {
            StopAllCoroutines();
        }

        public virtual void FixedUpd()
        {

        }

        public Vector3 CheckMuzzleVisible()
        {
            RaycastHit checkhit;
            Vector3 pointOfExplosive = Vector3.zero;
           // bool checkMuzzle = true;
            if (Physics.Linecast(parentTankManager.Turret.transform.position, parentTankManager.MuzzlePoint.transform.position, out checkhit, LayerMask.GetMask("Default", "Tank", "TankBounds", "Muzzle", "Friction", "Bounds"), QueryTriggerInteraction.Ignore))//LayerMask.GetMask(LayerMask.LayerToName(this.parentTankManager.MuzzleCheckPoint.layer))
            {
                if (checkhit.collider.gameObject.tag != "MuzzleCheck")
                {
                    pointOfExplosive = checkhit.point;
                    //checkMuzzle = false;
                }
            }
            return pointOfExplosive;
        }

        public void NextMuzzlePoint()
        {
            if (this.parentTankManager.selectedMuzzlePoint == this.parentTankManager.MuzzlePoints.Count - 1)
            {
                this.parentTankManager.selectedMuzzlePoint = 0;
                this.parentTankManager.MuzzlePoint = this.parentTankManager.MuzzlePoints[this.parentTankManager.selectedMuzzlePoint];
                this.parentTankManager.MuzzleCheckPoint = this.parentTankManager.MuzzlePoints[this.parentTankManager.selectedMuzzlePoint].transform.GetChild(0).gameObject;
                this.parentTankManager.closeDistanceAIM = this.parentTankManager.MuzzlePoint.GetComponent<CloseDistanceAIM>();
            }
            else
            {
                this.parentTankManager.selectedMuzzlePoint++;
                this.parentTankManager.MuzzlePoint = this.parentTankManager.MuzzlePoints[this.parentTankManager.selectedMuzzlePoint];
                this.parentTankManager.MuzzleCheckPoint = this.parentTankManager.MuzzlePoints[this.parentTankManager.selectedMuzzlePoint].transform.GetChild(0).gameObject;
                this.parentTankManager.closeDistanceAIM = this.parentTankManager.MuzzlePoint.GetComponent<CloseDistanceAIM>();
            }
        }
        private void FixedUpdate()
        {
            if (parentTankManager.hullManager.chassisManager.TankMovable)
                InputOperations();
            FixedUpd();
        }

        public virtual void InputOperations()
        {

        }

        public virtual ITurret AppendManagerToObject(GameObject turretObject)
        {
            return null;
        }
    }
}
