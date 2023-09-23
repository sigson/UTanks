using SecuredSpace.UI.Special;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UTanksClient.Services;

namespace SecuredSpace.UI.GameUI
{
    public class ChatMessageElement : MonoBehaviour
    {
        public Image SenderRankIcon;
        public Text SenderNickname;
        public Text MessageBody;

        public void UpdateMessage(int Rank, string Nickname, string Message, Color? nicknameColor = null)
        {
            SenderNickname.text = Nickname + ":";
            if (nicknameColor != null)
                SenderNickname.color = (Color)nicknameColor;
            MessageBody.text =  Message;
            //SenderRankIcon.GetComponent<Image>().sprite = ClientInit.ItemResourcesDBOld.AllRankData.AllRanks[ConstantService.instance.GetByConfigPath("ranksconfig").Deserialized["ranksExperiencesConfig"]["ranksExperiences"][Rank]["name"].ToString()].miniIcon;
            var messageWidth = 0f;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if(this.transform.GetChild(i).gameObject.activeInHierarchy)
                    messageWidth += this.transform.GetChild(i).GetComponent<RectTransform>().rect.width;
            }
            var newMessageRect = new Rect(MessageBody.transform.GetComponent<RectTransform>().rect);
            newMessageRect.width += this.transform.GetComponent<RectTransform>().rect.width * 2 - messageWidth;
            //MessageBody.transform.GetComponent<RectTransform>().rect.Set(newMessageRect.x, newMessageRect.y, newMessageRect.width, newMessageRect.height);
            MessageBody.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(newMessageRect.width, newMessageRect.height);
            //this.transform.parent.GetComponent<ContentLayoutRefreshMarker>().MarkForUpdate();
        }
    }

}