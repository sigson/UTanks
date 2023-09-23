using System;
using System.Collections.Generic;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
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

        public readonly Dictionary<string, (double, float)> Prices = new();

        public readonly Dictionary<string, string> Currencies = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "BY", "RUB" },
            { "DE", "EUR" },
            { "IT", "EUR" },
            { "RU", "RUB" },
            { "US", "USD" }
        };
    }
}
