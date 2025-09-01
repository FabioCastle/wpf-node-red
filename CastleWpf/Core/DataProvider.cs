using System.ComponentModel;
using System.Text;

namespace CastleWpf.Core;

internal class DataProvider : INotifyPropertyChanged
{
    private int _intValue = GetRandomIntValue();
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

    private string _stringValue = GetRandomStringValue();
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