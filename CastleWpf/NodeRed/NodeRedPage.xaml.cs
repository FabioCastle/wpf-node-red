using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Controls;

namespace CastleWpf.NodeRed
{
    /// <summary>
    /// Interaction logic for NodeRedPage.xaml
    /// </summary>
    public partial class NodeRedPage : Page
    {
        private readonly ManualResetEventSlim _viewLoaded = new(false);

        public NodeRedPage()
        {
            InitializeComponent();

            Loaded += WaitForNodeRed;
        }

        private async void WaitForNodeRed(object sender, RoutedEventArgs e)
        {
            _viewLoaded.Reset();

            await Services.NodeRedManager.WaitForNodeReadReadyAsync();

            NodeRedWebView.Source = new Uri("http://127.0.0.1:1880");
            NodeRedWebView.NavigationCompleted += OnNavigationCompleted;

            await Task.Run(_viewLoaded.Wait);
            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _viewLoaded.Set();
            NodeRedWebView.NavigationCompleted -= OnNavigationCompleted;
        }
    }
}
