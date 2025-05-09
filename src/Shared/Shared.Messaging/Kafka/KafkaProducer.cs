using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Shared.Messaging.Kafka
{
    public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>, IDisposable
    {
        private readonly IProducer<TKey, string> _producer;
        private readonly ILogger<KafkaProducer<TKey, TValue>> _logger;
        private bool _disposed;

        public KafkaProducer(ProducerConfig producerConfig, ILogger<KafkaProducer<TKey, TValue>> logger)
        {
            _producer = new ProducerBuilder<TKey, string>(producerConfig).Build();
            _logger = logger;
        }

        public async Task ProduceAsync(string topic, TKey key, TValue value)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaProducer<TKey, TValue>));
            }

            try
            {
                var message = JsonSerializer.Serialize(value);
                await _producer.ProduceAsync(topic, new Message<TKey, string> { Key = key, Value = message });
                _logger.LogInformation("Produced message to topic {Topic}: {Key}", topic, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error producing message to topic {Topic}: {Key}", topic, key);
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _producer.Dispose();
            _disposed = true;
        }
    }
}