using SecuredSpace.Battle.Tank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankJumper : MonoBehaviour
{
    public float jumpCoord = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ClientInitService.instance.LockInput && Input.GetKeyDown(KeyCode.R) && this.GetComponentInParent<TankManager>().hullManager.chassisManager.TankMovable)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + jumpCoord, this.transform.position.z);
            this.GetComponent<Rigidbody>().velocity = new Vector3(this.GetComponent<Rigidbody>().velocity.x, 0, this.GetComponent<Rigidbody>().velocity.z);
        }
            
    }
}
