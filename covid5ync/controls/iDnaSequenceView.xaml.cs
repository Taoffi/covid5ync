using isosoft.root;
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
	/// Interaction logic for iDnaSequenceView.xaml
	/// </summary>
	public partial class iDnaSequenceView : UserControl
	{
		ICommand		_copySelectionToClipboard;

		public iDnaSequenceView()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(Control_Loaded);
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			var presenter		= UiHelpers.FindChild<ScrollContentPresenter>(listItems, null);
			var mouseWheelZoom  = new MouseWheelZoom(presenter);
			PreviewMouseWheel   += mouseWheelZoom.Zoom;
		}


		public ICommand CopySelectionToClipboardCommand
		{
			get
			{
				if(_copySelectionToClipboard == null)
				{
					_copySelectionToClipboard = new CommandExecuter(() =>
					{
						iDnaSequence seq		= this.DataContext as iDnaSequence;

						if(seq == null)
							return;

						var		selectedItems			= seq.SelectedItems;

						if (selectedItems == null || selectedItems.Count() <= 0)
							return;

						string		str			= "";
						int			lastIndex	= 0;

						foreach(var node in selectedItems)
						{
							// for non consecutive selection: add new line
							if (lastIndex > 0 && lastIndex + 1 != node.Index)
								str += "\r\n";

							// the very first and for non consecutive selection: add new line + coordinate
							if (lastIndex <= 0 || lastIndex + 1 != node.Index)
								str += node.Index.ToString() + " ";

							str		+= node.Code;
							lastIndex	= node.Index;
						}

						if(str.Length <= 0)
							return;

						Clipboard.SetText(str, TextDataFormat.UnicodeText);
					});
				}
				return _copySelectionToClipboard;
			}
		}

		private void menuItemCopySelection2Clipboard_Click(object sender, RoutedEventArgs e)
		{
			CopySelectionToClipboardCommand.Execute(null);
		}
	}
}
