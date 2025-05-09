using Shared.Messaging.Models;

public class UserEventProducer : IUserEventProducer
    {
        private readonly IKafkaProducer<string, UserCreatedMessage> _kafkaProducer;
        private readonly IConfiguration _configuration;

        public UserEventProducer(
            IKafkaProducer<string, UserCreatedMessage> kafkaProducer,
            IConfiguration configuration)
        {
            _kafkaProducer = kafkaProducer;
            _configuration = configuration;
        }

        public async Task PublishUserCreatedAsync(ApplicationUser user)
        {
            var message = new UserCreatedMessage
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedDate = user.CreatedDate
            };

            var topic = _configuration["Kafka:Topics:UserCreated"];
            await _kafkaProducer.ProduceAsync(topic, user.Id, message);
        }
    }