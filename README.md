# Rabbit MQ Example

Rabbit MQ Example is a C# project for use rabbit mq.

## Installation

Must first install the [rabbitmq](https://hub.docker.com/_/rabbitmq) server.

```bash
docker pull rabbitmq

docker run -d --hostname my-rabbit --name some-rabbit -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password rabbitmq:3-management

```

## Usage

```c#
 using var connection = await factory.CreateConnectionAsync();
 using var channel = await connection.CreateChannelAsync();
 await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null, noWait: false);

 var messageJson = JsonConvert.SerializeObject(message);
 var body = Encoding.UTF8.GetBytes(messageJson);

 await Task.Run(async () => await channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: false, body: body));
```

## License

[Win]()
