using Assets.ClientCore.CoreImpl.ECS.Components.Battle.Bonus;
using SecuredSpace.ClientControl.Model;
using SecuredSpace.Battle.Drop;
using SecuredSpace.UnityExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient.ECS.Components.Battle.Bonus;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types.Battle.Bonus;

namespace SecuredSpace.ClientControl.Managers
{
    public class BattleDropManager : IGroupManager<ECSEntity>//deprecated
    {
        public GameObject DropObjectExample;
        public SerializableDictionary<long, DropManager> DropDB = new SerializableDictionary<long, DropManager>();

        public override void AddManager()
        {
            throw new System.NotImplementedException();
        }

        

        protected override void OnActivateManager()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAdd(ECSEntity entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAwakeManager()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeactivateManager()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnGet(ECSEntity entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRemove(ECSEntity entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRemoveManager()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnSet(ECSEntity entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStartManager()
        {
            throw new System.NotImplementedException();
        }
    }
}