using System.Windows;
using System.Windows.Controls;

namespace CastleWpf.Core;

/// <summary>
/// Interaction logic for DataProviderPage.xaml
/// </summary>
public partial class DataProviderPage : Page
{
    private readonly DataProvider _provider = new();

    public DataProviderPage()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            DataContext = _provider;
        };
    }

    private void RefreshIntValue(object sender, RoutedEventArgs e)
    {
        _provider.RefreshIntValue();
    }

    private void RefreshStringValue(object sender, RoutedEventArgs e)
    {
        _provider.RefreshStringValue();
    }
}