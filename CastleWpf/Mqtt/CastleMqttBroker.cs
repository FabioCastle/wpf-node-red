using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Buffers;
using System.IO;
using System.Net;
using System.Text.Json;

namespace CastleWpf.Mqtt;

internal class CastleMqttBroker(CastleMqttBrokerConfiguration _configuration)
{
    private readonly string STORAGE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Mqtt", "retained_messages.json");
    
    private MqttServer? _server;

    public void StartMqttBroker(CancellationToken cancellationToken)
    {
        Task.Run(async () => await StartMqttBrokenAsync(cancellationToken), cancellationToken).Wait(cancellationToken);
    }

    public void StopMqttBroker()
    {
        Task.Run(StopMqttBrokerAsync).Wait();
        //Logger.LogInformation("MQTT broker has stopped.");
    }

    private async Task StartMqttBrokenAsync(CancellationToken cancellationToken)
    {
        // The port for the default endpoint is localhost on port 1883.
        // The default endpoint is NOT encrypted!
        // Use the builder classes where possible.
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();

        if (IPAddress.TryParse(_configuration.TcpServer, out var ip))
        {
            options = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(ip)
                .Build();

            // The port can be changed using the following API (not used in this example).
            // new MqttServerOptionsBuilder()
            //     .WithDefaultEndpoint()
            //     .WithDefaultEndpointPort(1234)
            //     .Build();
        }

        _server = new MqttServerFactory().CreateMqttServer(options);

        // Setup connection validation before starting the server so that there is
        // no change to connect without valid credentials.
        _server.ValidatingConnectionAsync += e =>
        {
            // if (e.ClientId != "ValidClientId")
            // {
            //     e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            // }

            if (e.UserName != _configuration.Username)
            {
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            if (e.Password != _configuration.Password)
            {
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            return Task.CompletedTask;
        };

        _server.LoadingRetainedMessageAsync += async args =>
        {
            try
            {
                var models = await JsonSerializer.DeserializeAsync<List<CMqttRetainedMessage>>(File.OpenRead(STORAGE_PATH)) ?? [];
                var retainedMessages = models.Select(m => m.ToApplicationMessage()).ToList();

                args.LoadedRetainedMessages = retainedMessages;
                //Logger.LogInformation("Loaded retained messages from file {StoragePath}", storagePath);
            }
            catch (FileNotFoundException)
            {
                //Logger.LogWarning("File {StoragePath} was not found", storagePath);
            }
            catch (Exception e)
            {
                //Logger.LogError("Error reading retained messages: {Message}\n{StackTrace}", e.Message, e.StackTrace);
            }
        };

        _server.RetainedMessageChangedAsync += async args =>
        {
            try
            {
                var models = args.StoredRetainedMessages.Select(CMqttRetainedMessage.FromApplicationMessage);
                var jsonContent = JsonSerializer.Serialize(models);
                await File.WriteAllTextAsync(STORAGE_PATH, jsonContent, cancellationToken);
                //Logger.LogInformation("Retained messages stored to file {StoragePath}", storagePath);
            }
            catch (Exception e)
            {
                //Logger.LogError("Error writing retained messages: {Message}\n{StackTrace}", e.Message, e.StackTrace);
            }
        };

        _server.RetainedMessagesClearedAsync += _ =>
        {
            File.Delete(STORAGE_PATH);
            //Logger.LogInformation("Deleted retained messages stored to file {StoragePath}", storagePath);
            return Task.CompletedTask;
        };

        await _server.StartAsync();

        //Logger.LogInformation("MQTT Broker has started.");
    }

    private async Task StopMqttBrokerAsync()
    {
        if (_server == null)
            return;

        var connectedClients = await _server.GetClientsAsync() ?? [];
        foreach (var client in connectedClients)
        {
            await client.DisconnectAsync();
        }

        // Stop and dispose the MQTT server if it is no longer needed!
        await _server.StopAsync();
        _server.Dispose();
    }

    private record CMqttRetainedMessage(string? Topic, byte[]? Payload, List<MqttUserProperty>? UserProperties, string? ResponseTopic, byte[]? CorrelationData, string? ContentType, MqttPayloadFormatIndicator PayloadFormatIndicator, MqttQualityOfServiceLevel QualityOfServiceLevel)
    {
        public static CMqttRetainedMessage FromApplicationMessage(MqttApplicationMessage message)
        {
            return message == null
                ? throw new ArgumentNullException(nameof(message))
                : new CMqttRetainedMessage(message.Topic, message.Payload.ToArray(), message.UserProperties, message.ResponseTopic, message.CorrelationData, message.ContentType, message.PayloadFormatIndicator, message.QualityOfServiceLevel);
        }

        public MqttApplicationMessage ToApplicationMessage()
        {
            return new MqttApplicationMessage
            {
                Topic = Topic,
                PayloadSegment = new ArraySegment<byte>(Payload ?? []),
                UserProperties = UserProperties,
                ResponseTopic = ResponseTopic,
                CorrelationData = CorrelationData,
                ContentType = ContentType,
                PayloadFormatIndicator = PayloadFormatIndicator,
                QualityOfServiceLevel = QualityOfServiceLevel,
                Dup = false,
                Retain = true
            };
        }
    }
}