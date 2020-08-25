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
	/// Interaction logic for BookMarkListCtrl.xaml
	/// </summary>
	public partial class BookMarkListCtrl : UserControl
	{
		public BookMarkListCtrl()
		{
			InitializeComponent();
			this.Unloaded += BookMarkListCtrl_Unloaded;
		}

		private void BookMarkListCtrl_Unloaded(object sender, RoutedEventArgs e)
		{
			iDnaBookmarkList.Instance.SaveToAppConfig();
			iDnaMainMenu.Instance.BookMarkFavorites.ItemsSource	= null;
			iDnaMainMenu.Instance.BookMarkFavorites.ItemsSource = iDnaBookmarkList.Instance;
		}


		private void buttonDel_Click(object sender, RoutedEventArgs e)
		{
			var			curItem	= dataGrid.SelectedItem as iDnaBookmark;
			var			curList	= dataGrid.ItemsSource	as iDnaBookmarkList;

			if(curItem == null || curList == null)
				return;

			curList.Remove(curItem);
			buttonRefresh_Click(null, null);
		}


		private void buttonNewBookmark_Click(object sender, RoutedEventArgs e)
		{
			NewBookMarkWindow wnd		= new NewBookMarkWindow();
			wnd.Owner = Application.Current.MainWindow;

			wnd.ShowDialog();
			buttonRefresh_Click(null, null);
		}

		private void buttonRefresh_Click(object sender, RoutedEventArgs e)
		{
			var		curList		= dataGrid.ItemsSource;

			dataGrid.ItemsSource	= null;
			dataGrid.ItemsSource	= curList;
		}

		private void buttonOpen_Click(object sender, RoutedEventArgs e)
		{

		}

		private void buttonEdit_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
