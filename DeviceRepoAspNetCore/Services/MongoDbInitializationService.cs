using DeviceControllerLib.Models.MongoDb;
using DeviceControllerLib.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DeviceRepoAspNetCore.Services;

public class MongoDbInitializationService(
    IOptions<MongoDbSettings> mongoDbSettings,
    ILogger<MongoDbInitializationService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var userName = mongoDbSettings.Value.DatabaseUser;
        var password = mongoDbSettings.Value.DatabasePassword;

        var mongoUrl = new MongoUrlBuilder(mongoDbSettings.Value.ConnectionStringAnonymous)
        {
            Username = userName,
            Password = password
        }.ToMongoUrl();

        var client = new MongoClient(mongoUrl);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        var devicesCollection = database.GetCollection<AudioDeviceDocument>("devices");

        var indexKeysDefinition = Builders<AudioDeviceDocument>.IndexKeys
            .Ascending(d => d.PnpId)
            .Ascending(d => d.HostName);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<AudioDeviceDocument>(indexKeysDefinition, indexOptions);

        var maxRetries = mongoDbSettings.Value.MaxConnectRetries;
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                logger.LogInformation("Database (re-)initialization started.");
                await devicesCollection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
                logger.LogInformation("Database (re-)initialization completed successfully.");
                return;
            }
            catch (MongoConnectionException ex) when (ex is not MongoAuthenticationException && attempt < maxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 1s, 2s, 4s, 8s, 16s
                logger.LogWarning(
                    "MongoDB connection failed (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s...",
                    attempt + 1, maxRetries, delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize MongoDB.");
                throw;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
