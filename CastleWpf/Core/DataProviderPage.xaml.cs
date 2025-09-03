using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace CastleWpf.Core;

/// <summary>
/// Interaction logic for DataProviderPage.xaml
/// </summary>
public partial class DataProviderPage : Page
{
    public ObservableCollection<DataProvider> DataProviders { get; } = [];
    private bool _dataProvidersRegistered = false;

    public DataProviderPage()
    {
        InitializeComponent();

        DataProviders.Add(new DataProvider("OKUMA MULTUS U3000"));
        DataProviders.Add(new DataProvider("OKUMA LB-300"));
        DataProviders.Add(new DataProvider("SIEMENS S-7"));

        Loaded += (s, e) =>
        {
            if (!_dataProvidersRegistered)
            {
                foreach (var dataProvider in DataProviders)
                {
                    Services.MqttService.Register(dataProvider);
                }

                _dataProvidersRegistered = true;
            }
        };
    }
}