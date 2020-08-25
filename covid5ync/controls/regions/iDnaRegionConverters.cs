using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace iDna.controls
{
	public class iDnaRangeSliderValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return RangeLimitsForRangeSlider.GetHalfOrTwiceValue(value, false);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return RangeLimitsForRangeSlider.GetHalfOrTwiceValue(value, true);
		}
	}

	public static class RangeLimitsForRangeSlider
	{
		public static double GetHalfOrTwiceValue(object objValue, bool convertBack)
		{
			string		strValue	= objValue == null ? "" : objValue.ToString();

			if(string.IsNullOrEmpty(strValue))
				return 0.0;

			double		doubleValue	= 0.0;

			if(! double.TryParse(strValue, out doubleValue))
				return 0.0;

			return convertBack ? doubleValue * 2.0 : doubleValue /2.0;
		}

	}


}
