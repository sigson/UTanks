using System;
using System.Collections.Generic;
using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(636177020058645390L)]
    public sealed class SpecialOfferGroupComponent : GroupComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SpecialOfferGroupComponent() { }
        public SpecialOfferGroupComponent(ECSEntity key) : base(key)
        {
        }

        public SpecialOfferGroupComponent(long key, Dictionary<string, (double, float)> prices) : base(key)
        {
            Prices = prices;
        }

        public readonly Dictionary<string, (double, float)> Prices = new Dictionary<string, (double, float)>();

        public readonly Dictionary<string, string> Currencies = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "BY", "RUB" },
            { "DE", "EUR" },
            { "IT", "EUR" },
            { "RU", "RUB" },
            { "US", "USD" }
        };
    }
}
