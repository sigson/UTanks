namespace UTanksServer.Core.Database
{
    public struct DatabaseNetworkConfig
    {
        public bool Enabled { get; set; }
        public string HostAddress { get; set; }
        public int HostPort { get; set; }
        public string Key { get; set; }
        public string Token { get; set; }
    }
}
