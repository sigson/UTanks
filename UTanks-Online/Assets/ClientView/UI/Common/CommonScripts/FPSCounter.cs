using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecuredSpace.UI
{
    [Serializable]
    public class FPSCounter : MonoBehaviour
    {

        public int avgFrameRate;
        public TMP_Text display_Text;
        public int delay;
        private long lastTimestamp;

        private void Update()
        {
            if(ClientInitService.instance.gameSettings.ShowFPSPing)
            {
                avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
                if (lastTimestamp + delay < DateTime.Now.Ticks)
                {
                    //var newText = ((1f / Time.unscaledDeltaTime).ToString().Length > 5 ? (1f / Time.unscaledDeltaTime).ToString().Substring(0, 5) : Math.Round((1f / Time.unscaledDeltaTime), 2).ToString()).Replace(",", ".");
                    display_Text.text = Math.Round((1f / Time.unscaledDeltaTime), 2).ToString();
                    lastTimestamp = DateTime.Now.Ticks;
                }
            }
            if (ClientInitService.instance.gameSettings.ShowFPSPing != display_Text.enabled)
                display_Text.enabled = ClientInitService.instance.gameSettings.ShowFPSPing;
        }
    }

}