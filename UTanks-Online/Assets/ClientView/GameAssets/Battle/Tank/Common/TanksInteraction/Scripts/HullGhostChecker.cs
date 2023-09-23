using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Battle.Tank.Hull
{
    public class HullGhostChecker : MonoBehaviour
    {
        public TankManager parentTankManager;
        public List<Collider> collisionColliders = new List<Collider>();

        public void StartCheck()
        {
            StopAllCoroutines();
            collisionColliders.Clear();
            var meshCollider = this.GetComponent<MeshCollider>();
            meshCollider.enabled = false;
            meshCollider.enabled = true;
            StartCoroutine(CheckCollisions());
        }

        IEnumerator CheckCollisions()
        {
            yield return new WaitForSeconds(0.3f);
            while (!parentTankManager.Ghost && parentTankManager.checkGhost)
            {
                Collider coll = null;
                foreach (var collider in collisionColliders)
                {
                    
                    try {
                        var tankAnchor = collider.GetComponent<IManagableAnchor>();
                        if(tankAnchor.ownerManager<TankManager>().checkGhost == false && tankAnchor.ownerManager<TankManager>().Ghost == false)
                            coll = collider;
                    }
                    catch { }
                    try
                    {
                        var tankAnchor = collider.GetComponent<TankManager>();
                        if (tankAnchor.checkGhost == false && tankAnchor.Ghost == false)
                            coll = collider;
                    }
                    catch { }
                }
                if (coll == null)
                {
                    parentTankManager.DisableGhostTankAfterCheck();
                }
                if (collisionColliders.Count == 0)
                {
                    parentTankManager.DisableGhostTankAfterCheck();
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            collisionColliders.Add(other);
        }
        public void OnTriggerExit(Collider other)
        {
            try { collisionColliders.Remove(other); } catch { }
        }
    }
}