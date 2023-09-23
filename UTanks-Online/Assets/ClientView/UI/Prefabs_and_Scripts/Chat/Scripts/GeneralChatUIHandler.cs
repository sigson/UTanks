using SecuredSpace.UI.Special;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTanksClient;
using UTanksClient.ECS.Events.Chat;

namespace SecuredSpace.UI.GameUI
{
    [System.Serializable]
    public class GeneralChatUIHandler : MonoBehaviour
    {
        public GameObject ChatCanvas;
        public ChatMessageElement ChatMessageExample;
        public TMP_InputField MessageInput;
        public MultiImageButton MessageSendButton;
        public SerializableDictionary<ChatMessageElement, long> messages = new SerializableDictionary<ChatMessageElement, long>();

        public void Start()
        {
            //ChatCanvas.GetComponent<UpdateUIObject>().Init(ChatCanvas,
            //    (gameObject) =>
            //    {
            //        gameObject.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
            //    },
            //    (gameObject) =>
            //    {
            //        gameObject.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
            //    });
        }

        public void Update()
        {
            if (!ClientInitService.instance.LockInput && Input.GetKeyDown(KeyCode.Return))
            {
                if (EventSystem.current.currentSelectedGameObject == MessageInput.gameObject)
                {
                    SendChatMessage();
                }
            }
        }
        public void ShowMessage(int Rank, string Nickname, string Message, long entityId)
        {
            var messageObj = Instantiate(ChatMessageExample, ChatCanvas.transform);
            messageObj.gameObject.SetActive(true);
            messageObj.UpdateMessage(Rank, Nickname, Message);
            messages.Add(messageObj, entityId);
            //ChatCanvas.GetComponent<UpdateUIObject>().UpdateObject();
        }

        public void SendChatMessage()
        {
            if (MessageInput.text != "")
            {
                ClientNetworkService.instance.Socket.emit(new ChatSendMessageEvent()
                {
                    battleEntity = 0,
                    channelEntity = 0,
                    messageBody = MessageInput.text,
                    teamMessage = false
                }.PackToNetworkPacket());
                MessageInput.text = "";
            }
            MessageInput.Select();
            MessageInput.ActivateInputField();
        }
    }

}