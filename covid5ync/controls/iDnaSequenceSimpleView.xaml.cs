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
	/// Interaction logic for iDnaSequenceSimpleView.xaml
	/// </summary>
	public partial class iDnaSequenceSimpleView : UserControl
	{
	
		public iDnaSequenceSimpleView()
		{
			InitializeComponent();
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
