namespace CastleWpf.Mqtt;

internal class CastleMqttBrokerConfiguration(string tcpServer, string username, string password)
{
    public string TcpServer { get; set; } = tcpServer;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
}