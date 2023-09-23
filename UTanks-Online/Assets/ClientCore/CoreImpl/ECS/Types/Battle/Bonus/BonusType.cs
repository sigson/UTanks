using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UTanksClient.ECS.Types.Battle.Bonus
{
    public enum BonusType
    {
        Repair,
        Armor,
        Damage,
        Speed,
        Crystal,
        Gold,
        Ruby,
        Container,
        SuperContainer,
        TestBox
    }

    public static class BonusMatching
    {
        public static Dictionary<string, BonusType> Bonuses = new Dictionary<string, BonusType>()
        {
            {"medkit",  BonusType.Repair},
            {"armorup",  BonusType.Armor},
            {"damageup",  BonusType.Damage},
            {"nitro",  BonusType.Speed},
            {"crystal",  BonusType.Crystal},
            {"crystal_100",  BonusType.Gold},
            {"crystal_500",  BonusType.Ruby},
            {"spin_box",  BonusType.Container},
            {"super_spin_box",  BonusType.SuperContainer},
            {"",  BonusType.TestBox}
        };
    }
}
