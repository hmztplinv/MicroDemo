using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>, IDisposable
    {
        private readonly IConsumer<TKey, string> _consumer;
        private readonly ILogger<KafkaConsumer<TKey, TValue>> _logger;
        private bool _disposed;

        public event EventHandler<KafkaMessageReceivedEventArgs<TKey, TValue>> MessageReceived;

        public KafkaConsumer(ConsumerConfig consumerConfig, ILogger<KafkaConsumer<TKey, TValue>> logger)
        {
            _consumer = new ConsumerBuilder<TKey, string>(consumerConfig).Build();
            _logger = logger;
        }

        public void Subscribe(string topic)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer<TKey, TValue>));
            }

            _consumer.Subscribe(topic);
            _logger.LogInformation("Subscribed to topic {Topic}", topic);
        }

        public async Task ConsumeAsync(string topic, CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer<TKey, TValue>));
            }

            Subscribe(topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        if (consumeResult != null)
                        {
                            var value = JsonSerializer.Deserialize<TValue>(consumeResult.Message.Value);
                            var args = new KafkaMessageReceivedEventArgs<TKey, TValue>
                            {
                                Key = consumeResult.Message.Key,
                                Value = value,
                                Topic = consumeResult.Topic
                            };

                            OnMessageReceived(args);
                            _logger.LogInformation("Consumed message from topic {Topic}: {Key}", topic, consumeResult.Message.Key);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message from topic {Topic}", topic);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumption from topic {Topic} cancelled", topic);
                _consumer.Close();
            }
        }

        protected virtual void OnMessageReceived(KafkaMessageReceivedEventArgs<TKey, TValue> e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _consumer.Dispose();
            _disposed = true;
        }
    }