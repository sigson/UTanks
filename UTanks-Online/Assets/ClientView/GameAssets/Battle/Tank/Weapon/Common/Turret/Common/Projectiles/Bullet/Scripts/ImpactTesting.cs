using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ImpactTesting : MonoBehaviour
{
    public int RechargeTime;
    public Rigidbody tankRigidbody;
    public GameObject muzzlePoint;
    public float scaler = 2f;
    long timestamp = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(DateTime.Now.Ticks > timestamp + RechargeTime && !ClientInitService.instance.LockInput && Input.GetKey(KeyCode.Space))
        {
            Shoot();
            timestamp = DateTime.Now.Ticks;
        }
    }

    private void Shoot()
    {
        tankRigidbody.AddForceAtPosition(this.transform.forward * scaler, muzzlePoint.transform.position);
    }
}
