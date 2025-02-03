namespace _Framework.MassageBrokers
{
    public class RabbitMQSetting
    {
        public string HostName { get; set; } = null!;
        public int Port { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
