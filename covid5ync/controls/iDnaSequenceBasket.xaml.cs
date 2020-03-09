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
	}
}
