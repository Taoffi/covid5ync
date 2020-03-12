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
using System.Windows.Navigation;
using System.Windows.Shapes;
using iDna;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iDnaSequenceRegionListCtrl.xaml
	/// </summary>
	public partial class iDnaSequenceRegionListCtrl : UserControl
	{
		public iDnaSequenceRegionListCtrl()
		{
			InitializeComponent();
		}

		private void buttonSelectRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(seq == null || objNdx == null)
				return;

			int		minNdx	= objNdx.MinValue,
					maxNdx	= objNdx.MaxValue;

			seq.Select(minNdx, maxNdx, true);
		}

		private void buttonDeleteRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaRegionIndexList	ndxList	= listViewRegions.DataContext as iDnaRegionIndexList;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(ndxList == null || objNdx == null || seq == null)
				return;

			ndxList.Remove(objNdx);
			seq.RefreshNamedRegions();
		}

		private void buttonNewRegion_Click(object sender, RoutedEventArgs e)
		{
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(seq.Count <= 0)
			{
				MessageBox.Show("This sequence seems to be empty. Please check.", "Add named regions");
				return;
			}

			SequenceNewRegionWindow wnd		= new SequenceNewRegionWindow();
			wnd.Owner		= Application.Current.MainWindow;
			wnd.ShowDialog();
		}

		private void buttonSetSearchRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if (objNdx == null || seq == null || seq.Count <= 0)
				return;

			int				minIndex = objNdx.MinValue,
							maxIndex = objNdx.MaxValue;

			iDnaGobalSettings.Instance.SetSearchRegion(minIndex, maxIndex);
			seq.Select(minIndex, maxIndex, resetCurrentSelections: true);
		}
	}
}
