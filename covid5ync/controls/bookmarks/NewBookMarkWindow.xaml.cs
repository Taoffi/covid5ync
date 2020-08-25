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
	/// Interaction logic for NewBookMarkWindow.xaml
	/// </summary>
	public partial class NewBookMarkWindow : Window
	{

		iDnaBookmark		_item;

		public NewBookMarkWindow()
		{
			InitializeComponent();
			this.Loaded += NewBookMarkWindow_Loaded;
		}

		private void NewBookMarkWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if(_item == null)
				_item	= new iDnaBookmark("Your bookmark title", "your bookmark url");

			this.DataContext	= _item;
		}

		private void buttonAddOk_Click(object sender, RoutedEventArgs e)
		{
			if(_item == null)
				return;

			iDnaBookmarkList.Instance.Add(_item);

			this.Close();
		}
	}
}
