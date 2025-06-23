using System.Windows;

namespace CastleWpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Services.NodeRedManager.StartNodeRed();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        Services.NodeRedManager.StopNodeRed();
    }
}