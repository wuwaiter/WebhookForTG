using dotenv.net;
using WebhookForTG.Services;

// Local: load .env if present. Docker: secrets come from env_file / environment.
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: true));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddHttpClient<ILineNotifyService, LineNotifyService>();
builder.Services.AddHttpClient<ITelegramService, TelegramService>();

var app = builder.Build();

// Official ASP.NET images set DOTNET_RUNNING_IN_CONTAINER=true.
// HTTPS is handled by reverse proxy outside the container.
var runningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!runningInContainer)
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
