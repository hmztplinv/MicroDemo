public interface IUserEventProducer
    {
        Task PublishUserCreatedAsync(ApplicationUser user);
    }