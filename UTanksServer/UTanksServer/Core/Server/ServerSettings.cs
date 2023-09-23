using System.Net;

namespace UTanksServer.Core
{
    public struct ServerSettings
    {
        public IPAddress IPAddress { get; set; }
        public short Port { get; set; }
        public int MaxPlayers { get; set; }

        public bool DisableHeightMaps { get; set; }

        public bool DisablePingMessages { get; set; }

        public bool EnableTracing { get; set; }
        public bool EnableCommandStackTrace { get; set; }
        
        public bool MapBoundsInactive { get; set; }
        public bool SuperMegaCoolContainerActive { get; set; }
        public bool TestServer { get; set; }
    }
}
