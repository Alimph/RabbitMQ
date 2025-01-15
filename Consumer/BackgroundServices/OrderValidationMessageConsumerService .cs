using _Framework.MassageBrokers;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;


namespace Consumer.BackgroundServices
{
    public class OrderValidationMessageConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private readonly RabbitMQSetting _rabbitMqSetting;
        private readonly IChannel _channel;
        public OrderValidationMessageConsumerService(IOptions<RabbitMQSetting> rabbitMqSetting, IServiceProvider serviceProvider)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSetting.HostName,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };
            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartConsumingAsync(RabbitMQQueues.OrderValidationQueue, stoppingToken);
            await Task.CompletedTask;
        }

        private async Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
        {
            await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                bool processedSuccessfully = false;
                try
                {
                }
                catch (Exception ex)
                {
                    //_logger.LogError($"Exception occurred while processing message from queue {queueName}: {ex}");
                }

                if (processedSuccessfully)
                {
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    await _channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

        public override void Dispose()
        {
            _channel.CloseAsync().Wait();
            _connection.CloseAsync().Wait();
            base.Dispose();
        }
    }
}
