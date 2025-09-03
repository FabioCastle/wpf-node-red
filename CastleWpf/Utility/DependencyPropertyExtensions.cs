using System.Windows;

namespace CastleWpf.Utility;

internal static class DependencyPropertyExtensions
{
    /// <summary>
    /// Ritorna il valore corrente di una <see cref="DependencyProperty"/> in maniera thread-safe: viene garantita l'esecuzione nello UI Thread.
    /// </summary>
    /// <typeparam name="T">Tipo del valore di ritorno.</typeparam>
    /// <param name="dependencyObject">L'oggetto che continene la <see cref="DependencyProperty"/>.</param>
    /// <param name="property">La <see cref="DependencyProperty"/> da leggere.</param>
    public static T GetValue<T>(this DependencyObject dependencyObject, DependencyProperty property)
    {
        if (dependencyObject.Dispatcher.CheckAccess())
        {
            return (T)dependencyObject.GetValue(property);
        }
        else
        {
            return dependencyObject.Dispatcher.Invoke(() => (T)dependencyObject.GetValue(property));
        }
    }

    /// <summary>
    /// Imposta il valore di una <see cref="DependencyProperty"/> in maniera thread-safe: viene garantita l'esecuzione nello UI Thread.
    /// </summary>
    /// <typeparam name="T">Tipo del valore di ritorno.</typeparam>
    /// <param name="dependencyObject">L'oggetto che continene la <see cref="DependencyProperty"/>.</param>
    /// <param name="property">La <see cref="DependencyProperty"/> da scrivere.</param>
    /// <param name="value">Il valore da impostare.</param>
    public static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value)
    {
        if (dependencyObject.Dispatcher.CheckAccess())
        {
            dependencyObject.SetValue(property, value);
        }
        else
        {
            dependencyObject.Dispatcher.Invoke(() => dependencyObject.SetValue(property, value));
        }
    }

    /// <summary>
    /// Registra una dependency property usando i tipi di UserControl e di proprietà specificati.
    /// NOTA: Questo metodo semplifica il fatto di dover cambiare a mano typeof(Property) e typeof(UserControl) del metodo originale.
    /// </summary>
    /// <typeparam name="T_UserControl">Tipo di UserControl che registra la dependency property.</typeparam>
    /// <typeparam name="T_Property">Tipo di proprietà esposta tramite dependency property.</typeparam>
    /// <param name="name">Nome della proprietà associata. Usare nameof(Property) inserendo il nome della proprietà da rendere dependency property.</param>
    public static DependencyProperty Register<T_UserControl, T_Property>(string name)
    {
        return DependencyProperty.Register(name, typeof(T_Property), typeof(T_UserControl));
    }

    /// <summary>
    /// Registra una dependency property usando i tipi di UserControl e di proprietà specificati.
    /// NOTA: Questo metodo semplifica il fatto di dover cambiare a mano typeof(Property) e typeof(UserControl) del metodo originale.
    /// </summary>
    /// <typeparam name="T_UserControl">Tipo di UserControl che registra la dependency property.</typeparam>
    /// <typeparam name="T_Property">Tipo di proprietà esposta tramite dependency property.</typeparam>
    /// <param name="name">Nome della proprietà associata. Usare nameof(Property) inserendo il nome della proprietà da rendere dependency property.</param>
    /// <param name="metadata">Può essere usata per impostare un valore iniziale e una callback quando il valore della dependency property cambia.</param>
    public static DependencyProperty Register<T_UserControl, T_Property>(string name, PropertyMetadata metadata)
    {
        return DependencyProperty.Register(name, typeof(T_Property), typeof(T_UserControl), metadata);
    }
}