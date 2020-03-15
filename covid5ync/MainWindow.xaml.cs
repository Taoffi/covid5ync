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
			Loaded		+= MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			iDnaSequence.Instance.SequenceParseCompleted += Instance_SequenceParseCompleted;
			LoadStartupSequence();
		}

		private void Instance_SequenceParseCompleted(iDnaSequence sender, int nodeCount)
		{
			if(sender != null)
			{
				textBlockNodeCount.Text = sender.Count.ToString();
			}
		}

		void LoadStartupSequence()
		{
			string		startuPfile		= App.Instance.StartupFileName;

			if (! string.IsNullOrEmpty(startuPfile))
			{
				vm.iDnaCommandCentral.Instance.LoadXmlFile(startuPfile);
			}
		}
	}
}
