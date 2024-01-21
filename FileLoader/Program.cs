using Constants;
using FileLoader.Services;
using FileLoader.Services.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Renci.SshNet;
using SharedServices;
using SharedServices.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10000000;
});
builder.Services.AddControllers();
builder.Services.AddSingleton<IBackgroundFileQueue, BackgroundFileQueue>();
builder.Services.AddScoped<IFileRepository, FileRepository>(p =>
{
    SftpClient client = new(Hosts.HTML_FILE_REPOSITORY_HOST, Hosts.HTML_FILE_REPOSITORY_PORT, Hosts.HTML_FILE_REPOSITORY_USER, Hosts.HTML_FILE_REPOSITORY_PASSWORD);
    string folder = Hosts.HTML_FILE_REPOSITORY_FOLDER;
    return new FileRepository(client, folder);
});
builder.Services.AddHostedService<TaskSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
