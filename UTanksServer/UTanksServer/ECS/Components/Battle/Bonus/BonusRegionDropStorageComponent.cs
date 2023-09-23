using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.ECS.ECSCore;
using UTanksServer.ECS.Types.Battle.Bonus;

namespace UTanksServer.ECS.Components.Battle.Bonus
{
    [TypeUid(236057119736177950)]
    public class BonusRegionDropStorageComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        //mainly storage for crystals drop for split time of dropping
        public List<BonusDropRecord> DropStorage = new List<BonusDropRecord>();
        //this, who taken
        //public Action<ECSEntity, ECSEntity> onTaken;
        public BonusRegionDropStorageComponent() { }
        public BonusRegionDropStorageComponent(List<BonusDropRecord> dropStorage) => DropStorage = dropStorage;
    }

    public class BonusDropRecord : CachingSerializable
    {
        public BonusDropRecord(long timeDrop, BonusType bonustype)
        {
            TimeDrop = timeDrop;
            bonusType = bonustype;
        }

        public BonusDropRecord() { }

        public long TimeDrop;
        public BonusType bonusType;
    }
}
