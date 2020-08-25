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

<<<<<<< HEAD
=======
		private void buttonSelectRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(seq == null || objNdx == null)
				return;

			int		minNdx	= objNdx.MinValue,
					maxNdx	= objNdx.MaxValue;

			seq.GoToNodePage(minNdx);
			seq.Select(minNdx, maxNdx, true);
		}
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15

		private void buttonDeleteRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaRegionIndexList	ndxList	= listViewRegions.DataContext as iDnaRegionIndexList;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(ndxList == null || objNdx == null || seq == null)
				return;

<<<<<<< HEAD
			if(MessageBox.Show($"Delete region {objNdx.MinValue.ToString()} - {objNdx.MaxValue.ToString()}?", 
								"Delete region", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
				return;

=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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

<<<<<<< HEAD

		bool CanSelectRegion(int minIndex, int maxIndex)
		{
			iDnaSequence	seq			= iDnaSequence.Instance;
			int				max			= seq == null ? 0 : seq.Count;
			bool			canSelect	= max < minIndex || max < maxIndex ? false : true;

			if(! canSelect)
			{
				MessageBox.Show("Cannot select the region. It might be outside of range. Please check.", "Select region");
			}

			return canSelect;
		}

		private void buttonSelectRegion_Click(object sender, RoutedEventArgs e)
		{
			Button				btn		= sender as Button;
			iDnaRegionIndex		objNdx	= btn == null ? null : btn.DataContext as iDnaRegionIndex;
			iDnaSequence		seq		= iDnaSequence.Instance;

			if(seq == null || objNdx == null)
				return;

			int		minIndex	= objNdx.MinValue,
					maxIndex	= objNdx.MaxValue;

			if(! CanSelectRegion(minIndex, maxIndex))
				return;

			seq.GoToNodePage(minIndex);
			seq.Select(minIndex, maxIndex, true);
		}


=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
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
<<<<<<< HEAD

			if (!CanSelectRegion(minIndex, maxIndex))
				return;

			seq.GoToNodePage(minIndex);
=======
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
			seq.Select(minIndex, maxIndex, resetCurrentSelections: true);
		}
	}
}
