using System.ComponentModel;
using System.Text;

namespace CastleWpf.Core;

public class DataProvider : INotifyPropertyChanged
{
    public string Name { get; }

    public DataProvider()
    {
        Name = nameof(DataProvider);
        _intValue = GetRandomIntValue();
        _stringValue = GetRandomStringValue();
    }

    public DataProvider(string name)
    {
        Name = name;
        _intValue = GetRandomIntValue();
        _stringValue = GetRandomStringValue();
    }

    private int _intValue;
    public int IntValue
    {
        get => _intValue;
        set
        {
            if (_intValue != value)
            {
                _intValue = value;
                OnPropertyChanged(nameof(IntValue));
            }
        }
    }

    private string _stringValue;
    public string StringValue
    {
        get => _stringValue;
        set
        {
            if (_stringValue != value)
            {
                _stringValue = value;
                OnPropertyChanged(nameof(StringValue));
            }
        }
    }

    public void RefreshIntValue()
    {
        IntValue = GetRandomIntValue();
    }

    private static int GetRandomIntValue()
    {
        var random = new Random();
        return random.Next(100);
    }

    public void RefreshStringValue()
    {
        StringValue = GetRandomStringValue();
    }

    private static string GetRandomStringValue()
    {
        var random = new Random();
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < 8; i++)
        {
            var randomChar = random.Next('A', 'Z');
            stringBuilder.Append((char)randomChar);
        }

        return stringBuilder.ToString();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}