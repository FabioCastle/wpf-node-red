using CastleWpf.Utility;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CastleWpf.Core;

/// <summary>
/// Interaction logic for DataProviderWidget.xaml
/// </summary>
public partial class DataProviderWidget : UserControl
{
    public static readonly DependencyProperty ProviderNameProperty =
        DependencyPropertyExtensions.Register<DataProviderWidget, string>(nameof(ProviderName));

    public static readonly DependencyProperty IntValueProperty =
        DependencyPropertyExtensions.Register<DataProviderWidget, int>(nameof(IntValue));

    public static readonly DependencyProperty StringValueProperty =
        DependencyPropertyExtensions.Register<DataProviderWidget, string>(nameof(StringValue));

    public string ProviderName
    {
        get => this.GetValue<string>(ProviderNameProperty);
        set => this.SetValue<string>(ProviderNameProperty, value);
    }

    public int IntValue
    {
        get => this.GetValue<int>(IntValueProperty);
        set => this.SetValue<int>(IntValueProperty, value);
    }
    public string StringValue
    {
        get => this.GetValue<string>(StringValueProperty);
        set => this.SetValue<string>(StringValueProperty, value);
    }

    public DataProviderWidget()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (DataContext is DataProvider provider)
            {
                provider.PropertyChanged += OnProviderPropertyChanged;
                ProviderName = provider.Name;
                IntValue = provider.IntValue;
                StringValue = provider.StringValue;
            }

            DataContextChanged += OnProviderChanged;
        };

        Unloaded += (s, e) =>
        {
            if (DataContext is DataProvider provider)
            {
                provider.PropertyChanged -= OnProviderPropertyChanged;
            }

            DataContextChanged -= OnProviderChanged;
        };
    }

    private void OnProviderChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is DataProvider oldProvider)
        {
            oldProvider.PropertyChanged -= OnProviderPropertyChanged;
        }

        if (e.NewValue is DataProvider newProvider)
        {
            newProvider.PropertyChanged += OnProviderPropertyChanged;
            ProviderName = newProvider.Name;
            IntValue = newProvider.IntValue;
            StringValue = newProvider.StringValue;
        }
    }

    private void OnProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (DataContext is not DataProvider provider)
            return;

        if (e.PropertyName == nameof(DataProvider.IntValue))
        {
            IntValue = provider.IntValue;
        }
        else if (e.PropertyName == nameof(DataProvider.StringValue))
        {
            StringValue = provider.StringValue;
        }
    }

    private void RefreshIntValue(object sender, RoutedEventArgs e)
    {
        if (DataContext is DataProvider provider)
        {
            provider.RefreshIntValue();
        }
    }

    private void RefreshStringValue(object sender, RoutedEventArgs e)
    {
        if (DataContext is DataProvider provider)
        {
            provider.RefreshStringValue();
        }
    }
}