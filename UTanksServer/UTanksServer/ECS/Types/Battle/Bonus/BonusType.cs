using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UTanksServer.ECS.Types.Battle.Bonus
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

        public static Dictionary<BonusType, string> ConfigBonuses = new Dictionary<BonusType, string>()
        {
            { BonusType.Repair, "battle\\bonus\\repair"},
            { BonusType.Armor, "battle\\bonus\\armor"},
            { BonusType.Damage, "battle\\bonus\\damage"},
            { BonusType.Speed, "battle\\bonus\\speed"},
            { BonusType.Crystal, "battle\\bonus\\bounty\\crystal"},
            { BonusType.Gold, "battle\\bonus\\gold\\crystal"},
            { BonusType.Ruby, "battle\\bonus\\supergold\\ruby"},
            { BonusType.Container, "battle\\bonus\\bounty\\container"}
        };
    }
}
