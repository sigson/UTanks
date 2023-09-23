using SecuredSpace.ClientControl.Managers;
using SecuredSpace.Settings;
using SecuredSpace.UI.Special;
using SecuredSpace.UnityExtend.MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects.Lighting
{
    public class LineLight : TimerElement
    {
        List<ILight> lights = new List<ILight>();
        public float maxIntensity = 2.5f;
        public float LightingDistanceFactor = 20;
        public float LightSize = 2.53f;
        public Color LightColor;
        public float fadingTime;
        public bool realLight;
        public bool AutoUpdate;

        protected override void StartImpl()
        {
            timerTime = fadingTime;
            base.StartImpl();
            onTimerElapsed = (x) => { };
            running = true;
        }

        float distinctValue = 0f;
        bool stopLight = false;
        protected override void UpdateImpl()
        {
            base.UpdateImpl();
            distinctValue += Time.deltaTime;
            if (AutoUpdate)
            {
                if (!stopLight)
                    if (distinctValue > 0.3f || !running)
                    {
                        lights.ForEach(x => {
                            if (maxIntensity * (timeRemaining / fadingTime) > 0f)
                            {
                                x.lightIntencity = maxIntensity * (timeRemaining / fadingTime);
                                x.UpdateLight();
                            }
                            else
                            {
                                x.LightSwitch(false);
                                x.lightIntencity = maxIntensity;
                                x.gameObject.SetActive(false);
                                stopLight = true;
                            }
                        });
                        distinctValue = 0;
                    }
                //if (stopLight)
                //    reflectionProbes.Clear();
            }
        }

        public void HideAllLights() => lights.ForEach(x => x.gameObject.SetActive(false));

        public void SetLightToLine(Vector3 startLineWorldPosition, Vector3 endLineWorldPosition, bool UpdateLight = true)
        {
            //this.transform.position = startLineWorldPosition;
            //this.transform.LookAt(endLineWorldPosition);
            //this.transform.position = Vector3.MoveTowards(startLineWorldPosition, endLineWorldPosition, 0.5f);
            float distance = Vector3.Distance(startLineWorldPosition, endLineWorldPosition);
            //float factorBalanceDistinct = (distance / LightingFactor) - Mathf.Floor(distance / LightingFactor);
            //float balancedDistance = distance - (factorBalanceDistinct * LightingFactor);
            //if(factorBalanceDistinct < 0.5f)
            //    balancedDistance -= balancedDistance / LightingFactor;
            //else
            //    balancedDistance += balancedDistance / LightingFactor;
            int counter = 0;
            for (float i = 0; i < distance; i+= LightingDistanceFactor)
            {
                var lightPosition = Vector3.MoveTowards(startLineWorldPosition, endLineWorldPosition, i);//error
                //var lightPosition = startLineWorldPosition + i * (endLineWorldPosition - startLineWorldPosition);
                if (MapManager.CheckBoundsPositon(lightPosition))
                {
                    if(lights.Count-1 >= counter)
                    {
                        var lightScript = lights[counter];
                        lightScript.gameObject.SetActive(true);
                        lightScript.transform.position = lightPosition;
                        //var lightScript = light.GetComponent<ILight>();
                        if(UpdateLight)
                        {
                            lightScript.lightColor = LightColor;
                            lightScript.lightIntencity = maxIntensity;
                            lightScript.lightSize = LightSize;
                            lightScript.UpdateLight();
                            if (!lightScript.lightSwitchState)
                                lightScript.LightSwitch(true);
                        }
                        
                        //reflectionProbes.Add(lightScript);
                    }
                    else
                    {
                        string poolname = realLight ? "PointLightSources" : "ReflectionLightSources";
                        var light = EasyObjectPool.instance.GetObjectFromPool(poolname, lightPosition, Quaternion.identity);
                        
                        //light.transform.SetParent(this.transform);
                        var lightScript = light.GetComponent<ILight>();
                        lightScript.lightColor = LightColor;
                        var anchor = lightScript.gameObject.GetComponent<LightAnchor>();
                        anchor.WeaponLight = true;
                        anchor.UpdateObject();
                        //maxIntensity = lightScript.lightIntencity;
                        lightScript.lightIntencity = maxIntensity;
                        lightScript.lightSize = LightSize;
                        lightScript.checkGround = false;
                        lightScript.UpdateLight();
                        lightScript.LightSwitch(true);
                        lights.Add(lightScript);
                    }
                    
                }
                counter++;
            }
            if(!AutoUpdate)
                for(int i = counter; i < lights.Count; i++)
                {
                    lights[i].gameObject.SetActive(false);
                }
            stopLight = false;
            ResetTimer();
        }

        private void OnDestroy()
        {
            lights.ForEach(x =>
            {
                EasyObjectPool.instance.ReturnObjectToPool(x.gameObject);
            });
        }
    }
}