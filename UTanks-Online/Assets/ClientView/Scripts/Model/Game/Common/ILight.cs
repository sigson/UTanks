using SecuredSpace.ClientControl.Model;
using SecuredSpace.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects.Lighting
{
    public class ILight : IManagable
    {
        public Color lightColor;
        public float lightSize = 2.53f;//blend size
        public int lightImportance = 2;
        public float lightIntencity = 2.5f;
        public bool lightSwitchState = true;
        public bool checkGround = true;
        public Texture cookie;
        protected bool groundChecked = true;
        protected IAnchor anchor = null;
        public void OnEnable()
        {
            OnEnableImpl();
            groundChecked = true;
        }

        protected virtual void OnEnableImpl()
        {
            
        }

        public virtual void UpdateLight()
        {
            //CheckGroud();
        }

        protected virtual void CheckGroud()
        {
            if(checkGround && !groundChecked)
            {
                RaycastHit rayHit;
                if(Physics.Raycast(this.transform.position, this.transform.up * -1, out rayHit, lightSize, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    if(rayHit.collider == null)
                    {
                        groundChecked = true;
                    }
                }
            }
        }

        public void OnDisable()
        {
            OnDisableImpl();
            groundChecked = false;
        }

        public virtual void LightSwitch(bool switchValue, bool nestedUpdate = false)
        {
            bool anchorUpd = false;
            if(nestedUpdate == false)
            {
                CheckGroud();
                if (checkGround && !groundChecked && switchValue)
                    this.LightSwitch(false, true);
                if(anchor == null)
                    anchor = this.GetComponent<IAnchor>();
                if (anchor != null && (anchor.objectEnabled != lightSwitchState || anchor.objectEnabled != switchValue))
                {
                    this.LightSwitch(anchor.objectEnabled, true);
                    lightSwitchState = anchor.objectEnabled;
                    anchorUpd = true;
                }
            }
            if(!anchorUpd)
                lightSwitchState = switchValue;
        }

        protected virtual void OnDisableImpl()
        {
            
        }
    }
}