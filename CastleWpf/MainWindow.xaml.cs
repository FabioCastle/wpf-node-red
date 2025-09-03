using CastleWpf.Core;
using CastleWpf.NodeRed;
using CastleWpf.Utility;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;
using System.Windows;

namespace CastleWpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DataProviderPage _providerPage = new();
    private readonly NodeRedPage _nodeRedPage = new();

    public static readonly DependencyProperty WebView2LoadedProperty =
        DependencyPropertyExtensions.Register<MainWindow, bool>(nameof(WebView2Loaded));

    public bool WebView2Loaded
    {
        get => this.GetValue<bool>(WebView2LoadedProperty);
        set => this.SetValue<bool>(WebView2LoadedProperty, value);
    }

    public MainWindow()
    {
        InitializeComponent();
        
        Loaded += (s, e) =>
        {
            MainFrame.Navigate(_providerPage);
            WebView2Loaded = false;

            //_ = Task.Run(PreloadWebView2Async);
            Dispatcher.BeginInvoke(async () => await PreloadWebView2Async());
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

    private async Task PreloadWebView2Async()
    {
        try
        {
            // Creiamo un'istanza "usa e getta" di WebView2 solo per forzare
            // l'inizializzazione del processo sottostante. Non serve nemmeno aggiungerla all'UI.
            var webView = new WebView2();

            // Avvia l'inizializzazione dell'ambiente CoreWebView2.
            // La prima volta che viene chiamato nell'app, avvia msedgewebview2.exe.
            await webView.EnsureCoreWebView2Async(null);

            // Una volta che l'ambiente è pronto, aggiorniamo la nostra proprietà.
            // L'UI si aggiornerà di conseguenza tramite il binding.
            WebView2Loaded = true;

            // L'oggetto può essere eliminato, il processo in background rimarrà attivo
            // per un po', pronto per la vera istanza di WebView2 sulla tua pagina.
            webView.Dispose();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"(PreloadWebView2Async) {e.Message}\n{e.StackTrace}");
        }
    }
}