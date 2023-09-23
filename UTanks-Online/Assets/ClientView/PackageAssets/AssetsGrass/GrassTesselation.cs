using SecuredSpace.UI.Special;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Effects
{
    public class GrassTesselation : TimerElement
    {
        public Material grassMaterial;
        public float GrassDistanceFading = 0.24f;//0.11f;
                                                 //public float MaxVisibilityDistance = 100f;
        public float MaxUniform = 23f;//13f;
        public float NonChangeDistance = 20f;
        private bool grassEnabled;
        public bool GrassEnabled
        {
            get
            {
                return grassEnabled;
            }
            set
            {
                grassEnabled = value;
                if(value)
                {
                    this.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { this.GetComponent<MeshRenderer>().sharedMaterials[0], grassMaterial };
                }
                else
                {
                    this.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { this.GetComponent<MeshRenderer>().sharedMaterials[0] };
                    //this.GetComponent<MeshRenderer>().sharedMaterial =  this.GetComponent<MeshRenderer>().sharedMaterials[0];
                }
            }
        }
        // Start is called before the first frame update
        protected override void StartImpl()
        {
            timerTime = 0.5f;
            onTimerElapsed = (timer) => { };
            //grassMaterial = this.GetComponent<MeshRenderer>().sharedMaterials[1];
            GrassEnabled = ClientInitService.instance.gameSettings.Grass;
            base.StartImpl();
            running = true;
            //StartCoroutine("GrassTessel");
        }

        protected override void UpdateImpl()
        {
            if(GrassEnabled)
            {
                base.UpdateImpl();
                if (timeRemaining < 0)
                {
                    float dist = 0;
                    if(ClientInitService.instance.NowMainCamera == null)
                        dist = Vector3.Distance(Camera.main.transform.position, this.transform.position);
                    else
                        dist = Vector3.Distance(ClientInitService.instance.NowMainCamera.transform.position, this.transform.position);
                    if (dist > NonChangeDistance)
                        grassMaterial.SetFloat("_TessellationUniform", MaxUniform - dist * GrassDistanceFading);
                    ResetTimer();
                }
            }
        }

        //IEnumerator GrassTessel()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForEndOfFrame();

        //        yield return new WaitForSeconds(0.5f);
        //    }

        //}

    }
}
