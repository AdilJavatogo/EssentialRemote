using System.Globalization;

namespace EssentialRemote.Converters;

public class CenterToCornerConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double coordinate)
        {
            // Knoppen er 60x60, så radius er 30. Vi forskyder den med 30 for at centrere den.
            return coordinate - 30;
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}