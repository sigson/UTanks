using SecuredSpace.Battle.Tank;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUpper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!ClientInitService.instance.LockInput && Input.GetKeyDown(KeyCode.K) && this.GetComponentInParent<TankManager>().hullManager.chassisManager.TankMovable)
            this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);
    }
}
