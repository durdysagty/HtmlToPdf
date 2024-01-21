using Constants;
using FileLoader.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace FileLoader.Services
{
    public class TaskSender : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ConnectionFactory _factory;
        private readonly ILogger<TaskSender> _logger;

        public TaskSender(IServiceProvider sp, ILogger<TaskSender> logger)
        {
            _sp = sp;
            _factory = new()
            {
                HostName = Hosts.RABBITMQ_HOST,
                Port = Hosts.RABBITMQ_PORT,
                UserName = Hosts.RABBITMQ_NAME,
                Password = Hosts.RABBITMQ_PASSWORD
            };
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 1;
            bool sent = true;
            string? signal = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = _sp.CreateScope();
                    IBackgroundFileQueue fileQueue = scope.ServiceProvider.GetRequiredService<IBackgroundFileQueue>();
                    if (sent)
                        signal = await fileQueue.DequeueFileAsync(stoppingToken);
                    if (signal != null)
                    {
                        using IConnection connection = _factory.CreateConnection();
                        using IModel channel = connection.CreateModel();
                        channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        byte[] body = Encoding.UTF8.GetBytes(signal);
                        channel.BasicPublish(string.Empty, "task_queue", properties, body);
                        if (i > 1)
                            i = 1;
                        sent = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{message}", ex.Message);
                    await Task.Delay(i * 2000, stoppingToken);
                    i++;
                    sent = false;
                }
            }
        }
    }
}
