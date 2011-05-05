using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TaskDialogInterop
{
	/// <summary>
	/// Converts a null check into a negated visibility value.
	/// </summary>
	internal class NotNullVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
			{
				throw new InvalidOperationException();
			}
			return ((value != null) ? Visibility.Visible : Visibility.Collapsed);
		}
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	/// <summary>
	/// Chops up multiline command link text appropriately.
	/// </summary>
	internal class CommandLinkTextConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return null;

			if (value.GetType() != typeof(String))
			{
				throw new InvalidOperationException();
			}
			if (targetType != typeof(String))
			{
				throw new InvalidOperationException();
			}

			if (parameter == null || parameter.ToString() == "1")
			{
				if (value.ToString().Contains("\n"))
				{
					return value.ToString().Substring(0, value.ToString().IndexOf("\n"));
				}
				else
				{
					return value;
				}
			}
			else if (parameter.ToString() == "2")
			{
				if (value.ToString().Contains("\n"))
				{
					return value.ToString().Substring(value.ToString().IndexOf("\n") + 1);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return value;
			}
		}
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	/// <summary>
	/// Determines visibility for command link extra text.
	/// </summary>
	internal class CommandLinkExtraTextVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			if (value.GetType() != typeof(String))
			{
				throw new InvalidOperationException();
			}
			if (targetType != typeof(Visibility))
			{
				throw new InvalidOperationException();
			}

			return (String.IsNullOrEmpty(value.ToString()) || !value.ToString().Contains("\n")) ? Visibility.Collapsed : Visibility.Visible;
		}
		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
