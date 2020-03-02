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

			char	code	= node.Code;

			return new SolidColorBrush(iDnaBaseNucleotides.GetBaseColor(code));
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

			char	code		= node.Code;
			Color	color	= iDnaBaseNucleotides.GetBaseColor(code);

			//( 0.8 should darken it; or, for example, *1.25 to brighten it)
			if (node.IsSelected)
				color	= Color.FromArgb((byte)(color.A * 0.30), (byte)(color.R), (byte)(color.G), (byte)(color.B));	// Math.Max(color.B, (byte)1) * 8.25));

			return new SolidColorBrush(color);
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}


