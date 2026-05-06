using MassTransit;
using SpendSmart.Shared.Events;
using SpendSmart.Category.API.Services;

namespace SpendSmart.Category.API.Consumers
{
    public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<UserRegisteredConsumer> _logger;

        public UserRegisteredConsumer(ICategoryService categoryService, ILogger<UserRegisteredConsumer> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var data = context.Message;
            _logger.LogInformation("New user registered: {UserId} ({Email}). Global defaults will be used.", data.UserId, data.Email);
        }
    }
}
