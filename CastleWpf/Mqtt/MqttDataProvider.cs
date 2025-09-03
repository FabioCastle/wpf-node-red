using CastleWpf.Core;
using MQTTnet;
using MQTTnet.Protocol;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace CastleWpf.Mqtt;

internal class MqttDataProvider : IDisposable
{
    private readonly IMqttClient _client;
    private readonly Guid _clientId;
    private readonly DataProvider _provider;
    private readonly MqttClientOptions _options;
    private readonly CancellationToken _cancellationToken;
    private bool _connected = false;

    public DataProvider Provider => _provider;
    private string DataProviderTopic => $"/data_providers/{_clientId}";

    public MqttDataProvider(DataProvider _dataProvider, CastleMqttBrokerConfiguration _configuration, CancellationToken cancellationToken)
    {
        _provider = _dataProvider;
        _clientId = Guid.NewGuid();
        _cancellationToken = cancellationToken;

        _client = new MqttClientFactory().CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithClientId(_clientId.ToString())
            .WithCredentials(_configuration.Username, _configuration.Password)
            .WithTcpServer(_configuration.TcpServer)
            .Build();

        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        _client.ApplicationMessageReceivedAsync += OnApplicationReceivedAsync;
        _provider.PropertyChanged += OnDataProviderPropertyChanged;
    }

    private record DataProviderData(int IntValue, string StringValue);

    private async Task OnApplicationReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        if (args.ApplicationMessage == null)
            return;

        if (args.ApplicationMessage.Topic == DataProviderTopic)
        {
            try
            {
                var data = JsonSerializer.Deserialize<DataProviderData>(args.ApplicationMessage.Payload.ToArray());

                if (data != null)
                {
                    _provider.IntValue = data.IntValue;
                    _provider.StringValue = data.StringValue;
                }

                Debug.WriteLine($"(DataProvider {_clientId}) Received data: {JsonSerializer.Serialize(data)}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(DataProvider {_clientId}) {e.Message}\n{e.StackTrace}");
            }
        }

        await Task.CompletedTask;

        // NOTE: There is no NACK mechanism to force the server to re-send the message (implemented in v5 but this specific case may not be tackled).
        // SO: the callback should handle cases when the request can't be performed, either writing an error response or having a cycling retry policy
        // (maybe until a command that disables this execution arrives).
    }

    private void OnDataProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var data = new DataProviderData(_provider.IntValue, _provider.StringValue);
        var payload = JsonSerializer.SerializeToUtf8Bytes(data);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(DataProviderTopic)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .WithPayload(payload)
            .Build();

        Task.Run(async () => await SendMqttMessageAsync(message)).Wait();
    }

    private async Task SendMqttMessageAsync(MqttApplicationMessage message)
    {
        if (!_connected)
        {
            try
            {
                await _client.ConnectAsync(_options, _cancellationToken);
                Debug.WriteLine($"(DataProvider {_clientId}) Connected to broker");
                _connected = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"(DataProvider {_clientId}) Error while connecting to the broker: {e.Message}\n{e.StackTrace}");
                return;
            }
        }

        try
        {
            await _client.PublishAsync(message, _cancellationToken);
            Debug.WriteLine($"(DataProvider {_clientId}) Sent message on topic {message.Topic}");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"(DataProvider {_clientId}) Error while sending message on topic {message.Topic}: {e.Message}\n{e.StackTrace}");
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}