using CastleWpf.Core;
using CastleWpf.NodeRed;
using System.Windows;

namespace CastleWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DataProviderPage _providerPage = new();
    private readonly NodeRedPage _nodeRedPage = new();

    public MainWindow()
    {
        InitializeComponent();
        
        Loaded += (s, e) =>
        {
            MainFrame.Navigate(_providerPage);
        };
    }

    private void NavigateHome(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_providerPage);
    }

    private void NavigateNodeRed(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_nodeRedPage);
    }
}