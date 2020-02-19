using System;
using System.Globalization;
using System.Windows.Data;

[ValueConversion(typeof(string), typeof(bool?))]
internal class ConverterYN2TF : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		switch (System.Convert.ToString(value))
		{
		case "Y":
			return true;
		case "N":
			return false;
		default:
			return null;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		switch (new bool?(System.Convert.ToBoolean(value)))
		{
		case true:
			return "Y";
		case false:
			return "N";
		default:
			return "Null";
		}
	}
}
