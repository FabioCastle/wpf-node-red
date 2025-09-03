using CastleWpf.Core;
using System.ComponentModel;

namespace CastleWpf.Mqtt;

internal class CastleMqttService
{
    private readonly List<MqttDataProvider> _dataProviders = [];
    private static readonly CastleMqttBrokerConfiguration _brokerConfiguration = new("localhost", "castle-mqtt", "castle-mqtt");
    private readonly CastleMqttBroker _broker = new(_brokerConfiguration);
    private CancellationTokenSource? _cancellationTokenSource;

    public void StartMqttBroker()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _broker.StartMqttBroker(_cancellationTokenSource.Token);
    }

    public void StopMqttBroker()
    {
        _cancellationTokenSource?.Cancel();
        _broker.StopMqttBroker();
    }

    public void Register(DataProvider provider)
    {
        _dataProviders.Add(new MqttDataProvider(provider, _brokerConfiguration, _cancellationTokenSource?.Token ?? CancellationToken.None));
    }

    public void Unregister(DataProvider provider)
    {
        var mqttProvider = _dataProviders.Find(dp => dp.Provider == provider);
        if (mqttProvider != null)
        {
            _dataProviders.Remove(mqttProvider);
            mqttProvider.Dispose();
        }
    }
}