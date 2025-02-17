using _Framework.MassageBrokers;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using _Framework.Aggr;


namespace Consumer.BackgroundServices
{
    public class UserValidationMessageConsumerService() : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Delay(60_000).Wait();
            await StartConsumingAsync(RabbitMQQueues.UserValidationQueue, stoppingToken);
            await Task.CompletedTask;
        }

        private async Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
        {

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var userValidationModel = JsonConvert.DeserializeObject<UserValidationModel>(message);
                bool processedSuccessfully = true;
                try
                {
                    processedSuccessfully = UserValid(userValidationModel);
                }
                catch (Exception ex)
                {
                    //_logger.LogError($"Exception occurred while processing message from queue {queueName}: {ex}");
                }

                if (processedSuccessfully)
                {
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    await channel.BasicRejectAsync(deliveryTag: ea.DeliveryTag, requeue: true);
                }
            };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private bool UserValid(UserValidationModel model)
        {
            if (model.Age > 18 && model.Address.Any())
                return true;
            return false;
        }
    }
}
