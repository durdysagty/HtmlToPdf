using Constants;
using Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using PdfConverter.Services.Interfaces;
using PdfConverter.Services;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using SharedServices.Interfaces;
using SharedServices;
using Renci.SshNet;

// Prepearing a service
ConnectionFactory factory = new()
{
    HostName = Hosts.RABBITMQ_HOST,
    Port = Hosts.RABBITMQ_PORT,
    UserName = Hosts.RABBITMQ_NAME,
    Password = Hosts.RABBITMQ_PASSWORD
};
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();
channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
Console.WriteLine("Waiting for messages.");
EventingBasicConsumer consumer = new(channel);
IFileRepository htmlRepository = new FileRepository(new SftpClient(Hosts.HTML_FILE_REPOSITORY_HOST, Hosts.HTML_FILE_REPOSITORY_PORT, Hosts.HTML_FILE_REPOSITORY_USER, Hosts.HTML_FILE_REPOSITORY_PASSWORD), Hosts.HTML_FILE_REPOSITORY_FOLDER);
IFileRepository pdfRepository = new FileRepository(new SftpClient(Hosts.PDF_FILE_REPOSITORY_HOST, Hosts.PDF_FILE_REPOSITORY_PORT, Hosts.PDF_FILE_REPOSITORY_USER, Hosts.PDF_FILE_REPOSITORY_PASSWORD), Hosts.PDF_FILE_REPOSITORY_FOLDER);

// Cancel application
ManualResetEventSlim quitEvent = new(false);
Console.CancelKeyPress += (sender, eArgs) =>
{
    if (channel.IsOpen)
        channel.Close();
    channel.Dispose();
    if (connection.IsOpen)
        connection.Close();
    connection.Dispose();
    quitEvent.Set();
    eArgs.Cancel = true;
};

// Start getting messages
consumer.Received += async (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    string? message = Encoding.UTF8.GetString(body);
    if (!string.IsNullOrEmpty(message))
    {
        Console.WriteLine($"Task {message} received");
        string[] messageParts = message.Split(';');
        Signal signal = new(messageParts[0], messageParts[1], messageParts[2]);
        string htmlFile = await htmlRepository.GetFile(Hosts.HTML_FILE_REPOSITORY_FOLDER, signal.ClientId, signal.FileName, CancellationToken.None);
        IHtmlParser htmlParser = new HtmlParserService();
        if (htmlParser.IsValid(htmlFile))
        {
            using BrowserFetcher browserFetcher = new();
            await browserFetcher.DownloadAsync();
            using IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Timeout = 0,
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });
            using IPage page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlFile);
            PdfOptions pdfOptions = new()
            {
                Format = PaperFormat.A4
            };
            using Stream pdfStream = await page.PdfStreamAsync(pdfOptions);
            pdfStream.Position = 0;
            await pdfRepository.UploadAsync(pdfStream, signal.ClientId, $"{Guid.NewGuid()}.pdf", CancellationToken.None);

        }
        //TODO what if not valid?
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        await htmlRepository.DeleteFile(Hosts.HTML_FILE_REPOSITORY_FOLDER, signal.ClientId, signal.FileName, CancellationToken.None);
    }

    //TODO what if null?
};
channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

// Quit application
quitEvent.Wait();