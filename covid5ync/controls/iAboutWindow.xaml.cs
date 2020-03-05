using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iAboutWindow.xaml
	/// </summary>
	public partial class iAboutWindow : Window
	{
		public iAboutWindow()
		{
			InitializeComponent();
			this.Loaded += IAboutWindow_Loaded;

		}

		private void IAboutWindow_Loaded(object sender, RoutedEventArgs e)
		{
			string		strAppInfo		= vm.iDnaCommandCentral.ApplicationVersionInfoString();
			textBoxAppInfo.Text	= strAppInfo;
		}
	}
}
