namespace Chat
{
    public class KafkaConfig
    {
        public const string Position = "Kafka";
        public string BootstrapServers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string ConsumerGroupId { get; set; } = string.Empty;
    }
}
