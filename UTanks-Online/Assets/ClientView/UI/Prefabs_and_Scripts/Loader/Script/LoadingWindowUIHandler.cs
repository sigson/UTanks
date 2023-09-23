using SecuredSpace.ClientControl.Services;
using SecuredSpace.UnityExtend;
using SecuredSpace.UnityExtend.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient;

namespace SecuredSpace.UI.GameUI
{
    public class LoadingWindowUIHandler : MonoBehaviour
    {
        public SerializableDictionary<string, List<Sprite>> LoadingSlideshow = new SerializableDictionary<string, List<Sprite>>();
        public float SlideshowFrequency;
        public float AnimatonFrequency;
        public Text loadingText;
        public Image SlideImage;
        public Slider LoadingSlider;
        public Text connected;
        public Text notConnected;

        private int nowValue = 0;
        void Start()
        {
            Application.logMessageReceivedThreaded += Application_logMessageReceived;
            UnityTimer.Register(SlideshowFrequency, () => {
                if (SlideImage.gameObject.activeInHierarchy)
                    SlideImage.sprite = LoadingSlideshow["en"][Random.RandomRange(0, LoadingSlideshow["en"].Count - 1)];
            },
             null, true);
            //ClientInit.OnLoadedGame += (ev, ev2) => StartImpl();
        }

        private void OnEnable()
        {
            nowValue = 0;
            LoadingSlider.value = nowValue;
            if (ClientNetworkService.instance.PlayerEntityId != 0)
            {
                this.loadingText.gameObject.SetActive(false);
                this.connected.gameObject.SetActive(false);
                this.notConnected.gameObject.SetActive(false);
                //Application.logMessageReceivedThreaded -= Application_logMessageReceived;
            }
        }

        public void StartImpl()
        {
            //var loadingScript = ClientInit.uiManager.LoadingWindowUI.GetComponent<LoadingWindowUIHandler>();
            //this.loadingText.gameObject.SetActive(false);
            //this.connected.gameObject.SetActive(false);
            //this.notConnected.gameObject.SetActive(false);
        }
        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (ClientNetworkService.instance.PlayerEntityId != 0)
                return;
            UIService.instance.ExecuteInstruction((object Obj) =>
            {
                if (this.gameObject.activeInHierarchy && condition.Contains("LoadingLog"))
                {
                    var splitText = condition.Substring(condition.IndexOf("LoadingLog") + "LoadingLog".Length + 2).Split('/');
                    loadingText.text = splitText[2];
                    if(!(LoadingSlider.minValue == 0 && LoadingSlider.maxValue == int.Parse(splitText[1])))
                    {
                        LoadingSlider.minValue = 0;
                        LoadingSlider.maxValue = int.Parse(splitText[1]);
                    }
                    nowValue = int.Parse(splitText[0]);
                    LoadingSlider.value = nowValue;
                }
                if(ClientNetworkService.instance.Connected)
                {
                    connected.gameObject.SetActive(true);
                    notConnected.gameObject.SetActive(false);
                }
                else
                {
                    connected.gameObject.SetActive(false);
                    notConnected.gameObject.SetActive(true);
                }
            }, null);
        }
    }
}