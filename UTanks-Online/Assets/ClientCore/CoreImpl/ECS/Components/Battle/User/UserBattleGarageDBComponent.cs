using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Components.User;
using UTanksClient.ECS.ECSCore;
using UTanksClient.ECS.Types;

namespace UTanksClient.ECS.Components.Battle
{
    [TypeUid(625367836408507420)]
    public class UserBattleGarageDBComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public GarageData battleEquipment;
        //public SelectedEquipmentData selectedEquipment
        //{
        //    get
        //    {
        //        if (ownerEntity != null)
        //            return this.ownerEntity.GetComponent<UserGarageDBComponent>(UserGarageDBComponent.Id).selectedEquipment;
        //        else
        //            return null;
        //    }
        //    set
        //    {
        //        selectedEquipmentReal = value;
        //    }
        //}

    }
}
