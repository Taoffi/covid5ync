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
	/// Interaction logic for SequenceInfoWindow.xaml
	/// </summary>
	public partial class SequenceNewRegionWindow : Window
	{

		iDnaRegionIndex		_newRegion		 = null;

		public SequenceNewRegionWindow()
		{
			InitializeComponent();
			this.Loaded += SequenceNewRegionWindow_Loaded;
		}

		private void SequenceNewRegionWindow_Loaded(object sender, RoutedEventArgs e)
		{
			iDnaSequence	sequence = iDnaSequence.Instance;

			if(sequence == null)
				return;

			_newRegion		= new iDnaRegionIndex(sequence, "New region");
			this.DataContext	= _newRegion;
		}

		private void buttonOk_Click(object sender, RoutedEventArgs e)
		{
			iDnaSequence sequence = iDnaSequence.Instance;

			if (sequence == null || _newRegion == null)
				return;

			if( ! sequence.AddRegion( _newRegion))
			{
				MessageBox.Show("Sorry! could not add the defined region. Please check if its coordinates do not already exist",
						 "Add region", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			this.Close();
		}
	}
}
