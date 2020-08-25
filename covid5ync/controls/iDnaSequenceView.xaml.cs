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
using iDna.vm;

namespace iDna.controls
{
	/// <summary>
	/// Interaction logic for iDnaSequenceView.xaml
	/// </summary>
	public partial class iDnaSequenceView : UserControl
	{
<<<<<<< HEAD
=======
		protected bool				_handleMouseWheelZoom		=true;
		protected MouseWheelZoom	_mouseWheelZoom;

		public bool HandleMouseWheelZoom
		{
			get { return _handleMouseWheelZoom; }
			set
			{
				_handleMouseWheelZoom	= value;
				//if(!value && _mouseWheelZoom !=null)
				//	PreviewMouseWheel -= _mouseWheelZoom.Zoom;
			}
		}

>>>>>>> 5d087e45665096debbc20a0b92888c7a03316a15
		public iDnaSequenceView()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(Control_Loaded);
		}

		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			if(! _handleMouseWheelZoom)
				return;

			var presenter		= UiHelpers.FindChild<ScrollContentPresenter>(listItems, null);
			_mouseWheelZoom		= new MouseWheelZoom(presenter);
			PreviewMouseWheel   += _mouseWheelZoom.Zoom;
		}

		public bool ViewPageIndicatorBar
		{
			get { return this.colPageLocationBar.Width.Value > 0.0; }
			set
			{
				this.colPageLocationBar.Width		= value ? new GridLength(6.0) : new GridLength(0.0);
				borderCurPageIndicator.Visibility	= value ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}
}
