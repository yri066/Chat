namespace Chat.Domain.Entities
{
    public class KafkaConfig
    {
        public const string Position = "Kafka";
        public string BootstrapServers { get; init; } = string.Empty;
        public string Topic { get; init; } = string.Empty;
        public string ConsumerGroupId { get; init; } = string.Empty;
    }
}
