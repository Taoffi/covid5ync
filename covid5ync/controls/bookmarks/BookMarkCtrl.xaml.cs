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
	/// Interaction logic for BookMarkCtrl.xaml
	/// </summary>
	public partial class BookMarkCtrl : UserControl
	{
		public BookMarkCtrl()
		{
			InitializeComponent();
		}

		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			iDnaBookmark	item		= new iDnaBookmark("new item", "new url");
			var				list		= iDnaBookmarkList.Instance;

			list.InsertNew(item);
			list.SelectedItem	= item;
		}
	}
}
