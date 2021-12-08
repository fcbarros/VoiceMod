namespace ChatService
{
    public class AppSettings
    {
        public CZeroMq ZeroMq { get; set; }

        public class CZeroMq
        {
            public int Port { get; set; }
        }
    }
}
