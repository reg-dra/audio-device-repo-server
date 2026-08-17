using DeviceController.SmokeHost;
using DeviceControllerLib.Controllers;
using DeviceControllerLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IChecksumService, FakeChecksumService>();
builder.Services.AddSingleton<IAudioDeviceStorage, InMemoryAudioDeviceStorage>();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(AudioDevicesController).Assembly);

var app = builder.Build();

app.MapControllers();

app.Run();
