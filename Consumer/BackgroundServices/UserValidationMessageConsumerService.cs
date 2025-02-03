using _Framework.MassageBrokers;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;


namespace Consumer.BackgroundServices
{
    public class UserValidationMessageConsumerService(IConnection connection, IChannel channel) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartConsumingAsync(RabbitMQQueues.OrderValidationQueue, stoppingToken);
            await Task.CompletedTask;
        }

        private async Task StartConsumingAsync(string queueName, CancellationToken cancellationToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672
                };
                connection = await factory.CreateConnectionAsync();
                channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    bool processedSuccessfully = true;
                    try
                    {
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
            catch (Exception e)
            {

                throw;
            }
          
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
