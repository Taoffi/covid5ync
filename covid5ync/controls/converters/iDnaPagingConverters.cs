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

	public class PagingRectangleMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double		relativePagePosition	= values == null || values.Length < 2 || !(values[0] is double) ? 0.0 : (double) values[0],
						ctrlHeight				= values == null || values.Length < 2 || !(values[1] is double) ? 0.0 : (double)values[1],
						top						= relativePagePosition <= 0 ? 1.0 : relativePagePosition * ctrlHeight;
			top	= Math.Max(0.5, top);
			return new Thickness(0.5, top, 1.0, 0.5);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class PagingRectangleHeightConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double		relativePageHeight	= values == null || values.Length < 2 || !(values[0] is double) ? 0.0 : (double) values[0],
						ctrlHeight			= values == null || values.Length < 2 || !(values[1] is double) ? 0.0 : (double)values[1];
			return ctrlHeight * relativePageHeight;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}


