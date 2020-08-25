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

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iDnaSequenceBasket.xaml
	/// </summary>
	public partial class iDnaSequenceBasket : UserControl
	{
		public iDnaSequenceBasket()
		{
			InitializeComponent();
		}


		private void buttonGotoPage_Click(object sender, RoutedEventArgs e)
		{
			Button	button		= sender as Button;

			if(button == null || button.DataContext == null)
				return;

			iDnaSequence	seq	= button.DataContext as iDnaSequence;

			if(seq == null || seq.Count <= 0)
				return;

			iDnaNode	node	= seq[0];
			iDnaSequence.Instance.GoToNodePage(node);
		}

		private void buttonEditInfo_Click(object sender, RoutedEventArgs e)
		{
			Button	button		= sender as Button;

			if(button == null || button.DataContext == null)
				return;

			iDnaSequence	seq	= button.DataContext as iDnaSequence;

			if(seq == null || seq.Count <= 0)
				return;

			SequenceInfoWindow seqWnd		= new SequenceInfoWindow() {  DataContext = seq };

			seqWnd.Owner	= Application.Current.MainWindow;
			seqWnd.ShowDialog();
		}
<<<<<<< HEAD
=======

		private void buttonDeleteItem_Click(object sender, RoutedEventArgs e)
		{
			Button					button		= sender as Button;
			List<iDnaSequence>		basket		= listSequences.ItemsSource as List<iDnaSequence>;
			iDnaSequence			seq			= button == null ? null : button.DataContext as iDnaSequence;

			if (button == null || seq == null || basket ==null)
			{
				MessageBox.Show("Sorry! couldnot access the item or basket!");
				return;
			}

			basket.Remove(seq);

			listSequences.ItemsSource	= null;
			listSequences.ItemsSource	= basket;
			iDnaSequence.Instance.NotifyBasketsChanged();
		}
>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
	}
}
