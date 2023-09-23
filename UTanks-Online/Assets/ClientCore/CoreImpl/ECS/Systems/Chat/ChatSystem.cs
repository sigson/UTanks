using SecuredSpace.ClientControl.Services;
using SecuredSpace.UI;
using SecuredSpace.UI.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Components.Battle.BattleComponents;
using UTanksClient.ECS.Components.Battle.Team;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Events.Chat;
using UTanksClient.ECS.Templates.User;
using UTanksClient.Extensions;

namespace UTanksClient.ECS.Systems.Chat
{
    public class ChatSystem : ECSSystem
    {
        public override bool HasInterest(ECSEntity entity)
        {
            return false;
        }

        public override void Initialize(ECSSystemManager SystemManager)
        {
            SystemEventHandler.Add(ChatSendMessageEvent.Id, new List<Func<ECSEvent, object>>() {
                (Event) => {
                    ChatMessageAccept((ChatSendMessageEvent)Event);
                    return null;
                }
            });
            //ComponentsOnChangeCallbacks.Add()
            //this.Enabled = true;
        }

        private void ChatMessageAccept(ChatSendMessageEvent chatSendMessageEvent)
        {
            var playerEntity = ManagerScope.entityManager.EntityStorage[ClientNetworkService.instance.PlayerEntityId];
            if(!playerEntity.HasComponent(BattleOwnerComponent.Id))
            {
                UIService.instance.ExecuteInstruction((object Obj) =>
                {
                    UIService.instance.ChatUI.GetComponent<GeneralChatUIHandler>().ShowMessage(chatSendMessageEvent.SenderRank, chatSendMessageEvent.SenderNickname, chatSendMessageEvent.messageBody, chatSendMessageEvent.EntityOwnerId);
                }, null);
            }
            else
            {
                UIService.instance.ExecuteInstruction((object Obj) =>
                {
                    UIService.instance.battleUIHandler.ShowMessage(chatSendMessageEvent.SenderRank, chatSendMessageEvent.SenderNickname, chatSendMessageEvent.messageBody, chatSendMessageEvent.EntityOwnerId);
                }, null);
            }
        }

        public override void Operation(ECSEntity entity, ECSComponent Component)
        {
            
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedComponentsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { }, IValues = { } }.Upd();
        }

        public override ConcurrentDictionaryEx<long, int> ReturnInterestedEventsList()
        {
            return new ConcurrentDictionaryEx<long, int>() { IKeys = { ChatSendMessageEvent.Id }, IValues = { 0 } }.Upd();
        }

        public override void Run(long[] entities)
        {

        }

        public override bool UpdateInterestedList(List<long> ComponentsId)
        {
            return false;
        }
    }
}
