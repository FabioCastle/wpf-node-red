using CastleWpf.Mqtt;
using CastleWpf.NodeRed;

namespace CastleWpf;

internal static class Services
{
    public static NodeRedService NodeRedService { get; } = new NodeRedService();
    public static CastleMqttService MqttService { get; } = new CastleMqttService();
}