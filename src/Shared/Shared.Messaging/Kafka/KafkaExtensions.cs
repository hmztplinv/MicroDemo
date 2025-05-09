using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Kafka;

public static class KafkaExtensions
    {
        public static IServiceCollection AddKafkaProducer<TKey, TValue>(
            this IServiceCollection services, 
            string bootstrapServers, 
            Action<ProducerConfig> configureProducer = null)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All
            };

            configureProducer?.Invoke(producerConfig);

            services.AddSingleton<IKafkaProducer<TKey, TValue>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KafkaProducer<TKey, TValue>>>();
                return new KafkaProducer<TKey, TValue>(producerConfig, logger);
            });

            return services;
        }

        public static IServiceCollection AddKafkaConsumer<TKey, TValue>(
            this IServiceCollection services, 
            string bootstrapServers, 
            string groupId, 
            Action<ConsumerConfig> configureConsumer = null)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            configureConsumer?.Invoke(consumerConfig);

            services.AddSingleton<IKafkaConsumer<TKey, TValue>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<KafkaConsumer<TKey, TValue>>>();
                return new KafkaConsumer<TKey, TValue>(consumerConfig, logger);
            });

            return services;
        }
    }