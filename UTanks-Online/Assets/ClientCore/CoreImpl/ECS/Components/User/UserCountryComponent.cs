using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1470735489716)]
    public class UserCountryComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public UserCountryComponent() { }
        public UserCountryComponent(string CountryCode)
        {
            this.CountryCode = CountryCode;
        }

        public string CountryCode { get; set; }
    }
}
