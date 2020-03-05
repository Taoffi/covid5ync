using System;
using System.Collections.Generic;
using System.IO;
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

namespace iDna
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			iDnaSequence.Instance.SequenceParseCompleted += Instance_SequenceParseCompleted;
			LoadSequence();
		}

		private void Instance_SequenceParseCompleted(iDnaSequence sender, int nodeCount)
		{
			if(sender != null)
			{
				//sequenceCtrl.listItems.ItemsSource = sender;
				textBlockNodeCount.Text = sender.Count.ToString();
			}
		}

		async void LoadSequence()
		{
			await Dispatcher.InvokeAsync(() => vm.iDnaCommandCentral.Instance.LoadBuiltinSequence.Execute(null));

			Uri			appInfoUri		= new Uri("/data/app-version-info.txt", UriKind.Relative);
			Stream		appInfoStream	= Application.GetResourceStream(appInfoUri).Stream;
			string		strAppInfo		= "";

			using (StreamReader reader = new StreamReader(appInfoStream))
			{
				strAppInfo = reader.ReadToEnd();
			}
			appInfoStream.Close();

			textBoxAppInfo.Text	= strAppInfo;
		}
	}
}
