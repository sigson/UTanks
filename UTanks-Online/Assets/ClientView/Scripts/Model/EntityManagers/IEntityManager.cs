using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTanksClient;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IEntityManager : IECSObjectManager<ECSEntity>
    {
        public ECSEntity ManagerEntity
        {
            get
            {
                return this.ManagerECSObject;
            }
            set
            {
                this.ManagerECSObject = value;
            }
        }

        public long ManagerEntityId => this.ManagerECSObjectId;
    }
}