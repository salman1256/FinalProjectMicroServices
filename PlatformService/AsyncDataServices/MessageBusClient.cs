using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
namespace PlatformService.AsyncDataServices
{

    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])

            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
                Console.WriteLine("Connected to Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to Connect the Message Bus: " + ex.Message);
            }
        }

        private void  RabbitMQ_ConnectionShutDown(object sender,ShutdownEventArgs e)
        {
        Console.WriteLine("RabbiMQ Connection Shutdown");
        }
        public void Dispose()
        {
            System.Console.WriteLine("Message Bus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message=JsonSerializer.Serialize(platformPublishedDto);
            if(_connection.IsOpen)
            {
                System.Console.WriteLine("Rabbit MQ Connection open , start sending message...");
                SendMessage(message);
            }
            else{
                System.Console.WriteLine("RabbitMQ connection closed, not sending the messsage");
            }
        }

        private void SendMessage(string message)
        {
           var body=Encoding.UTF8.GetBytes(message);
           _channel.BasicPublish(exchange:"trigger",routingKey:"",basicProperties:null,body:body);
           System.Console.WriteLine($"We have sent {message}");
        }
    }
}
