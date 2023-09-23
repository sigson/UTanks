using System;
using UTanksServer.Core.Protocol;
using UTanksServer.ECS.ECSCore;

namespace UTanksServer.ECS.Components.User
{
    [TypeUid(227829636162916000)]
    public class RegistrationDateComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public RegistrationDateComponent() { }
        public RegistrationDateComponent(long date) {
            Date = date;
        }

        [OptionalMapped] public long? Date { get; set; } = null;
    }
}
