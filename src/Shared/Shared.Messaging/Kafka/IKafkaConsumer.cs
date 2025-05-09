public interface IKafkaConsumer<TKey, TValue>
    {
        Task ConsumeAsync(string topic, CancellationToken cancellationToken);
        void Subscribe(string topic);
        event EventHandler<KafkaMessageReceivedEventArgs<TKey, TValue>> MessageReceived;
    }

    public class KafkaMessageReceivedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public string Topic { get; set; }
    }