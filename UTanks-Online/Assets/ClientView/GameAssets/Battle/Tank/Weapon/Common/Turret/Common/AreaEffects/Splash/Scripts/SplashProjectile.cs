using SecuredSpace.ClientControl.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class SplashProjectile : IProjectile
    {
        public SphereCollider ContactCollider;
        public int SizeOfSplash;

        private void OnEnable()
        {
            ContactCollider.radius = SizeOfSplash;
            ContactCollider.enabled = true;
            StartCoroutine("AwaitToHide");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Tank")
            {
                var parentManager = other.GetComponent<IManagableAnchor>();
                if (parentManager != null && !hitedTanks.ContainsKey(parentManager.ownerManager<TankManager>()))
                    SplashCheck(parentManager.ownerManager<TankManager>());
            }
        }

        IEnumerator AwaitToHide()
        {
            yield return new WaitForSeconds(0.1f);
            ContactCollider.enabled = false;
            
        }

        public void SplashCheck(TankManager tankManager)
        {

        }

        protected override void OnDestroyImpl()
        {
            //base.OnDestroyImpl();
            ContactCollider.enabled = false;
        }
    }
}