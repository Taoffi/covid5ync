using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace iDna.controls
{

	public class Bool2Visibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value == null)
				return Visibility.Collapsed;

			bool			bvalue	= false;

			if(! bool.TryParse(value.ToString(), out bvalue))
				return Visibility.Collapsed;

			return bvalue ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ReverseBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return true;

			bool bvalue = false;

			if (!bool.TryParse(value.ToString(), out bvalue))
				return true;

			return ! bvalue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class iDnaBaseCode2String : IValueConverter
	{
		public object Convert(object nodeValue, Type targetType, object parameter, CultureInfo culture)
		{
			iDnaBase		node	= nodeValue as iDnaBase;

			if(node == null)
				return null;

			return node.Code.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class iDnaNodeCode2String : IValueConverter
	{
		public object Convert(object nodeValue, Type targetType, object parameter, CultureInfo culture)
		{
			iDnaNode		node	= nodeValue as iDnaNode;

			if(node == null)
				return null;

			return node.Code.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class iDnaBaseColor2Brush : IValueConverter
	{
		static		SolidColorBrush		_defaultBrush	= new SolidColorBrush(Colors.LightGray);
		public object Convert(object nodeValue, Type targetType, object parameter, CultureInfo culture)
		{
			iDnaBase		node	= nodeValue as iDnaBase;

			if(node == null)
				return _defaultBrush;

			Color	color	= node.Color;

			return new SolidColorBrush(color);
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}


	public class iDnaNodeColor2Brush : IValueConverter
	{
		static		SolidColorBrush		_defaultBrush	= new SolidColorBrush(Colors.LightGray);
		public object Convert(object nodeValue, Type targetType, object parameter, CultureInfo culture)
		{
			iDnaNode		node	= nodeValue as iDnaNode;

			if(node == null)
				return _defaultBrush;

			Color	color	= node.Color;

			return new SolidColorBrush(color);
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}


