using isosoft.assemblyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iAboutCtrl.xaml
	/// </summary>
	public partial class iAboutCtrl : UserControl
	{
		public iAboutCtrl()
		{
			InitializeComponent();
			Init_info();
		}

		private void Init_info()
		{
			textVersion.Text		= "version: " + ApplicationVersionInfo.LongVersionString;
			textVersionDate.Text	= string.Format("{0:dd/MM/yyyy HH:mm:ss}", CurrentAssemblyBuildDateOfAsteriskVersion());
				//ApplicationVersionInfo.BuildDateOfAsteriskVersion());	// <= gets the trie core version
		}

		public static DateTime CurrentAssemblyBuildDateOfAsteriskVersion()
		{
			DateTime		date			= new DateTime(2000, 1, 1);
			string[]		parts			= Assembly.GetExecutingAssembly().FullName.Split(',');
			string[]		versionParts	= parts[1].Split('.');

			date	= date.AddDays(Int32.Parse(versionParts[2]));
			date	= date.AddSeconds(Int32.Parse(versionParts[3]) * 2);

			//if (System.TimeZoneInfo.Local.IsDaylightSavingTime(date))
			//{
			//	date = date.AddHours(1);
			//}
			return date;
		}

	}
}
