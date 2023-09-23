using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components
{
    [TypeUid(1453796862447)]
    public sealed class ClientLocaleComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public ClientLocaleComponent() { }
        public ClientLocaleComponent(string LocaleCode)
        {
            this.LocaleCode = LocaleCode;
        }

        public string LocaleCode { get; set; }
    }
}
