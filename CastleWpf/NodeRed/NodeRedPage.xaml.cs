using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Controls;

namespace CastleWpf.NodeRed;

/// <summary>
/// Interaction logic for NodeRedPage.xaml
/// </summary>
public partial class NodeRedPage : Page
{
    public NodeRedPage()
    {
        InitializeComponent();

        Loaded += WaitForNodeRed;
    }

    private async void WaitForNodeRed(object sender, RoutedEventArgs e)
    {
        await Services.NodeRedService.WaitForNodeReadReadyAsync();

        NodeRedWebView.Source = new Uri("http://127.0.0.1:1880");
        NodeRedWebView.NavigationCompleted += OnNavigationCompleted;
    }

    private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        LoadingPanel.Visibility = Visibility.Collapsed;
        NodeRedWebView.NavigationCompleted -= OnNavigationCompleted;
    }
}