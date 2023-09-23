using UTanksClient.Core.Protocol;
using UTanksClient.ECS.ECSCore;

namespace UTanksClient.ECS.Components
{
    [TypeUid(1439792100478)]
    public sealed class SessionSecurityPublicComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        public SessionSecurityPublicComponent() { }

        public SessionSecurityPublicComponent(string publicKey)
            => PublicKey = publicKey;

        public string PublicKey { get; set; } = "AI5q8XLJibe9vwx50OoS4A6nHai3oNd6U3ct96535B3azEoHfWKXQYOV6CbJfXUOBAoUvDzVbJGiOXPED9k0jAM=:AQAB"; // hardcoded value!!!
    }
}
